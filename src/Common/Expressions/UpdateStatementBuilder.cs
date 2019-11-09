﻿/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpdateStatementBuilder : IStatementBuilder<DataUpdateContext>
	{
		#region 常量定义
		private const string TEMPORARY_ALIAS = "tmp";
		#endregion

		#region 构建方法
		public IEnumerable<IStatementBase> Build(DataUpdateContext context)
		{
			var statement = new UpdateStatement(context.Entity);

			//获取要更新的数据模式（模式不为空）
			if(!context.Schema.IsEmpty)
			{
				//依次生成各个数据成员的关联（包括它的子元素集）
				foreach(var member in context.Schema.Members)
				{
					this.BuildSchema(context, statement, statement.Table, context.Data, member);
				}
			}

			//生成条件子句
			statement.Where = this.Where(context, statement);

			if(statement.Fields.Count > 0)
				yield return statement;
		}
		#endregion

		#region 私有方法
		private void BuildSchema(DataUpdateContext context, UpdateStatement statement, TableIdentifier table, object data, SchemaMember member)
		{
			//忽略主键修改，即不能修改主键
			if(member.Token.Property.IsPrimaryKey)
				return;

			//确认当前成员是否有必须的写入值
			var provided = context.Validate(member.Token.Property, out var value);

			//如果不是批量更新，并且当前成员没有改动则返回
			if(!context.IsMultiple && !this.HasChanges(data, member.Name) && !provided)
				return;

			if(member.Token.Property.IsSimplex)
			{
				//忽略不可变属性
				if(member.Token.Property.Immutable && !provided)
					return;

				var field = table.CreateField(member.Token);
				var parameter = provided ?
					Expression.Parameter(field, member, value) :
					Expression.Parameter(field, member);

				//如果当前的数据成员类型为递增步长类型则生成递增表达式
				if(member.Token.MemberType == typeof(Interval))
				{
					/*
					 * 注：默认参数类型为对应字段的类型，而该字段类型可能为无符号数，
					 * 因此当参数类型为无符号数并且步长为负数(递减)，则可能导致参数类型转换溢出，
					 * 所以必须将该参数类型重置为无符号整数。
					 */
					parameter.DbType = System.Data.DbType.Int32;

					//字段设置项的值为字段加参数的加法表达式
					statement.Fields.Add(new FieldValue(field, field.Add(parameter)));
				}
				else
				{
					statement.Fields.Add(new FieldValue(field, parameter));
				}

				statement.Parameters.Add(parameter);
			}
			else
			{
				//不可变复合属性不支持任何写操作，即在修改操作中不能包含不可变复合属性
				if(member.Token.Property.Immutable)
					throw new DataException($"The '{member.FullPath}' is an immutable complex(navigation) property and does not support the update operation.");

				var complex = (IDataEntityComplexProperty)member.Token.Property;

				if(complex.Multiplicity == DataAssociationMultiplicity.Many)
				{
					this.BuildUpsertion(context, data, statement, member);
				}
				else
				{
					if(context.Source.Features.Support(Feature.Updation.Multitable))
						this.BuildComplex(context, statement, data, member);
					else
						this.BuildComplexStandalone(context, statement, data, member);
				}
			}
		}

		private void BuildComplex(DataUpdateContext context, UpdateStatement statement, object data, SchemaMember member)
		{
			if(!member.HasChildren)
				return;

			var table = this.Join(statement, member);

			foreach(var child in member.Children)
			{
				BuildSchema(context, statement, table, member.Token.GetValue(data), child);
			}
		}

		private void BuildComplexStandalone(DataUpdateContext context, UpdateStatement statement, object data, SchemaMember member)
		{
			if(!member.HasChildren)
				return;

			var complex = (IDataEntityComplexProperty)member.Token.Property;
			statement.Returning = new ReturningClause(TableDefinition.Temporary());

			var slave = new UpdateStatement(complex.Foreign, member);
			statement.Slaves.Add(slave);

			//创建从属更新语句的条件子查询语句
			var selection = new SelectStatement(statement.Returning.Table.Identifier());

			foreach(var link in complex.Links)
			{
				if(statement.Returning.Table.Field(link.Principal) != null)
					statement.Returning.Append(statement.Table.CreateField(link.Principal), ReturningClause.ReturningMode.Deleted);

				var field = selection.Table.CreateField(link.Principal);
				selection.Select.Members.Add(field);

				if(selection.Where == null)
					selection.Where = Expression.Equal(field, slave.Table.CreateField(link.Foreign));
				else
					selection.Where = Expression.AndAlso(slave.Where,
					                  Expression.Equal(field, slave.Table.CreateField(link.Foreign)));
			}

			slave.Where = Expression.Exists(selection);

			foreach(var child in member.Children)
			{
				this.BuildSchema(context, slave, slave.Table, member.Token.GetValue(data), child);
			}
		}

		private void BuildUpsertion(DataUpdateContext context, object data, IStatementBase master, SchemaMember schema)
		{
			var entityType = schema.Parent == null ? context.EntityType : schema.Parent.Token.MemberType;

			//构建 Upsert 操作上下文
			var upsert = new DataUpsertContext(
				context.DataAccess,
				schema.Token.Property.Entity.Name,
				true,
				data,
				new Schema(schema.Token.Property.Entity, entityType, schema),
				context.HasStates ? context.States : null);

			//构建 Upsert 语句
			var statements = context.Source.Driver.Builder.Build(upsert);

			//将新建的语句加入到主语句的从属集中
			foreach(var statement in statements)
			{
				master.Slaves.Add(statement);
			}
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool HasChanges(object data, string name)
		{
			switch(data)
			{
				case IModel model:
					return model.HasChanges(name);
				case IDataDictionary dictionary:
					return dictionary.HasChanges(name);
				case IDictionary<string, object> generic:
					return generic.ContainsKey(name);
				case System.Collections.IDictionary classic:
					return classic.Contains(name);
			}

			return true;
		}

		private TableIdentifier Join(UpdateStatement statement, SchemaMember schema)
		{
			if(schema == null || schema.Token.Property.IsSimplex)
				return null;

			//获取关联的源
			ISource source = schema.Parent == null ?
			                 statement.Table :
			                 statement.From.Get(schema.Path);

			//第一步：处理模式成员所在的继承实体的关联
			if(schema.Ancestors != null)
			{
				foreach(var ancestor in schema.Ancestors)
				{
					source = statement.Join(source, ancestor, schema.FullPath);
				}
			}

			//第二步：处理模式成员（导航属性）的关联
			var join = statement.Join(source, schema);
			var target = (TableIdentifier)join.Target;
			statement.Tables.Add(target);

			//返回关联的目标表
			return target;
		}

		private IExpression Where(DataUpdateContext context, UpdateStatement statement)
		{
			if(context.IsMultiple || context.Condition == null)
			{
				var criteria = new ConditionExpression(ConditionCombination.And);

				foreach(var key in statement.Entity.Key)
				{
					if(!statement.Entity.GetTokens(context.EntityType).TryGet(key.Name, out var token))
						throw new DataException($"No required primary key field values were specified for the updation '{statement.Entity.Name}' entity data.");

					var field = statement.Table.CreateField(key);
					var parameter = Expression.Parameter(field, new SchemaMember(token));

					criteria.Add(Expression.Equal(field, parameter));
					statement.Parameters.Add(parameter);
				}

				criteria.Add(statement.Where(context.Validate()));

				return criteria.Count > 0 ? criteria : null;
			}

			return statement.Where(context.Validate());
		}
		#endregion
	}
}
