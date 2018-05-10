using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public static class ConditionExtension
	{
		#region 公共方法
		public static BinaryExpression ToExpression(this Condition condition, Func<string, FieldIdentifier> map)
		{
			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			switch(condition.Operator)
			{
				case ConditionOperator.Between:
					return Expression.Between(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.Like:
					return Expression.Like(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.In:
					return Expression.In(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.NotIn:
					return Expression.NotIn(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.Equal:
					return Expression.Equal(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.NotEqual:
					return Expression.NotEqual(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.GreaterThan:
					return Expression.GreaterThan(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.GreaterThanEqual:
					return Expression.GreaterThanOrEqual(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.LessThan:
					return Expression.LessThan(map(condition.Name), Expression.Constant(condition.Value));
				case ConditionOperator.LessThanEqual:
					return Expression.LessThanOrEqual(map(condition.Name), Expression.Constant(condition.Value));
				default:
					return null;
			}
		}

		public static ConditionExpression ToExpression(this ConditionCollection conditions, Func<string, FieldIdentifier> map)
		{
			if(conditions == null)
				throw new ArgumentNullException(nameof(conditions));

			ConditionExpression expressions = new ConditionExpression(conditions.ConditionCombination, conditions.Count);

			foreach(var condition in conditions)
			{
				switch(condition)
				{
					case Condition c:
						expressions.Add(ToExpression(c, map));
						break;
					case ConditionCollection cs:
						expressions.Add(ToExpression(cs, map));
						break;
				}
			}

			return expressions;
		}
		#endregion

		#region 私有方法
		private static Operator GetConditionOperator(ConditionCombination combination)
		{
			switch(combination)
			{
				case ConditionCombination.And:
					return Operator.AndAlso;
				case ConditionCombination.Or:
					return Operator.OrElse;
				default:
					throw new ArgumentException();
			}
		}
		#endregion
	}
}
