using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class AggregationExpression : CallExpression
	{
		#region 构造函数
		public AggregationExpression(Grouping.GroupAggregationMethod method, IEnumerable<IExpression> arguments) : base(method.ToString(), CallType.Function, arguments)
		{
			this.Method = method;
		}

		public AggregationExpression(Grouping.GroupAggregationMethod method, params IExpression[] arguments) : base(method.ToString(), CallType.Function, arguments)
		{
			this.Method = method;
		}
		#endregion

		#region 公共属性
		public Grouping.GroupAggregationMethod Method
		{
			get;
		}

		public string Alias
		{
			get;
			set;
		}
		#endregion
	}
}
