using System;

namespace Zongsoft.Data.Common.Expressions
{
	public class Constant<T> : IExpression
	{
		public T Value
		{
			get;
			set;
		}
	}
}
