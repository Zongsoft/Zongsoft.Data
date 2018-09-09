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
			throw new NotImplementedException();
		}
		#endregion

		#region 私有方法
		private IEnumerable<JoinClause> Join(DeleteStatement statement, TableIdentifier table, string fullPath = null)
		{
			if(table == null || table.Entity == null)
				yield break;

			var super = table.Entity.GetBaseEntity();

			while(super != null)
			{
				var join = statement.CreateJoin(table, fullPath);
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

			var join = statement.CreateJoin(table, schema);
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
				foreach(var ancestor in ctx.Ancestors)
				{
					
					//确认当前属性对应的源已经生成
					this.EnsureSource(statement, ctx.Path, ancestor, ctx.Property, out source);
				}
			});

			return source;
		}

		private void EnsureSource(DeleteStatement statement, string path, IEntityMetadata parent, IEntityPropertyMetadata property, out ISource source)
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

				return;
			}

			//当前复合属性的完整路径为：路径.属性名
			var fullPath = (string.IsNullOrEmpty(path) ? string.Empty : path + ".") + property.Name;

			//如果当前语句的FROM子句部分已经包含了当前导航属性的关联子句，则不用生成对应的关联子句
			if(statement.From.TryGet(fullPath, out source))
				return;

			//上面已经将单值属性处理完成并返回，剩下的就是复合属性
			var complex = (IEntityComplexPropertyMetadata)property;

			//不管是一对多还是一对一的导航属性，都创建对应的JOIN关联
			source = this.CreateJoin(statement, path, complex, source);
		}

		private ISource GetSource(DeleteStatement statement, string path)
		{
			if(string.IsNullOrEmpty(path))
				return statement.From.First();
			else
				return statement.From.Get(path);
		}

		private ISource CreateJoin(DeleteStatement statement, string path, IEntityComplexPropertyMetadata complex, ISource source)
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

		private ISource EnsureBaseSource(DeleteStatement statement, string path, IEntityMetadata parent, IEntityPropertyMetadata property)
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
	}
}
