using System;
using System.IO;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IExpression
	{
		IExpression Accept(IExpressionVisitor visitor);
	}
}
