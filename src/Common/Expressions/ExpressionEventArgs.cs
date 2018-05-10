using System;
using System.Text;

namespace Zongsoft.Data.Common.Expressions
{
	public class ExpressionEventArgs : EventArgs
	{
		#region 构造函数
		public ExpressionEventArgs(StringBuilder output, IExpression expression)
		{
			this.Output = output;
			this.Expression = expression;
		}
		#endregion

		#region 公共属性
		public StringBuilder Output
		{
			get;
		}

		public IExpression Expression
		{
			get;
		}
		#endregion
	}
}
