using System;

namespace Zongsoft.Data.Common.Expressions
{
	public class ConstantExpression : Expression
	{
		#region 常量定义
		public static readonly ConstantExpression Null = new ConstantExpression(null);
		#endregion

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
	}
}
