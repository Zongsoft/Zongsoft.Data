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
	public class SelectStatementBuilder : SelectStatementBuilderBase<DataSelectContext>
	{
		#region 构建方法
		public override IEnumerable<IStatement> Build(DataSelectContext context)
		{
			var statement = new SelectStatement(context.Entity);

			if(context.Grouping != null)
			{
				//生成分组子句
				this.GenerateGrouping(statement, context.Grouping);
			}
			else if(context.Schema != null && !context.Schema.IsEmpty)
			{
				foreach(var entry in context.Schema.Entries)
				{
					//生成数据模式对应的子句
					this.GenerateSchema(statement, statement.Table, entry);
				}
			}

			//生成条件子句
			statement.Where = this.GenerateCondition(statement, context.Condition);

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

		private void GenerateSchema(SelectStatement statement, ISource origin, SchemaEntry entry)
		{
			if(entry.Ancestors != null)
			{
				foreach(var ancestor in entry.Ancestors)
				{
					origin = statement.Join(origin, ancestor, entry.Path);
				}
			}

			if(entry.Token.Property.IsComplex)
			{
				var complex = (IEntityComplexPropertyMetadata)entry.Token.Property;

				//一对多的导航属性对应一个新语句（新语句别名即为该导航属性的全称）
				if(complex.Multiplicity == AssociationMultiplicity.Many)
				{
					var slave = new SelectStatement(complex.GetForeignEntity(out var foreignProperty), entry.FullPath) { Paging = entry.Paging };
					var table = slave.Table;

					if(foreignProperty.IsSimplex)
						slave.Select.Members.Add(slave.Table.CreateField(foreignProperty));
					else
						table = (TableIdentifier)slave.Join(slave.Table, (IEntityComplexPropertyMetadata)foreignProperty).Target;

					statement.Slaves.Add(slave);

					//为一对多的导航属性增加必须的条件参数
					foreach(var link in complex.Links)
					{
						var field = slave.Table.CreateField(slave.Table.Entity.Properties.Get(link.Role));
						field.Alias = null;
						slave.Where = Expression.Equal(field, Expression.Parameter(field.Name));
					}

					if(entry.Sortings != null)
						this.GenerateSortings(slave, table, entry.Sortings);

					if(entry.HasChildren)
					{
						foreach(var child in entry.Children)
						{
							this.GenerateSchema(slave, table, child);
						}
					}

					return;
				}

				//对于一对一的导航属性，创建其关联子句即可
				origin = statement.Join(origin, complex, entry.FullPath);
			}
			else
			{
				var field = origin.CreateField(entry.Token.Property);

				//只有数据模式元素是导航子元素以及与当前语句的别名不同（相同则表示为同级），才需要指定字段引用的别名
				if(entry.Parent != null && !string.Equals(entry.Path, statement.Alias, StringComparison.OrdinalIgnoreCase))
				{
					if(string.IsNullOrEmpty(statement.Alias))
						field.Alias = entry.FullPath;
					else
						field.Alias = Zongsoft.Common.StringExtension.TrimStart(entry.FullPath, statement.Alias + ".", StringComparison.OrdinalIgnoreCase);
				}

				statement.Select.Members.Add(field);
			}

			if(entry.HasChildren)
			{
				foreach(var child in entry.Children)
				{
					this.GenerateSchema(statement, origin, child);
				}
			}
		}
		#endregion
	}
}
