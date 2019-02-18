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
using System.Data;
using System.Data.Common;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public static class StatementExtension
	{
		public static void Bind(this IStatementBase statement, DbCommand command, object data)
		{
			if(!statement.HasParameters)
				return;

			foreach(var parameter in statement.Parameters)
			{
				var dbParameter = command.Parameters[parameter.Name];

				if(dbParameter.Direction == ParameterDirection.Input || dbParameter.Direction == ParameterDirection.InputOutput)
				{
					if(parameter.Schema == null)
						dbParameter.Value = parameter.Value;
					else if(data != null)
					{
						if(data is IEntity entity)
						{
							if(entity.HasChanges(parameter.Schema.Name))
								dbParameter.Value = parameter.Schema.Token.GetValue(data);
							else
								dbParameter.Value = ((IEntitySimplexPropertyMetadata)parameter.Schema.Token.Property).Value;
						}
						else if(data is IDataDictionary dictionary)
						{
							if(dictionary.HasChanges(parameter.Schema.Name))
								dbParameter.Value = dictionary.GetValue(parameter.Schema.Name);
							else
								dbParameter.Value = ((IEntitySimplexPropertyMetadata)parameter.Schema.Token.Property).Value;
						}
						else
						{
							dbParameter.Value = parameter.Schema.Token.GetValue(data);
						}
					}
				}
			}
		}

		public static ISource From(this IStatement statement, string memberPath, out IEntityPropertyMetadata property)
		{
			return From(statement, statement.Table, memberPath, out property);
		}

		public static ISource From(this IStatement statement, TableIdentifier origin, string memberPath, out IEntityPropertyMetadata property)
		{
			var found = origin.Reduce(memberPath, ctx =>
			{
				var source = ctx.Source;

				if(ctx.Ancestors != null)
				{
					foreach(var ancestor in ctx.Ancestors)
					{
						source = statement.Join(source, ancestor, ctx.Path);
					}
				}

				if(ctx.Property.IsComplex)
				{
					var complex = (IEntityComplexPropertyMetadata)ctx.Property;

					if(complex.Multiplicity == AssociationMultiplicity.Many)
						throw new DataException($"The specified '{ctx.FullPath}' member is a one-to-many composite(navigation) property that cannot appear in the sorting and condition clauses.");

					source = statement.Join(source, complex, ctx.FullPath);
				}

				return source;
			});

			if(found.IsFailed)
				throw new DataException($"The specified '{memberPath}' member does not exist in the '{origin.Entity?.Name}' entity and it's inherits.");

			//输出找到的属性元素
			property = found.Property;

			//返回找到的源
			return found.Source;
		}

		public static IExpression Where(this IStatement statment, ICondition criteria)
		{
			if(criteria == null)
				return null;

			if(criteria is Condition c)
				return ConditionExtension.ToExpression(c, field => From(statment, field, out var property).CreateField(property), parameter => statment.Parameters.Add(parameter));

			if(criteria is ConditionCollection cc)
				return ConditionExtension.ToExpression(cc, field => From(statment, field, out var property).CreateField(property), parameter => statment.Parameters.Add(parameter));

			throw new NotSupportedException($"The '{criteria.GetType().FullName}' type is an unsupported condition type.");
		}
	}
}
