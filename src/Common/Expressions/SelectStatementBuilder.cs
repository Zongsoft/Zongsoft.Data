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

			if(context.Condition != null)
				statement.Where = this.GenerateCondition(statement, context.Condition);

			if(context.Sortings != null && context.Sortings.Length > 0)
			{
				statement.OrderBy = new OrderByClause();

				foreach(var sorting in context.Sortings)
				{
					var source = this.EnsureSource(statement, sorting.Name, out var property);
					statement.OrderBy.Add(source.CreateField(property), sorting.Mode);
				}
			}

			yield return statement;
		}
		#endregion

		#region 私有方法
		private void GenerateSchema(SelectStatement statement, ISource source, SchemaEntry entry)
		{
			SchemaEntry current = entry;

			if(current.Ancestors != null)
			{
				foreach(var ancestor in current.Ancestors)
				{
					source = statement.Join(source, ancestor, current.Path);

					if(!statement.From.Contains(source))
						statement.From.Add(source);
				}
			}

			if(current.Token.Property.IsComplex)
			{
				var complex = (IEntityComplexPropertyMetadata)current.Token.Property;
				source = statement.Join(source, complex, current.FullPath);

				if(!statement.From.Contains(source))
					statement.From.Add(source);
			}

			if(current.Token.Property.IsSimplex)
			{
				var field = source.CreateField(current.Token.Property);

				if(current.Parent != null)
					field.Alias = current.FullPath;

				statement.Select.Members.Add(field);
			}

			if(current.HasChildren)
			{
				foreach(var child in current.Children)
				{
					this.GenerateSchema(statement, source, child);
				}
			}
		}

		private ISource EnsureSource(SelectStatement statement, string memberPath, out IEntityPropertyMetadata property)
		{
			var found = statement.Table.Spread(memberPath, ctx =>
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
				{
					source = statement.Join(source, (IEntityComplexPropertyMetadata)ctx.Property, ctx.FullPath);

					if(!statement.From.Contains(source))
						statement.From.Add(source);
				}

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
				return ConditionExtension.ToExpression(c, field => EnsureSource(statement, field, out var property).CreateField(property), (_, __) => statement.CreateParameter(_, __));

			if(condition is ConditionCollection cc)
				return ConditionExtension.ToExpression(cc, field => EnsureSource(statement, field, out var property).CreateField(property), (_, __) => statement.CreateParameter(_, __));

			throw new NotSupportedException($"The '{condition.GetType().FullName}' type is an unsupported condition type.");
		}
		#endregion
	}
}
