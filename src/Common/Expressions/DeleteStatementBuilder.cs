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
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class DeleteStatementBuilder : IStatementBuilder<DataDeleteContext>
	{
		#region 常量定义
		private const string TEMPORARY_ALIAS = "tmp";
		#endregion

		#region 构建方法
		public IEnumerable<IStatementBase> Build(DataDeleteContext context)
		{
			if(context.Source.Features.Support(Feature.Deletion.Multitable))
				return this.BuildSimplicity(context);
			else
				return this.BuildComplexity(context);
		}
		#endregion

		#region 虚拟方法
		/// <summary>
		/// 构建支持多表删除的语句。
		/// </summary>
		/// <param name="context">构建操作需要的数据访问上下文对象。</param>
		/// <returns>返回多表删除的语句。</returns>
		protected virtual IEnumerable<IStatementBase> BuildSimplicity(DataDeleteContext context)
		{
			var statement = new DeleteStatement(context.Entity);

			//构建当前实体的继承链的关联集
			this.Join(statement, statement.Table).ToArray();

			//获取要删除的数据模式（模式不为空）
			if(!context.Schema.IsEmpty)
			{
				//依次生成各个数据成员的关联（包括它的继承链、子元素集）
				foreach(var schema in context.Schema.Members)
				{
					this.Join(statement, statement.Table, schema);
				}
			}

			//生成条件子句
			statement.Where = statement.Where(context.Validate());

			yield return statement;
		}

		/// <summary>
		/// 构建单表删除的语句，因为不支持多表删除所以单表删除操作由多条语句以主从树形结构来表达需要进行的多批次的删除操作。
		/// </summary>
		/// <param name="context">构建操作需要的数据访问上下文对象。</param>
		/// <returns>返回的单表删除的多条语句的主句。</returns>
		protected virtual IEnumerable<IStatementBase> BuildComplexity(DataDeleteContext context)
		{
			var statement = new DeleteStatement(context.Entity);

			//生成条件子句
			statement.Where = statement.Where(context.Validate());

			if(!context.Schema.IsEmpty)
				this.BuildReturning(statement, context.Schema.Members);

			yield return statement;
		}
		#endregion

		#region 私有方法
		private void BuildReturning(DeleteStatement statement, IEnumerable<SchemaMember> schemas)
		{
			statement.Returning = new ReturningClause(TableDefinition.Temporary());

			foreach(var key in statement.Entity.Key)
			{
				statement.Returning.Table.Field(key);
				statement.Returning.Append(statement.Table.CreateField(key), ReturningClause.ReturningMode.Deleted);
			}

			var super = statement.Entity.GetBaseEntity();

			while(super != null)
			{
				this.BuildInherit(statement, super);
				super = super.GetBaseEntity();
			}

			foreach(var schema in schemas)
			{
				if(schema.Token.Property.IsSimplex)
					continue;

				var complex = (IDataEntityComplexProperty)schema.Token.Property;
				ISource src = null;

				if(complex.Entity == statement.Entity)
					src = statement.Table;
				else
					src = statement.Join(statement.Table, complex.Entity);

				foreach(var link in complex.Links)
				{
					//某些导航属性可能与主键相同，表定义的字段定义方法（TableDefinition.Field(...)）可避免同名字段的重复定义
					if(statement.Returning.Table.Field(link.Principal) != null)
						statement.Returning.Append(src.CreateField(link.Name), ReturningClause.ReturningMode.Deleted);
				}

				this.BuildSlave(statement, schema);
			}
		}

		private DeleteStatement BuildSlave(DeleteStatement master, SchemaMember schema)
		{
			var complex = (IDataEntityComplexProperty)schema.Token.Property;
			var statement = new DeleteStatement(complex.Foreign, schema);
			var reference = master.Returning.Table.Identifier();

			if(complex.Links.Length == 1)
			{
				var select = new SelectStatement(reference);
				select.Select.Members.Add(reference.CreateField(complex.Links[0].Name));
				statement.Where = Expression.In(statement.Table.CreateField(complex.Links[0].Role), select);
			}
			else
			{
				var join = new JoinClause(TEMPORARY_ALIAS, reference, JoinType.Inner);

				foreach(var link in complex.Links)
				{
					join.Condition.Add(
						Expression.Equal(
							statement.Table.CreateField(link.Role),
							reference.CreateField(link.Name)));
				}

				statement.From.Add(join);
			}

			var super = statement.Entity.GetBaseEntity();

			if(super != null || schema.HasChildren)
			{
				this.BuildReturning(statement, schema.Children);
			}
			else
			{
				master.Slaves.Add(statement);
			}

			return statement;
		}

		private DeleteStatement BuildInherit(DeleteStatement master, IDataEntity entity)
		{
			var statement = new DeleteStatement(entity);
			var reference = master.Returning.Table.Identifier();

			if(entity.Key.Length == 1)
			{
				var select = new SelectStatement(reference);
				select.Select.Members.Add(reference.CreateField(master.Returning.Table.Fields.First().Name));
				statement.Where = Expression.In(statement.Table.CreateField(entity.Key[0]), select);
			}
			else
			{
				var join = new JoinClause(TEMPORARY_ALIAS, reference, JoinType.Inner);

				foreach(var key in entity.Key)
				{
					join.Condition.Add(
						Expression.Equal(
							statement.Table.CreateField(key),
							reference.CreateField(key)));
				}

				statement.From.Add(join);
			}

			master.Slaves.Add(statement);

			return statement;
		}

		private IEnumerable<JoinClause> Join(DeleteStatement statement, TableIdentifier table, string fullPath = null)
		{
			if(table == null || table.Entity == null)
				yield break;

			var super = table.Entity.GetBaseEntity();

			while(super != null)
			{
				var clause = JoinClause.Create(table,
											   fullPath,
											   name => statement.From.TryGet(name, out var join) ? (JoinClause)join : null,
											   entity => statement.CreateTableReference(entity));

				if(!statement.From.Contains(clause))
				{
					statement.From.Add(clause);
					statement.Tables.Add((TableIdentifier)clause.Target);
				}

				super = super.GetBaseEntity();

				yield return clause;
			}
		}

		private void Join(DeleteStatement statement, TableIdentifier table, SchemaMember schema)
		{
			if(table == null || schema == null || schema.Token.Property.IsSimplex)
				return;

			//不可变复合属性不支持任何写操作，即在删除操作中不能包含不可变复合属性
			if(schema.Token.Property.Immutable)
				throw new DataException($"The '{schema.FullPath}' is an immutable complex(navigation) property and does not support the delete operation.");

			var join = statement.Join(table, schema);
			var target = (TableIdentifier)join.Target;
			statement.Tables.Add(target);

			//生成当前导航属性表的继承链关联集
			var joins = this.Join(statement, target, schema.FullPath);

			if(schema.HasChildren)
			{
				foreach(var child in schema.Children)
				{
					if(joins != null)
					{
						join = joins.FirstOrDefault(j => ((TableIdentifier)j.Target).Entity == child.Token.Property.Entity);

						if(join != null)
							target = (TableIdentifier)join.Target;
					}

					this.Join(statement, target, child);
				}
			}
		}
		#endregion
	}
}
