using System;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IStatementBuilder
	{
		IExpression Build(DataAccessContextBase context);
	}
}
