using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class Script
	{
		#region 只读变量
		private static readonly Collections.INamedCollection<ParameterExpression> Empty = new Collections.NamedCollection<ParameterExpression>(p => p.Name);
		#endregion

		#region 构造函数
		public Script(string text, Collections.INamedCollection<ParameterExpression> parameters = null)
		{
			this.Text = text;
			this.Parameters = parameters ?? Empty;
		}
		#endregion

		#region 公共属性
		public string Text
		{
			get;
			set;
		}

		public Collections.INamedCollection<ParameterExpression> Parameters
		{
			get;
		}
		#endregion
	}
}
