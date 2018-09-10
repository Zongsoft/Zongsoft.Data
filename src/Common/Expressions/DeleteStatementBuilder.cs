/*
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
	public class DeleteStatementBuilder : IStatementBuilder
	{
		#region 常量定义
		private const string MAIN_INHERIT_PREFIX = "base:";
		#endregion

		#region 公共方法
		IEnumerable<IStatement> IStatementBuilder.Build(IDataAccessContextBase context, IDataSource source)
		{
			if(context.Method == DataAccessMethod.Delete)
				return this.Build((DataDeleteContext)context, source);

			//抛出数据异常
			throw new DataException($"The {this.GetType().Name} builder does not support the {context.Method} operation.");
		}

		public IEnumerable<IStatement> Build(DataDeleteContext context, IDataSource source)
		{
			if(string.IsNullOrEmpty(context.Schema) || source.Driver.Features.Support(DeleteFeatures.Multitable))
				yield return this.BuildSimplicity(context);
			else
				yield return this.BuildComplexity(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual DeleteStatement CreateStatement(IEntityMetadata entity, TableIdentifier table)
		{
			return new DeleteStatement(entity, table);
		}

		protected virtual IStatement BuildSimplicity(DataDeleteContext context)
		{
			var table = new TableIdentifier(context.Entity, "T");
			var statement = this.CreateStatement(context.Entity, table);

			//构建当前实体的继承链的关联集
			this.Join(statement, table);

			//获取要删除的数据模式（可能为空）
			var schemas = context.Schemas;

			if(schemas != null && schemas.Count > 0)
			{
				//依次生成各个数据成员的关联（包括它的继承链、子元素集）
				foreach(var schema in schemas)
				{
					this.Join(statement, table, schema);
				}
			}

			if(context.Condition != null)
				statement.Where = GenerateCondition(statement, context.Condition);

			return statement;
		}

		protected virtual IStatement BuildComplexity(DataDeleteContext context)
		{
			var temp = TableDefinition.Temporary("xxx");

			throw new NotImplementedException();
		}
		#endregion

		#region 私有方法
		private JoinClause Join(DeleteStatement statement, IEntityMetadata entity, string fullPath)
		{
			var join = statement.Join(entity, fullPath);

			statement.From.Add(join);

			return join;
		}

		private IEnumerable<JoinClause> Join(DeleteStatement statement, TableIdentifier table, string fullPath = null)
		{
			if(table == null || table.Entity == null)
				yield break;

			var super = table.Entity.GetBaseEntity();

			while(super != null)
			{
				var join = statement.Join(table, fullPath);
				statement.Tables.Add((TableIdentifier)join.Target);
				statement.From.Add(join);
				super = super.GetBaseEntity();

				yield return join;
			}
		}

		private void Join(DeleteStatement statement, TableIdentifier table, Schema schema)
		{
			if(table == null || schema == null || schema.Token.Property.IsSimplex)
				return;

			var join = statement.Join(table, schema);
			var target = (TableIdentifier)join.Target;
			statement.Tables.Add(target);
			statement.From.Add(join);

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

		private JoinClause Join(DeleteStatement statement, IEntityComplexPropertyMetadata complex, string fullPath)
		{
			if(statement.From.TryGet(fullPath, out var source))
				return source as JoinClause;

			return null;
		}

		private ISource EnsureSource(DeleteStatement statement, string memberPath)
		{
			ISource source = null;

			var found = statement.Entity.Properties.Find(memberPath, ctx =>
			{
				if(ctx.Ancestors != null)
				{
					foreach(var ancestor in ctx.Ancestors)
					{
						source = this.Join(statement, ancestor, ctx.Path);
					}
				}

				if(ctx.Property.IsComplex)
				{
					var complex = (IEntityComplexPropertyMetadata)ctx.Property;
					source = this.Join(statement, complex, ctx.FullPath);
				}
			});

			return source;
		}

		private IExpression GenerateCondition(DeleteStatement statement, ICondition condition)
		{
			if(condition == null)
				return null;

			if(condition is Condition c)
				return ConditionExtension.ToExpression(c, field => EnsureSource(statement, field).CreateField(field), (_, __) => statement.CreateParameter(_, __));

			if(condition is ConditionCollection cc)
				return ConditionExtension.ToExpression(cc, field => EnsureSource(statement, field).CreateField(field), (_, __) => statement.CreateParameter(_, __));

			throw new NotSupportedException($"The '{condition.GetType().FullName}' type is an unsupported condition type.");
		}
		#endregion
	}
}
