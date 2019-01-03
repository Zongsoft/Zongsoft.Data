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
	public class SelectStatementBuilder : IStatementBuilder<DataSelectContext>
	{
		#region 常量定义
		private const string MAIN_INHERIT_PREFIX = "base:";
		#endregion

		#region 构建方法
		public IEnumerable<IStatement> Build(DataSelectContext context)
		{
			var table = new TableIdentifier(context.Entity, "T");
			var statement = this.CreateStatement(context.Entity, table, context.Paging);

			if(context.Grouping != null)
			{
				this.GenerateGrouping(context, statement, context.Grouping);
			}
			else
			{
				var memberPaths = context.ResolveScope();

				foreach(var memberPath in memberPaths)
				{
					this.GenerateFromAndSelect(context, statement, memberPath);
				}
			}

			if(context.Condition != null)
				statement.Where = GenerateCondition(statement, context.Condition);

			if(context.Sortings != null && context.Sortings.Length > 0)
			{
				statement.OrderBy = new OrderByClause();

				foreach(var sorting in context.Sortings)
				{
					var token = EnsureField(statement, sorting.Name);
					statement.OrderBy.Add(token.CreateField(), sorting.Mode);
				}
			}

			return new[] { statement };
		}
		#endregion

		#region 虚拟方法
		protected virtual SelectStatement CreateStatement(IEntityMetadata entity, ISource table, Paging paging)
		{
			return new SelectStatement(entity, table) { Paging = paging };
		}
		#endregion

		#region 私有方法
		private void GenerateGrouping(DataSelectContext context, SelectStatement statement, Grouping grouping)
		{
			PropertyToken token;

			if(grouping.Keys != null && grouping.Keys.Length > 0)
			{
				//创建分组子句
				statement.GroupBy = new GroupByClause();

				foreach(var key in grouping.Keys)
				{
					token = this.EnsureField(statement, key.Name);

					if(token.Property.IsComplex)
						throw new DataException($"The grouping key '{token.Property.Name}' can not be a complex property.");

					token.Statement.GroupBy.Keys.Add(token.CreateField());
					token.Statement.Select.Members.Add(token.CreateField(key.Alias));
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
					token = this.EnsureField(statement, aggregate.Name);

					if(token.Property.IsComplex)
						throw new DataException($"The field '{token.Property.Name}' of aggregate function can not be a complex property.");

					token.Statement.Select.Members.Add(
						new AggregateExpression(aggregate.Method, token.CreateField())
						{
							Alias = string.IsNullOrEmpty(aggregate.Alias) ? aggregate.Name : aggregate.Alias
						});
				}
			}

			if(!string.IsNullOrEmpty(context.Schema))
			{
				var scopes = context.ResolveScope();

				foreach(var scope in scopes)
				{
					this.GenerateFromAndSelect(context, statement, scope);
				}
			}
		}

		private PropertyToken EnsureField(SelectStatement statement, string memberPath)
		{
			var found = statement.Entity.Properties.Find(memberPath, statement.From.FirstOrDefault(), ctx =>
			{
				var source = ctx.Token;

				foreach(var ancestor in ctx.Ancestors)
				{
					//确认当前属性对应的源已经生成
					var slave = this.EnsureSource(statement, ctx.Path, ancestor, ctx.Property, out source);

					//如果返回的从属查询语句不为空，则表示该属性是一对多的导航属性，即需要更新当前操作的查询语句
					if(slave != null)
						statement = slave;
				}

				return source;
			});

			//如果指定的成员路径没有找到对应的属性，则抛出异常
			if(found.IsFailed)
				throw new DataException($"The specified '{memberPath}' member does not exist in the '{statement.Entity}' entity.");

			//返回确认的属性标记
			return new PropertyToken(found.Property, found.Token, statement);
		}

		private SelectStatement EnsureSource(SelectStatement statement, string path, IEntityMetadata parent, IEntityPropertyMetadata property, out ISource source)
		{
			//设置输出参数默认值
			source = null;

			//如果当前属性位于所属实体的父实体中，则先生成父实体的关联
			if(!parent.Equals(property.Entity))
				source = this.EnsureBaseSource(statement, path, parent, property);

			//处理单值属性
			if(property.IsSimplex)
			{
				//获取对应的源
				if(source == null)
					source = this.GetSource(statement, path);

				//单值属性不需要生成对应的数据源
				return null;
			}

			//当前复合属性的完整路径为：路径.属性名
			var fullPath = (string.IsNullOrEmpty(path) ? string.Empty : path + ".") + property.Name;

			//如果当前语句的FROM子句部分已经包含了当前导航属性的关联子句，则不用生成对应的关联子句
			if(statement.From.TryGet(fullPath, out source))
				return null;

			//上面已经将单值属性处理完成并返回，剩下的就是复合属性
			var complex = (IEntityComplexPropertyMetadata)property;

			//如果当前是一对多的导航属性
			if(complex.Multiplicity == AssociationMultiplicity.Many)
			{
				//获取一对多导航属性对应的附属查询语句
				if(statement.Slaves.TryGet(fullPath, out var slave))
				{
					source = slave.From.First();
				}
				else
				{
					//创建附属查询语句对应的主表标识（即为一对多导航属性的关联表）
					source = new TableIdentifier(complex.GetForeignEntity(), "T");

					//创建一个附属查询语句并加入到主查询语句的附属集中（注：附属查询语句的名字必须为导航属性的完整路径）
					slave = statement.CreateSlave(fullPath, complex, source);

					//创建一个与组合的条件表达式
					var conditions = ConditionExpression.And();

					//将约束键入到关联条件中
					if(complex.HasConstraints())
					{
						foreach(var constraint in complex.Constraints)
						{
							conditions.Add(Expression.Equal(source.CreateField(constraint.Name), complex.GetConstraintValue(constraint)));
						}
					}

					if(complex.Links.Length == 1)
					{
						conditions.Add(Expression.In(source.CreateField(complex.Links[0].Name), (IExpression)statement.CreateTemporaryReference(complex.Links[0].Role)));
					}
					else
					{
						var joinTarget = statement.CreateTemporaryReference();
						var join = new JoinClause(null, joinTarget, JoinType.Inner);
						var joinConditions = (ConditionExpression)join.Condition;

						foreach(var link in complex.Links)
						{
							joinConditions.Add(
								Expression.Equal(
									source.CreateField(link.Name),
									joinTarget.CreateField(link.Role)));
						}

						slave.From.Add(join);
					}

					//设置导航属性的关联条件
					if(conditions.Count > 0)
						slave.Where = conditions;

					var foreignProperty = complex.GetForeignProperty();

					if(foreignProperty != null && foreignProperty.IsComplex)
					{
						var foreignComplex = (IEntityComplexPropertyMetadata)foreignProperty;
						this.CreateJoin(slave, path, foreignComplex, source);

						if(foreignComplex.HasConstraints())
						{
							foreach(var constraint in foreignComplex.Constraints)
							{
								conditions.Add(Expression.Equal(source.CreateField(constraint.Name), foreignComplex.GetConstraintValue(constraint)));
							}
						}
					}
				}

				//返回附属查询语句
				return slave;
			}

			source = this.CreateJoin(statement, path, complex, source);

			return null;
		}

		private ISource GetSource(SelectStatement statement, string path)
		{
			if(statement.IsSlave)
			{
				if(string.Equals(path, statement.Slaver.Name, StringComparison.OrdinalIgnoreCase))
				{
					if(statement.Slaver.Umbilical.TryGetForeignMemberPath(out var foreignPath))
						return statement.From.Get(foreignPath);
					else
						return statement.From.First();
				}

				return statement.From.Get(path.Substring(statement.Slaver.Name.Length + 1));
			}
			else
			{
				if(string.IsNullOrEmpty(path))
					return statement.From.First();
				else
					return statement.From.Get(path);
			}
		}

		private ISource CreateJoin(SelectStatement statement, string path, IEntityComplexPropertyMetadata complex, ISource source)
		{
			//当前复合属性的完整路径为：路径.属性名
			var fullPath = (string.IsNullOrEmpty(path) ? string.Empty : path + ".") + complex.Name;

			//如果当前语句的FROM子句部分已经包含了当前导航属性的关联子句，则不用生成对应的关联子句
			if(statement.From.TryGet(fullPath, out var result))
				return result;

			//为当前导航属性创建关联子句的表标识
			var target = statement.CreateTable(complex.GetForeignEntity());

			//生成当前导航属性对应的关联子句（关联名为导航属性的完整路径）
			var joining = new JoinClause(fullPath, target, (complex.Multiplicity == AssociationMultiplicity.One ? JoinType.Inner : JoinType.Left));

			if(source == null)
				source = this.GetSource(statement, path);

			//将关联子句的条件转换为特定的条件表达式
			var conditions = (ConditionExpression)joining.Condition;

			//将约束键入到关联条件中
			if(complex.HasConstraints())
			{
				foreach(var constraint in complex.Constraints)
				{
					conditions.Add(Expression.Equal(source.CreateField(constraint.Name), complex.GetConstraintValue(constraint)));
				}
			}

			foreach(var link in complex.Links)
			{
				conditions.Add(Expression.Equal(target.CreateField(link.Role), source.CreateField(link.Name)));
			}

			//将创建的关联源加入到查询语句的数据源集
			statement.From.Add(joining);

			return joining;
		}

		private ISource EnsureBaseSource(SelectStatement statement, string path, IEntityMetadata parent, IEntityPropertyMetadata property)
		{
			//获取约定的继承关联的名称
			var joiningName = this.GetInheritName(path, property);

			//如果该继承关联已经存在，则返回它即可
			if(statement.From.TryGet(joiningName, out var source))
				return source;

			//获取属性路径（不含属性名）对应的实体以及当前查询语句中的源，注意：因为属性路径部分确保已经生成了对应的源，所以下面的查找必定成功
			var parentSource = string.IsNullOrEmpty(path) ? statement.From.First() : statement.From.Get(this.GetInheritName(path, parent.Name));

			//如果实体属性集中包含当指定的属性则返回对应的源
			if(parent.Properties.Contains(property))
				return parentSource;

			//定义待关联属性所属的实体的表标识
			var target = property.Entity;
			var targetSource = statement.CreateTable(target);

			//创建一个关联子句
			var joining = new JoinClause(joiningName, targetSource);

			//添加关联子句的条件项
			for(int i = 0; i < target.Key.Length; i++)
			{
				((ConditionExpression)joining.Condition).Add(
					Expression.Equal(
						targetSource.CreateField(target.Key[i]),
						parentSource.CreateField(parent.Key[i].GetFieldName(out var alias), alias)));
			}

			//将关联子句加入到查询语句中
			statement.From.Add(joining);

			//返回创建的关联子句
			return joining;
		}

		private void GenerateFromAndSelect(DataSelectContext context, SelectStatement statement, string memberPath)
		{
			//尝试生成指定成员对应的数据源（FROM子句）
			var token = this.EnsureField(statement, memberPath);

			if(token.Property.IsSimplex)
			{
				//将单值属性加入到返回字段集中
				//注意：如果该单值属性隶属于导航属性中（即成员路径文本包含单点符），则必须显式设定其字段别名为指定的成员路径
				//token.Statement.Select.Members.Add(token.CreateField(memberPath.Contains(".") ? memberPath : null));
				token.Statement.Select.Members.Add(token.CreateField());
			}
			else
			{
				var complex = (IEntityComplexPropertyMetadata)token.Property;

				if(complex.TryGetForeignMemberPath(out var foreignPath))
				{
					this.GenerateFromAndSelect(context, token.Statement, foreignPath);
				}
				else
				{
					var path = statement.IsSlave ? statement.Slaver.Name : memberPath;
					var members = context.GetEntityMembers(path);

					//循环遍历导航属性中的所有单值属性（并且必须是返回类型中定义了的）
					foreach(var property in complex.GetForeignEntity().Properties.Where(p => p.IsSimplex && (members == null || members.Contains(p.Name))))
					{
						//将导航属性中的单值属性加入到返回字段集中
						token.Statement.Select.Members.Add(token.CreateField((IEntitySimplexPropertyMetadata)property, path));
					}
				}
			}
		}

		private IExpression GenerateCondition(SelectStatement statement, ICondition condition)
		{
			if(condition == null)
				return null;

			if(condition is Condition c)
				return ConditionExtension.ToExpression(c, field => EnsureField(statement, field).CreateField(), (_, __) => statement.CreateParameter(_, __));

			if(condition is ConditionCollection cc)
				return ConditionExtension.ToExpression(cc, field => EnsureField(statement, field).CreateField(), (_, __) => statement.CreateParameter(_, __));

			throw new NotSupportedException($"The '{condition.GetType().FullName}' type is an unsupported condition type.");
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetInheritName(string prefix, IEntityPropertyMetadata property)
		{
			return (string.IsNullOrEmpty(prefix) ? MAIN_INHERIT_PREFIX : prefix + ":") + property.Entity.Name;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetInheritName(string prefix, string name)
		{
			return (string.IsNullOrEmpty(prefix) ? MAIN_INHERIT_PREFIX : prefix + ":") + name;
		}
		#endregion

		#region 嵌套结构
		private struct PropertyToken
		{
			#region 公共字段
			public SelectStatement Statement;
			public IEntityPropertyMetadata Property;
			public ISource Source;
			#endregion

			#region 构造函数
			public PropertyToken(IEntityPropertyMetadata property, ISource source, SelectStatement statement)
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
			public FieldIdentifier CreateField(IEntitySimplexPropertyMetadata property, string path)
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
