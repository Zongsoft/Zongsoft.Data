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

		#region 公共方法
		public SelectStatement Build(DataSelectionContext context)
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
				var members = Utility.ResolveScope(context.Scope, entity, context.GetEntityMembers());

				foreach(var member in members)
				{
					this.GenerateFromAndSelect(context, statement, member);
				}
			}

			if(context.Condition != null)
				statement.Where = GenerateCondition(statement, entity, context.Condition);

			if(context.Sortings != null && context.Sortings.Length > 0)
			{
				statement.OrderBy = new OrderByClause();

				foreach(var sorting in context.Sortings)
				{
					var token = GenerateFrom(statement, entity, sorting.Name);
					statement.OrderBy.Add(token.CreateField(), sorting.Mode);
				}
			}

			return statement;
		}
		#endregion

		#region 私有方法
		private void GenerateGrouping(DataSelectionContext context, SelectStatement statement, Grouping grouping)
		{
			PropertyToken token;

			var entity = context.GetEntity();

			//创建分组子句
			statement.GroupBy = new GroupByClause();

			foreach(var key in grouping.Keys)
			{
				token = this.GenerateFrom(statement, entity, key.Name);

				if(token.Property.IsComplex)
					throw new DataException($"The grouping key '{token.Property.Name}' can not be a complex property.");

				token.Statement.GroupBy.Keys.Add(token.CreateField());
				token.Statement.Select.Members.Add(token.CreateField(key.Alias));
			}

			if(grouping.Filter != null)
			{
				statement.GroupBy.Having = GenerateCondition(statement, entity, grouping.Filter);
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
					token = this.GenerateFrom(statement, entity, aggregate.Name);

					if(token.Property.IsComplex)
						throw new DataException($"The field '{token.Property.Name}' of aggregate function can not be a complex property.");

					token.Statement.Select.Members.Add(
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

		private PropertyToken GenerateFrom(SelectStatement statement, IEntity entity, string memberPath)
		{
			ISource source = null;
			SelectStatement slave = null;

			var found = entity.Properties.Find(memberPath, (path, property) =>
			{
				slave = this.EnsureSource(statement, entity, path, property, out source);

				if(slave != null)
					statement = slave;
			});

			//如果指定的成员路径没有找到对应的属性，则抛出异常
			if(found == null)
				throw new DataException($"The specified '{memberPath}' field is not existed.");

			if(source == null && found.IsSimplex)
				source = this.GenerateBaseFrom(statement, entity, found.Entity);

			return new PropertyToken(found, source ?? statement.From.First(), statement);
		}

		private SelectStatement EnsureSource(SelectStatement statement, IEntity entity, string path, IEntityProperty property, out ISource source)
		{
			//设置输出参数默认值
			source = null;

			//处理单值属性
			if(property.IsSimplex)
			{
				//如果路径部分（不含属性名）不为空，则表示当前单值属性隶属导航属性中
				//由于导航属性一定会先处理完成，因此输出的数据源即为导航属性完整名（即当前属性的路径）
				if(path != null && path.Length > 0)
					source = statement.From.Get(path);

				//单值属性不需要生成对应的数据源
				return null;
			}

			//当前复合属性的完整路径为：路径.属性名
			var fullPath = (string.IsNullOrEmpty(path) ? string.Empty : path + ".") + property.Name;

			//如果当前语句的FROM子句部分已经包含了当前导航属性的关联子句，则不用生成对应的关联子句
			if(statement.From.Contains(fullPath))
				return null;

			//上面已经将单值属性处理完成并返回，剩下的就是复合属性
			var complex = (IEntityComplexProperty)property;

			FieldIdentifier GetSelfField(string name)
			{
				ISource self;

				//如果路径部分（不含属性名）为空，则表示当前导航属性隶属查询语句的主表中
				//由于当前导航属性有可能位于主表的父实体中定义，因此需要尝试创建主表对应的父实体
				if(string.IsNullOrEmpty(path))
					self = this.EnsureBaseSource(statement, entity, name);
				else //如果路径部分（不含属性名）不为空，则表示当前导航属性隶属于另一个已经构建好的导航属性中
					self = statement.From.Get(path);

				//从数据源创建关联的引用字段
				return self.CreateField(name);
			}

			var conditions = ConditionExpression.And();

			if(complex.Constraints != null && complex.Constraints.Length > 0)
			{
				foreach(var constraint in complex.Constraints)
				{
					conditions.Add(Expression.Equal(GetSelfField(constraint.Name), complex.GetConstraintValue(constraint)));
				}
			}

			//如果当前是一对多的导航属性
			if(complex.IsMultiple)
			{
				//获取一对多导航属性对应的附属查询语句
				if(statement.Slaves.TryGet(fullPath, out var slave))
				{
					source = slave.From.First();
				}
				else
				{
					//创建附属查询语句对应的主表标识（即为一对多导航属性的关联表）
					source = new TableIdentifier(complex.GetForeignEntity().GetTableName(), "T");

					//创建一个附属查询语句（注：附属查询语句的名字必须为导航属性的完整路径）
					slave = new SelectStatement(fullPath, source);

					//将新建的附属查询语句加入到主查询语句的附属子集中
					statement.Slaves.Add(slave);

					foreach(var link in complex.Links)
					{
						conditions.Add(Expression.In(source.CreateField(link.Name), statement.GetSubquery(link.Role)));
					}

					//设置导航属性的关联条件
					if(conditions.Count > 1)
						slave.Where = conditions;
					else
						slave.Where = conditions.First();
				}

				//返回附属查询语句
				return slave;
			}

			//为当前导航属性创建关联子句的表标识
			var target = statement.CreateTable(property.Entity);

			//生成当前导航属性对应的关联子句（关联名为导航属性的完整路径）
			source = new JoinClause(fullPath, target, JoinType.Left);

			foreach(var link in complex.Links)
			{
				conditions.Add(Expression.Equal(target.CreateField(link.Role), GetSelfField(link.Name)));
			}

			//设置导航属性的关联条件
			if(conditions.Count > 1)
				((JoinClause)source).Condition = conditions;
			else
				((JoinClause)source).Condition = conditions.First();

			//将创建的关联源加入到查询语句的数据源集
			statement.From.Add(source);

			return null;
		}

		private ISource EnsureBaseSource(SelectStatement statement, IEntity entity, string field)
		{
			if(entity.Properties.Contains(field))
				return statement.From.First();

			IEntity baseEntity;

			while((baseEntity = entity.GetBaseEntity()) != null)
			{
				if(baseEntity.Properties.Contains(field))
				{
					if(statement.From.TryGet(JOINCLAUSE_INHERIT_PREFIX + baseEntity.Name, out var source))
						return source;

					return this.GenerateBaseFrom(statement, entity, baseEntity);
				}

				entity = baseEntity;
			}

			throw new DataException($"The specified '{field}' field is not existed.");
		}

		private ISource GenerateBaseFrom(SelectStatement statement, IEntity deriveEntity, IEntity baseEntity)
		{
			//if(deriveEntity.Equals(baseEntity))
			//	return statement.From.First();

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
					Expression.Equal(baseTable.CreateField(baseEntity.Key[i]),
						statement.From.First().CreateField(deriveEntity.Key[i].Name)));
			}

			statement.From.Add(source);

			return source;
		}

		private void GenerateFromAndSelect(DataSelectionContext context, SelectStatement statement, string memberPath, IEntity entity = null)
		{
			//尝试生成指定成员对应的数据源（FROM子句）
			var token = this.GenerateFrom(statement, entity ?? context.GetEntity(), memberPath);

			if(token.Property.IsSimplex)
			{
				//将单值属性加入到返回字段集中
				//注意：如果该单值属性隶属于导航属性中（即成员路径文本包含单点符），则必须显式设定其字段别名为指定的成员路径
				statement.Select.Members.Add(token.CreateField(memberPath.Contains(".") ? memberPath : null));
			}
			else
			{
				var complex = (IEntityComplexProperty)token.Property;

				if(complex.TryGetForeignMemberPath(out var foreignPath))
				{
					this.GenerateFromAndSelect(context, token.Statement, foreignPath, complex.GetForeignEntity());
				}
				else
				{
					var members = context.GetEntityMembers(memberPath);

					//循环遍历导航属性中的所有单值属性（并且必须是返回类型中定义了的）
					foreach(var property in complex.GetForeignEntity().Properties.Where(p => p.IsSimplex && (members == null || members.Contains(p.Name))))
					{
						//将导航属性中的单值属性加入到返回字段集中
						statement.Select.Members.Add(token.CreateField((IEntitySimplexProperty)property, memberPath));
					}
				}
			}
		}

		private IExpression GenerateCondition(SelectStatement statement, IEntity entity, ICondition condition)
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
		#endregion

		#region 嵌套结构
		private struct PropertyToken
		{
			#region 公共字段
			public SelectStatement Statement;
			public IEntityProperty Property;
			public ISource Source;
			#endregion

			#region 构造函数
			public PropertyToken(IEntityProperty property, ISource source, SelectStatement statement)
			{
				this.Property = property ?? throw new ArgumentNullException(nameof(property));
				this.Source = source ?? throw new ArgumentNullException(nameof(source));
				this.Statement = statement ?? throw new ArgumentNullException(nameof(statement));
			}
			#endregion

			#region 公共方法
			/// <summary>
			/// 创建当前属性所属数据源的字段标识。
			/// </summary>
			/// <param name="alias">指定创建字段的别名，如果为空则采用当前属性定义的别名。</param>
			/// <returns>返回创建的字段标识。</returns>
			public FieldIdentifier CreateField(string alias = null)
			{
				return this.Source.CreateField(
					this.Property.GetFieldName(out var defaultAlias), //获取当前属性定义的字段名并根据其定义输出对应的别名
					string.IsNullOrEmpty(alias) ? defaultAlias : alias); //如果该方法的别名参数为空，则使用当前属性定义的默认别名
			}

			/// <summary>
			/// 创建当前导航属性关联的指定外键单值属性的字段标识。
			/// </summary>
			/// <param name="property">指定的外键单值属性（必须是当前属性关联的外键单值属性）。</param>
			/// <param name="path">指定的当前导航属性的路径（不能为空或空字符串）。</param>
			/// <returns>返回创建的字段标识。</returns>
			public FieldIdentifier CreateField(IEntitySimplexProperty property, string path)
			{
				if(string.IsNullOrEmpty(path))
					throw new ArgumentNullException(nameof(path));

				return this.Source.CreateField(property.GetFieldName(out _), path + "." + property.Name);
			}
			#endregion
		}
		#endregion
	}
}
