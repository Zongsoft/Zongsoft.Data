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
					return BinaryExpression.Between(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.Like:
					return BinaryExpression.Like(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.In:
					return BinaryExpression.In(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.NotIn:
					return BinaryExpression.NotIn(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.Equal:
					return BinaryExpression.Equal(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.NotEqual:
					return BinaryExpression.NotEqual(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.GreaterThan:
					return BinaryExpression.GreaterThan(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.GreaterThanEqual:
					return BinaryExpression.GreaterThanOrEqual(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.LessThan:
					return BinaryExpression.LessThan(map(condition.Name), ConstantExpression.Create(condition.Value));
				case ConditionOperator.LessThanEqual:
					return BinaryExpression.LessThanOrEqual(map(condition.Name), ConstantExpression.Create(condition.Value));
				default:
					return null;
			}
		}

		public static BinaryExpression ToExpression(this ConditionCollection conditions, Func<string, FieldIdentifier> map)
		{
			if(conditions == null)
				throw new ArgumentNullException(nameof(conditions));

			BinaryExpression result = null;

			foreach(var condition in conditions)
			{
				if(result == null)
				{
					if(condition is Condition c)
						result = ToExpression(c, map);
					else if(condition is ConditionCollection cs)
						result = ToExpression(cs, map);
				}
				else
				{
					if(condition is Condition c)
						result = new BinaryExpression(GetConditionOperator(conditions.ConditionCombination), result, ToExpression(c, map));
					else if(condition is ConditionCollection cs)
						result = new BinaryExpression(GetConditionOperator(conditions.ConditionCombination), result, ToExpression(cs, map));
				}
			}

			return result;
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
