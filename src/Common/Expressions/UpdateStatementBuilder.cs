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
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpdateStatementBuilder : IStatementBuilder<DataUpdateContext>
	{
		#region 常量定义
		private const string TEMPORARY_ALIAS = "tmp";
		#endregion

		#region 构建方法
		public IEnumerable<IStatement> Build(DataUpdateContext context)
		{
			if(context.Source.Features.Support(Feature.Updation.Multitable))
				return this.BuildSimplicity(context);
			else
				return this.BuildComplexity(context);
		}
		#endregion

		#region 虚拟方法
		/// <summary>
		/// 构建支持多表更新的语句。
		/// </summary>
		/// <param name="context">构建操作需要的数据访问上下文对象。</param>
		/// <returns>返回多表更新的语句。</returns>
		protected virtual IEnumerable<IStatement> BuildSimplicity(DataUpdateContext context)
		{
			var statement = new UpdateStatement(context.Entity);

			//获取要更新的数据模式（模式不为空）
			if(!context.Schema.IsEmpty)
			{
				//依次生成各个数据成员的关联（包括它的继承链、子元素集）
				foreach(var member in context.Schema.Members)
				{
					if(member.Token.Property.IsSimplex)
					{
						var field = statement.Table.CreateField(member.Token);
						var parameter = Expression.Parameter(ParameterExpression.Anonymous, member, field);

						statement.Fields.Add(new FieldValue(field, parameter));
						statement.Parameters.Add(parameter);
					}
					else
					{
						this.Join(statement, statement.Table, member);
					}
				}
			}

			if(context.Condition != null)
				statement.Where = GenerateCondition(statement, context.Condition);

			yield return statement;
		}

		/// <summary>
		/// 构建单表更新的语句，因为不支持多表更新所以单表更新操作由多条语句以主从树形结构来表达需要进行的多批次的更新操作。
		/// </summary>
		/// <param name="context">构建操作需要的数据访问上下文对象。</param>
		/// <returns>返回的单表更新的多条语句的主句。</returns>
		protected virtual IEnumerable<IStatement> BuildComplexity(DataUpdateContext context)
		{
			var statement = new UpdateStatement(context.Entity);

			if(context.Condition != null)
				statement.Where = GenerateCondition(statement, context.Condition);

			return null;
		}
		#endregion

		#region 私有方法
		private void Join(UpdateStatement statement, ISource source, SchemaMember schema)
		{
			if(source == null || schema == null || schema.Token.Property.IsSimplex)
				return;

			if(((IEntityComplexPropertyMetadata)schema.Token.Property).Multiplicity == AssociationMultiplicity.Many)
				return;

			//第一步：处理模式成员所在的继承实体的关联
			if(schema.Ancestors != null)
			{
				foreach(var ancestor in schema.Ancestors)
				{
					source = statement.Join(source, ancestor, schema.FullPath);
				}
			}

			//第二步：处理模式成员（导航属性）的关联
			var join = statement.Join(source, schema);
			var target = (TableIdentifier)join.Target;
			statement.Tables.Add(target);

			//第三步：处理模式成员的子成员集的关联
			//if(schema.HasChildren)
			//{
			//	foreach(var child in schema.Children)
			//	{
			//		this.Join(statement, source, child);
			//	}
			//}
		}

		private ISource EnsureSource(UpdateStatement statement, string memberPath, out IEntityPropertyMetadata property)
		{
			var found = statement.Table.Reduce(memberPath, ctx =>
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
				throw new DataException($"The specified '{memberPath}' member does not exist in the '{statement.Entity.Name}' entity.");

			//输出找到的属性元素
			property = found.Property;

			//返回找到的源
			return found.Source;
		}

		private IExpression GenerateCondition(UpdateStatement statement, ICondition condition)
		{
			if(condition == null)
				return null;

			if(condition is Condition c)
				return ConditionExtension.ToExpression(c, field => EnsureSource(statement, field, out var property).CreateField(property), parameter => statement.Parameters.Add(parameter));

			if(condition is ConditionCollection cc)
				return ConditionExtension.ToExpression(cc, field => EnsureSource(statement, field, out var property).CreateField(property), parameter => statement.Parameters.Add(parameter));

			throw new NotSupportedException($"The '{condition.GetType().FullName}' type is an unsupported condition type.");
		}
		#endregion
	}
}
