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
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public static class StatementExtension
	{
		public static IExpression CreateParameter(this IStatement statement, Condition condition, FieldIdentifier field)
		{
			if(condition.Operator == ConditionOperator.Between)
			{
				if(Range.TryGetRange(condition.Value, out var minimum, out var maximum))
				{
					var minParameter = Expression.Parameter("?", minimum, field);
					var maxParameter = Expression.Parameter("?", maximum, field);

					statement.Parameters.Add(minParameter);
					statement.Parameters.Add(maxParameter);

					return new RangeExpression(minParameter, maxParameter);
				}

				return null;
			}

			var parameter = Expression.Parameter("?", condition.Value, field);
			statement.Parameters.Add(parameter);
			return parameter;
		}

		public static ParameterExpression CreateParameter(this IStatement statement, Schema schema, FieldIdentifier field)
		{
			var parameter = Expression.Parameter("?", schema, field);
			statement.Parameters.Add(parameter);
			return parameter;
		}
	}
}
