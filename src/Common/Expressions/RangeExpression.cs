using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public struct RangeExpression : IExpression
	{
		#region 公共字段
		public readonly IExpression Minimum;
		public readonly IExpression Maximum;
		#endregion

		#region 构造函数
		public RangeExpression(IExpression minimum, IExpression maximum)
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}
		#endregion

		#region 公共方法
		public IExpression Accept(IExpressionVisitor visitor)
		{
			return visitor.Visit(this);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return $"{Minimum} ~ {Maximum}";
		}
		#endregion
	}
}
