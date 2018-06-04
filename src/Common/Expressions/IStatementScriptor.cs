using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IStatementScriptor
	{
		string Script(IExpression statement);
	}
}
