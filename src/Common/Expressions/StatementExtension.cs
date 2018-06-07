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

		public static ParameterExpression CreateParameter(this IStatement statement, string path, FieldIdentifier field)
		{
			var parameter = Expression.Parameter("?", path, field);
			statement.Parameters.Add(parameter);
			return parameter;
		}
	}
}
