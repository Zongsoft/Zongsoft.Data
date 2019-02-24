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
			if(context.Source.Features.Support(Feature.Updation.Multitable))
				return this.BuildSimplicity(context);
			else
				return this.BuildComplexity(context);
		}
		#endregion

		#region 虚拟方法
		/// <summary>
		/// 构建支持多表更新的语句。
		/// </summary>
		/// <param name="context">构建操作需要的数据访问上下文对象。</param>
		/// <returns>返回多表更新的语句。</returns>
		protected virtual IEnumerable<IStatementBase> BuildSimplicity(DataUpdateContext context)
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

			yield return statement;
		}

		/// <summary>
		/// 构建单表更新的语句，因为不支持多表更新所以单表更新操作由多条语句以主从树形结构来表达需要进行的多批次的更新操作。
		/// </summary>
		/// <param name="context">构建操作需要的数据访问上下文对象。</param>
		/// <returns>返回的单表更新的多条语句的主句。</returns>
		protected virtual IEnumerable<IStatementBase> BuildComplexity(DataUpdateContext context)
		{
			var statement = new UpdateStatement(context.Entity);

			//生成条件子句
			statement.Where = this.Where(context, statement);

			return null;
		}
		#endregion

		#region 私有方法
		private void BuildSchema(DataUpdateContext context, UpdateStatement statement, TableIdentifier table, object data, SchemaMember member)
		{
			//忽略主键修改，即不能修改主键
			if(member.Token.Property.IsPrimaryKey)
				return;

			//如果不是批量更新，并且当前成员没有改动则返回
			if(!context.IsMultiple && !this.HasChanges(data, member.Name))
				return;

			if(member.Token.Property.IsSimplex)
			{
				var field = table.CreateField(member.Token);
				var parameter = Expression.Parameter(ParameterExpression.Anonymous, member, field);

				statement.Fields.Add(new FieldValue(field, parameter));
				statement.Parameters.Add(parameter);
			}
			else
			{
				var complex = (IEntityComplexPropertyMetadata)member.Token.Property;

				if(complex.Multiplicity == AssociationMultiplicity.Many)
					this.BuildUpsertion(statement, member);
				else
					table = this.Join(statement, member);

				if(member.HasChildren)
				{
					foreach(var child in member.Children)
					{
						BuildSchema(context, statement, table, member.Token.GetValue(data), child);
					}
				}
			}
		}

		private IStatementBase BuildUpsertion(IStatementBase master, SchemaMember schema)
		{
			var complex = (IEntityComplexPropertyMetadata)schema.Token.Property;
			var statement = new UpsertStatement(complex.Foreign, schema);

			foreach(var member in schema.Children)
			{
				if(member.Token.Property.IsComplex)
					continue;

				var property = (IEntitySimplexPropertyMetadata)member.Token.Property;

				var field = statement.Table.CreateField(property);
				var parameter = this.IsLinked(complex, property) ?
				                Expression.Parameter(property.Name, property.Type) :
				                Expression.Parameter(ParameterExpression.Anonymous, member, field);

				//设置参数的默认值
				parameter.Value = property.Value;

				statement.Fields.Add(field);
				statement.Values.Add(parameter);
				statement.Parameters.Add(parameter);
			}

			//将新建的语句加入到主语句的从属集中
			master.Slaves.Add(statement);

			return statement;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool HasChanges(object data, string name)
		{
			if(data == null)
				return false;

			switch(data)
			{
				case IEntity entity:
					return entity.HasChanges(name);
				case IDataDictionary dictionary:
					return dictionary.HasChanges(name);
				case IDictionary<string, object> generic:
					return generic.ContainsKey(name);
				case System.Collections.IDictionary classic:
					return classic.Contains(name);
			}

			return true;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool IsLinked(IEntityComplexPropertyMetadata owner, IEntitySimplexPropertyMetadata property)
		{
			var links = owner.Links;

			for(int i = 0; i < links.Length; i++)
			{
				if(object.Equals(links[i].Foreign, property))
					return true;
			}

			return false;
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
					var parameter = Expression.Parameter(ParameterExpression.Anonymous, new SchemaMember(token), field);

					criteria.Add(Expression.Equal(field, parameter));
					statement.Parameters.Add(parameter);
				}

				if(context.Condition != null)
					criteria.Add(statement.Where(context.Condition));

				return criteria.Count > 0 ? criteria : null;
			}

			return statement.Where(context.Condition);
		}
		#endregion
	}
}
