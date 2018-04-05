using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public struct FieldValue
	{
		#region 构造函数
		public FieldValue(FieldIdentifier field, IExpression value)
		{
			this.Field = field;
			this.Value = value;
		}
		#endregion

		#region 公共属性
		public FieldIdentifier Field
		{
			get;
		}

		public IExpression Value
		{
			get;
		}
		#endregion
	}
}
