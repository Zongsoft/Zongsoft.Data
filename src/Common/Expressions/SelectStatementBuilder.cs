/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using Zongsoft.Reflection;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class SelectStatementBuilder
	{
		public IEnumerable<SelectStatement> Build(DataSelectionContext context)
		{
			var entity = DataEnvironment.Metadata.Entities.Get(context.Name);
			var members = EntityMemberProvider.Default.GetMembers(context.EntityType);

			var table = new TableIdentifier(entity.Name, "T");
			var statement = new SelectStatement(table);

			if(context.Grouping != null)
			{
				this.GenerateGrouping(statement, entity, context.Grouping);
			}
			else
			{
				var scopes = Utility.RinseScope(context.Scope, entity, members);

				foreach(var scope in scopes)
				{
					if(entity.Properties.TryGet(scope, out var property))
					{
						if(scope.Contains(".") || property.IsComplex)
							this.GenerateFromAndSelect(statement, entity, scope);
						else
							statement.Select.Members.Add(table.CreateField(property.Name));
					}
				}

				if(context.Condition != null)
					statement.Where = GenerateCondition(statement, entity, context.Condition);
			}

			if(context.Sortings != null && context.Sortings.Length > 0)
			{
				statement.OrderBy = new OrderByClause();

				foreach(var sorting in context.Sortings)
				{
					var token = GenerateFrom(statement, entity, sorting.Name);
					statement.OrderBy.Add(token.CreateField(), sorting.Mode);
				}
			}

			return new SelectStatement[] { statement };
		}

		private void GenerateGrouping(SelectStatement statement, IEntity entity, Grouping grouping)
		{
			PropertyToken token;

			foreach(var key in grouping.Keys)
			{
				token = this.GenerateFrom(statement, entity, key.Name);

				if(token.Property.IsComplex)
					throw new DataException();

				statement.GroupBy.Members.Add(token.CreateField());
				statement.Select.Members.Add(token.CreateField(key.Alias));
			}

			if(grouping.Condition != null)
			{
				statement.GroupBy.Having = GenerateCondition(statement, entity, grouping.Condition);
			}

			foreach(var aggregation in grouping.Aggregations)
			{
				token = this.GenerateFrom(statement, entity, aggregation.Name);

				if(token.Property.IsComplex)
					throw new DataException();

				statement.Select.Members.Add(new AggregationExpression(aggregation.Method, token.CreateField()) { Alias = aggregation.Alias });
			}
		}

		private PropertyToken GenerateFrom(SelectStatement statement, IEntity entity, string field)
		{
			JoinClause joining = null;

			var found = entity.Properties.Find(field, (path, property) =>
			{
				if(property.IsSimplex || statement.From.Contains(path))
					return;

				var complex = (IEntityComplexProperty)property;
				var target = statement.CreateTable(property.Entity.Name);
				joining = new JoinClause(path, target, JoinType.Left);

				if(complex.Relationship.Members.Length > 1)
				{
					var conditions = ConditionExpression.And();

					foreach(var on in complex.Relationship.Members)
					{
						conditions.Add(
							BinaryExpression.Equal(
								statement.From.First().CreateField(on.Principal.Name),
								target.CreateField(on.Foreign.Name)));
					}

					joining.Condition = conditions;
				}
				else
				{
					joining.Condition = BinaryExpression.Equal(
								statement.From.First().CreateField(complex.Relationship.Members[0].Principal.Name),
								target.CreateField(complex.Relationship.Members[0].Foreign.Name));
				}

				statement.From.Add(joining);
			});

			if(found == null)
				throw new DataException();

			return new PropertyToken(found, joining);
		}

		private void GenerateFromAndSelect(SelectStatement statement, IEntity entity, string field)
		{
			var token = this.GenerateFrom(statement, entity, field);

			if(token.Property != null)
			{
				if(token.Property.IsSimplex)
				{
					statement.Select.Members.Add(token.CreateField());
				}
				else
				{
					var complex = (IEntityComplexProperty)token.Property;

					foreach(var property in complex.Relationship.Foreign.Properties.Where(p => p.IsSimplex))
					{
						statement.Select.Members.Add(token.CreateField());
					}
				}
			}
		}

		private BinaryExpression GenerateCondition(SelectStatement statement, IEntity entity, ICondition condition)
		{
			if(condition is Condition c)
			{
				return ConditionExtension.ToExpression(c, field => GenerateFrom(statement, entity, field).CreateField());
			}
			else if(condition is ConditionCollection cs)
			{
				return ConditionExtension.ToExpression(cs, field => GenerateFrom(statement, entity, field).CreateField());
			}

			return null;
		}

		private struct PropertyToken
		{
			public PropertyToken(IEntityProperty property, ISource source)
			{
				this.Property = property;
				this.Source = source;
			}

			public IEntityProperty Property;
			public ISource Source;

			public FieldIdentifier CreateField(string alias = null)
			{
				return this.Source.CreateField(this.Property.Name, alias);
			}
		}
	}
}
