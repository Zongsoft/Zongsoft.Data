using System;
using System.Data;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IStatementScriptor
	{
		IDataProvider Provider
		{
			get;
		}

		Script Script(IStatement statement);
	}
}
