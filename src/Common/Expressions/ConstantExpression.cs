using System;

namespace Zongsoft.Data.Common.Expressions
{
	public class ConstantExpression : IExpression
	{
		#region 构造函数
		public ConstantExpression(object value)
		{
			this.Value = value;
		}
		#endregion

		#region 公共属性
		public object Value
		{
			get;
		}
		#endregion

		#region 静态方法
		public static ConstantExpression Create(object value)
		{
			return new ConstantExpression(value);
		}
		#endregion
	}
}
