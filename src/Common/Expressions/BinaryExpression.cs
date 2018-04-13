using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class BinaryExpression : IExpression
	{
		#region 构造函数
		public BinaryExpression(Operator @operator, IExpression left, IExpression right)
		{
			this.Operator = @operator;
			this.Left = left;
			this.Right = right;
		}
		#endregion

		#region 公共属性
		public IExpression Left
		{
			get;
		}

		public Operator Operator
		{
			get;
		}

		public IExpression Right
		{
			get;
		}
		#endregion

		#region 静态方法
		public static BinaryExpression Assign(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Assign, left, right);
		}

		public static BinaryExpression Add(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Plus, left, right);
		}

		public static BinaryExpression Subtract(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Minus, left, right);
		}

		public static BinaryExpression Multiply(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Multiply, left, right);
		}

		public static BinaryExpression Divide(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Divide, left, right);
		}

		public static BinaryExpression Modulo(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Modulo, left, right);
		}

		public static BinaryExpression AndAlso(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.AndAlso, left, right);
		}

		public static BinaryExpression OrElse(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.OrElse, left, right);
		}

		public static BinaryExpression In(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.In, left, right);
		}

		public static BinaryExpression NotIn(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.NotIn, left, right);
		}

		public static BinaryExpression Like(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Like, left, right);
		}

		public static BinaryExpression Between(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Between, left, right);
		}

		public static BinaryExpression Equal(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Equal, left, right);
		}

		public static BinaryExpression NotEqual(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.NotEqual, left, right);
		}

		public static BinaryExpression LessThan(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.LessThan, left, right);
		}

		public static BinaryExpression LessThanOrEqual(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.LessThanOrEqual, left, right);
		}

		public static BinaryExpression GreaterThan(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.GreaterThan, left, right);
		}

		public static BinaryExpression GreaterThanOrEqual(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.GreaterThanOrEqual, left, right);
		}
		#endregion
	}
}
