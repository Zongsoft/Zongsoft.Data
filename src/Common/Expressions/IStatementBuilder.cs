using System;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IStatementBuilder
	{
		IStatement Build(DataAccessContextBase context);
	}
}
