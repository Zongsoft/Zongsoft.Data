using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IExpressionVisitor
	{
		IExpression Visit(IExpression expression);
	}
}
