using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class BinaryExpression : Expression
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
	}
}
