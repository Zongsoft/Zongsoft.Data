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
	public class SelectStatementBuilder : IStatementBuilder<DataSelectContext>
	{
		#region 构建方法
		public IEnumerable<IStatement> Build(DataSelectContext context)
		{
			var statement = new SelectStatement(context.Entity);

			if(context.Schema != null && !context.Schema.IsEmpty)
			{
				foreach(var entry in context.Schema.Entries)
				{
					this.GenerateSchema(statement, statement.Table, entry);
				}
			}

			//生成条件子句
			statement.Where = this.GenerateCondition(statement, context.Condition);

			//生成分组子句
			this.GenerateGrouping(statement, context.Grouping);

			//生成排序子句
			this.GenerateSortings(statement, statement.Table, context.Sortings);

			yield return statement;
		}
		#endregion

		#region 私有方法
		private void GenerateGrouping(SelectStatement statement, Grouping grouping)
		{
			if(grouping == null)
				return;

			if(grouping.Keys != null && grouping.Keys.Length > 0)
			{
				//创建分组子句
				statement.GroupBy = new GroupByClause();

				foreach(var key in grouping.Keys)
				{
					var source = this.EnsureSource(statement, null, key.Name, out var property);

					if(property.IsComplex)
						throw new DataException($"The grouping key '{property.Name}' can not be a complex property.");

					statement.GroupBy.Keys.Add(source.CreateField(property));
					statement.Select.Members.Add(source.CreateField(property.GetFieldName(out var alias), key.Alias ?? alias));
				}

				if(grouping.Filter != null)
				{
					statement.GroupBy.Having = GenerateCondition(statement, grouping.Filter);
				}
			}

			foreach(var aggregate in grouping.Aggregates)
			{
				if(string.IsNullOrEmpty(aggregate.Name) || aggregate.Name == "*")
				{
					statement.Select.Members.Add(
						new AggregateExpression(aggregate.Method, Expression.Constant(0))
						{
							Alias = string.IsNullOrEmpty(aggregate.Alias) ? aggregate.Method.ToString() : aggregate.Alias
						});
				}
				else
				{
					var source = this.EnsureSource(statement, null, aggregate.Name, out var property);

					if(property.IsComplex)
						throw new DataException($"The field '{property.Name}' of aggregate function can not be a complex property.");

					statement.Select.Members.Add(
						new AggregateExpression(aggregate.Method, source.CreateField(property))
						{
							Alias = string.IsNullOrEmpty(aggregate.Alias) ? aggregate.Name : aggregate.Alias
						});
				}
			}
		}

		private void GenerateSortings(SelectStatement statement, TableIdentifier origin, Sorting[] sortings)
		{
			if(sortings == null || sortings.Length == 0)
				return;

			statement.OrderBy = new OrderByClause();

			foreach(var sorting in sortings)
			{
				var source = this.EnsureSource(statement, origin, sorting.Name, out var property);
				statement.OrderBy.Add(source.CreateField(property), sorting.Mode);
			}
		}

		private void GenerateSchema(SelectStatement statement, ISource source, SchemaEntry entry)
		{
			IEntityComplexPropertyMetadata complex = null;

			if(entry.Token.Property.IsComplex)
			{
				complex = (IEntityComplexPropertyMetadata)entry.Token.Property;

				if(complex.Multiplicity == AssociationMultiplicity.Many)
				{
					TableIdentifier table;

					if(statement.Into == null)
						statement.Into = table = TableIdentifier.Temporary(null);
					else
						table = statement.Into as TableIdentifier;

					var slave = new SelectStatement(table, entry.FullPath) { Paging = entry.Paging };
					var join = slave.Join(slave.Table, complex);
					statement.Slaves.Add(slave);

					if(entry.Sortings != null && join.Target is TableIdentifier origin)
						this.GenerateSortings(slave, origin, entry.Sortings);

					if(entry.HasChildren)
					{
						foreach(var child in entry.Children)
						{
							this.GenerateSchema(slave, join, child);
						}
					}

					return;
				}
			}

			if(entry.Ancestors != null)
			{
				foreach(var ancestor in entry.Ancestors)
				{
					source = statement.Join(source, ancestor, entry.Path);

					if(!statement.From.Contains(source))
						statement.From.Add(source);
				}
			}

			if(complex != null)
			{
				source = statement.Join(source, complex, entry.FullPath);
			}
			else
			{
				var field = source.CreateField(entry.Token.Property);

				if(entry.Parent != null)
					field.Alias = entry.FullPath;

				statement.Select.Members.Add(field);
			}

			if(entry.HasChildren)
			{
				foreach(var child in entry.Children)
				{
					this.GenerateSchema(statement, source, child);
				}
			}
		}

		private ISource EnsureSource(SelectStatement statement, TableIdentifier origin, string memberPath, out IEntityPropertyMetadata property)
		{
			if(origin == null)
				origin = statement.Table;

			var found = origin.Reduce(memberPath, ctx =>
			{
				var source = ctx.Source;

				if(ctx.Ancestors != null)
				{
					foreach(var ancestor in ctx.Ancestors)
					{
						source = statement.Join(source, ancestor, ctx.Path);

						if(!statement.From.Contains(source))
							statement.From.Add(source);
					}
				}

				if(ctx.Property.IsComplex)
					source = statement.Join(source, (IEntityComplexPropertyMetadata)ctx.Property, ctx.FullPath);

				return source;
			});

			if(found.IsFailed)
				throw new DataException($"The specified '{memberPath}' member does not exist in the '{statement.Entity}' entity.");

			//输出找到的属性元素
			property = found.Property;

			//返回找到的源
			return found.Source;
		}

		private IExpression GenerateCondition(SelectStatement statement, ICondition condition)
		{
			if(condition == null)
				return null;

			if(condition is Condition c)
				return ConditionExtension.ToExpression(c, field => EnsureSource(statement, null, field, out var property).CreateField(property), (_, __) => statement.CreateParameter(_, __));

			if(condition is ConditionCollection cc)
				return ConditionExtension.ToExpression(cc, field => EnsureSource(statement, null, field, out var property).CreateField(property), (_, __) => statement.CreateParameter(_, __));

			throw new NotSupportedException($"The '{condition.GetType().FullName}' type is an unsupported condition type.");
		}
		#endregion
	}
}
