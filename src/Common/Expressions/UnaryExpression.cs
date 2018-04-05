using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class UnaryExpression : IExpression
	{
		#region 构造函数
		public UnaryExpression(Operator @operator, IExpression operand)
		{
			this.Operator = @operator;
			this.Operand = operand;
		}
		#endregion

		#region 公共属性
		public IExpression Operand
		{
			get;
		}

		public Operator Operator
		{
			get;
		}
		#endregion

		#region 静态方法
		public static UnaryExpression Negate(IExpression operand)
		{
			return new UnaryExpression(Operator.Minus, operand);
		}

		public static UnaryExpression Not(IExpression operand)
		{
			return new UnaryExpression(Operator.Not, operand);
		}
		#endregion
	}
}
