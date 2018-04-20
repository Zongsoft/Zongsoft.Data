using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class AggregateExpression : MethodExpression
	{
		#region 构造函数
		public AggregateExpression(Grouping.AggregateMethod method, IEnumerable<IExpression> arguments) : base(method.ToString(), MethodType.Function, arguments)
		{
			this.Method = method;
		}

		public AggregateExpression(Grouping.AggregateMethod method, params IExpression[] arguments) : base(method.ToString(), MethodType.Function, arguments)
		{
			this.Method = method;
		}
		#endregion

		#region 公共属性
		public Grouping.AggregateMethod Method
		{
			get;
		}
		#endregion
	}
}
