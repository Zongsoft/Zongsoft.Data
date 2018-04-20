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
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Reflection;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class SelectStatementBuilder
	{
		#region 常量定义
		private const string JOINCLAUSE_INHERIT_PREFIX = "base:";
		#endregion

		public IEnumerable<SelectStatement> Build(DataSelectionContext context)
		{
			var entity = context.GetEntity();
			var table = new TableIdentifier(entity, "T");
			var statement = new SelectStatement(table);

			if(context.Grouping != null)
			{
				this.GenerateGrouping(context, statement, context.Grouping);
			}
			else
			{
				var scopes = Utility.ResolveScope(context.Scope, entity, context.GetEntityMembers());

				foreach(var scope in scopes)
				{
					this.GenerateFromAndSelect(context, statement, scope);
				}
			}

			if(context.Condition != null)
				statement.Where = GenerateCondition(context, statement, context.Condition);

			if(context.Sortings != null && context.Sortings.Length > 0)
			{
				statement.OrderBy = new OrderByClause();

				foreach(var sorting in context.Sortings)
				{
					var token = GenerateFrom(context, statement, sorting.Name);
					statement.OrderBy.Add(token.CreateField(), sorting.Mode);
				}
			}

			return new SelectStatement[] { statement };
		}

		private void GenerateGrouping(DataSelectionContext context, SelectStatement statement, Grouping grouping)
		{
			PropertyToken token;

			//创建分组子句
			statement.GroupBy = new GroupByClause();

			foreach(var key in grouping.Keys)
			{
				token = this.GenerateFrom(context, statement, key.Name);

				if(token.Property.IsComplex)
					throw new DataException($"The grouping key '{token.Property.Name}' can not be a complex property.");

				statement.GroupBy.Keys.Add(token.CreateField());
				statement.Select.Members.Add(token.CreateField(key.Alias));
			}

			if(grouping.Filter != null)
			{
				statement.GroupBy.Having = GenerateCondition(context, statement, grouping.Filter);
			}

			foreach(var aggregate in grouping.Aggregates)
			{
				if(string.IsNullOrEmpty(aggregate.Name) || aggregate.Name == "*")
				{
					statement.Select.Members.Add(
						new AggregateExpression(aggregate.Method, ConstantExpression.Create(0))
						{
							Alias = string.IsNullOrEmpty(aggregate.Alias) ? aggregate.Method.ToString() : aggregate.Alias
						});
				}
				else
				{
					token = this.GenerateFrom(context, statement, aggregate.Name);

					if(token.Property.IsComplex)
						throw new DataException($"The field '{token.Property.Name}' of aggregate function can not be a complex property.");

					statement.Select.Members.Add(
						new AggregateExpression(aggregate.Method, token.CreateField())
						{
							Alias = string.IsNullOrEmpty(aggregate.Alias) ? aggregate.Name : aggregate.Alias
						});
				}
			}

			if(!string.IsNullOrEmpty(context.Scope))
			{
				var scopes = Utility.ResolveScope(context.Scope, context.GetEntity(), context.GetEntityMembers());

				foreach(var scope in scopes)
				{
					this.GenerateFromAndSelect(context, statement, scope);
				}
			}
		}

		private PropertyToken GenerateFrom(DataSelectionContext context, SelectStatement statement, string field)
		{
			ISource source = null;
			var entity = context.GetEntity();

			var found = entity.Properties.Find(field, (path, property) =>
			{
				if(property.IsSimplex)
				{
					var index = path.LastIndexOf(".");

					if(index > 0)
						source = statement.From.Get(path.Substring(0, index));

					return;
				}

				if(statement.From.Contains(path))
					return;

				IExpression value;
				var complex = (IEntityComplexProperty)property;
				var target = statement.CreateTable(property.Entity);
				source = new JoinClause(path, target, JoinType.Left);

				if(complex.Links.Length > 1)
				{
					var conditions = ConditionExpression.And();

					foreach(var link in complex.Links)
					{
						if(string.IsNullOrWhiteSpace(link.Role))
							value = ConstantExpression.Create(link.Value);
						else
						{
							var index = path.LastIndexOf(".");

							if(index < 0)
								value = this.EnsureBaseSource(context, statement, link.Role).CreateField(link.Role);
							else
								value = statement.From.Get(path.Substring(0, index)).CreateField(link.Role);
						}

						conditions.Add(BinaryExpression.Equal(target.CreateField(link.Name), value));
					}

					((JoinClause)source).Condition = conditions;
				}
				else
				{
					if(string.IsNullOrWhiteSpace(complex.Links[0].Role))
						value = ConstantExpression.Create(complex.Links[0].Value);
					else
					{
						var index = path.LastIndexOf(".");

						if(index < 0)
							value = this.EnsureBaseSource(context, statement, complex.Links[0].Role).CreateField(complex.Links[0].Role);
						else
							value = statement.From.Get(path.Substring(0, index)).CreateField(complex.Links[0].Role);
					}

					((JoinClause)source).Condition = BinaryExpression.Equal(target.CreateField(complex.Links[0].Name), value);
				}

				statement.From.Add(source);
			});

			if(found == null)
				throw new DataException($"The specified '{field}' field is not existed.");

			if(source == null && found.IsSimplex)
				source = this.GenerateBaseFrom(context, statement, found.Entity);

			return new PropertyToken(found, source ?? statement.From.First());
		}

		private ISource EnsureBaseSource(DataSelectionContext context, SelectStatement statement, string field)
		{
			var entity = context.GetEntity();

			if(entity.Properties.Contains(field))
				return statement.From.First();

			while((entity = entity.GetBaseEntity()) != null)
			{
				if(entity.Properties.Contains(field))
				{
					if(statement.From.TryGet(JOINCLAUSE_INHERIT_PREFIX + entity.Name, out var source))
						return source;

					return this.GenerateBaseFrom(context, statement, entity);
				}
			}

			throw new DataException($"The specified '{field}' field is not existed.");
		}

		private ISource GenerateBaseFrom(DataSelectionContext context, SelectStatement statement, IEntity baseEntity)
		{
			var deriveEntity = context.GetEntity();

			if(deriveEntity.Equals(baseEntity))
				return statement.From.First();

			var joiningName = JOINCLAUSE_INHERIT_PREFIX + baseEntity.Name;

			if(statement.From.TryGet(joiningName, out var source))
				return source;

			var baseTable = statement.CreateTable(baseEntity);
			var conditions = ConditionExpression.And();
			source = new JoinClause(joiningName, baseTable, JoinType.Inner)
			{
				Condition = conditions
			};

			for(var i = 0; i < baseEntity.Key.Length; i++)
			{
				conditions.Add(
					BinaryExpression.Equal(baseTable.CreateField(baseEntity.Key[i]),
						statement.From.First().CreateField(deriveEntity.Key[i].Name)));
			}

			statement.From.Add(source);

			return source;
		}

		private void GenerateFromAndSelect(DataSelectionContext context, SelectStatement statement, string field)
		{
			var token = this.GenerateFrom(context, statement, field);

			if(token.Property != null)
			{
				if(token.Property.IsSimplex)
				{
					statement.Select.Members.Add(token.CreateField(field.Contains(".") ? field : null));
				}
				else
				{
					var complex = (IEntityComplexProperty)token.Property;
					var members = context.GetEntityMembers(field);

					foreach(var property in complex.GetForeignEntity().Properties.Where(p => p.IsSimplex && (members == null || members.Contains(p.Name))))
					{
						statement.Select.Members.Add(token.CreateField((IEntitySimplexProperty)property, field));
					}
				}
			}
		}

		private IExpression GenerateCondition(DataSelectionContext context, SelectStatement statement, ICondition condition)
		{
			if(condition is Condition c)
			{
				return ConditionExtension.ToExpression(c, field => GenerateFrom(context, statement, field).CreateField());
			}
			else if(condition is ConditionCollection cs)
			{
				return ConditionExtension.ToExpression(cs, field => GenerateFrom(context, statement, field).CreateField());
			}

			return null;
		}

		#region 嵌套结构
		private struct PropertyToken
		{
			#region 公共字段
			public IEntityProperty Property;
			public ISource Source;
			#endregion

			#region 构造函数
			public PropertyToken(IEntityProperty property, ISource source)
			{
				this.Property = property;
				this.Source = source;
			}
			#endregion

			#region 公共方法
			public FieldIdentifier CreateField(string alias = null)
			{
				if(string.IsNullOrEmpty(this.Property.Alias))
					return this.Source.CreateField(this.Property.Name, alias);
				else
					return this.Source.CreateField(this.Property.Alias, alias);
			}

			public FieldIdentifier CreateField(IEntitySimplexProperty property, string path)
			{
				var alias = string.IsNullOrEmpty(path) ? null : path + "." + property.Name;

				if(string.IsNullOrEmpty(property.Alias))
					return this.Source.CreateField(property.Name, alias);
				else
					return this.Source.CreateField(property.Alias, alias);
			}
			#endregion
		}
		#endregion
	}
}
