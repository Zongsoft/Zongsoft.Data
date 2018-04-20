using System;

namespace Zongsoft.Data.Common.Expressions
{
	public class ConstantExpression : IExpression
	{
		#region 构造函数
		public ConstantExpression(object value, Type valueType = null)
		{
			this.Value = value;
			this.ValueType = valueType ?? (value == null ? typeof(object) : value.GetType());
		}
		#endregion

		#region 公共属性
		public Type ValueType
		{
			get;
		}

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
