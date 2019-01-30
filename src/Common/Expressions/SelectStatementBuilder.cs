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
			var statement = new SelectStatement(context.Entity) { Paging = context.Paging };

			//生成分组子句
			if(context.Grouping != null)
				this.GenerateGrouping(statement, context.Grouping);

			if(context.Schema != null && !context.Schema.IsEmpty)
			{
				foreach(var entry in context.Schema.Members)
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

		private void GenerateSchema(SelectStatement statement, ISource origin, SchemaMember member)
		{
			if(member.Ancestors != null)
			{
				foreach(var ancestor in member.Ancestors)
				{
					origin = statement.Join(origin, ancestor, member.Path);
				}
			}

			if(member.Token.Property.IsComplex)
			{
				var complex = (IEntityComplexPropertyMetadata)member.Token.Property;

				//一对多的导航属性对应一个新语句（新语句别名即为该导航属性的全称）
				if(complex.Multiplicity == AssociationMultiplicity.Many)
				{
					var slave = new SelectStatement(complex.Foreign, member.FullPath) { Paging = member.Paging };
					var table = slave.Table;

					if(complex.ForeignProperty != null)
					{
						if(complex.ForeignProperty.IsSimplex)
							slave.Select.Members.Add(slave.Table.CreateField(complex.ForeignProperty));
						else
							table = (TableIdentifier)slave.Join(slave.Table, (IEntityComplexPropertyMetadata)complex.ForeignProperty).Target;
					}

					statement.Slaves.Add(slave);

					//为一对多的导航属性增加必须的链接字段及对应的条件参数
					foreach(var link in complex.Links)
					{
						var principalField = origin.CreateField(link.Principal);
						principalField.Alias = "$" + member.FullPath + ":" + link.Name;
						statement.Select.Members.Add(principalField);

						var foreignField = slave.Table.CreateField(link.Foreign);
						foreignField.Alias = null;
						slave.Where = Expression.Equal(foreignField, slave.Parameters.Add(link.Name, link.Foreign.Type));
					}

					if(member.Sortings != null)
						this.GenerateSortings(slave, table, member.Sortings);

					if(member.HasChildren)
					{
						foreach(var child in member.Children)
						{
							this.GenerateSchema(slave, table, child);
						}
					}

					return;
				}

				//对于一对一的导航属性，创建其关联子句即可
				origin = statement.Join(origin, complex, member.FullPath);
			}
			else
			{
				var field = origin.CreateField(member.Token.Property);

				//只有数据模式元素是导航子元素以及与当前语句的别名不同（相同则表示为同级），才需要指定字段引用的别名
				if(member.Parent != null && !string.Equals(member.Path, statement.Alias, StringComparison.OrdinalIgnoreCase))
				{
					if(string.IsNullOrEmpty(statement.Alias))
						field.Alias = member.FullPath;
					else
						field.Alias = Zongsoft.Common.StringExtension.TrimStart(member.FullPath, statement.Alias + ".", StringComparison.OrdinalIgnoreCase);
				}

				statement.Select.Members.Add(field);
			}

			if(member.HasChildren)
			{
				foreach(var child in member.Children)
				{
					this.GenerateSchema(statement, origin, child);
				}
			}
		}
		#endregion
	}
}
