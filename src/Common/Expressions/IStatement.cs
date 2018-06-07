using System;
using System.IO;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IStatement : IExpression
	{
		bool HasParameters
		{
			get;
		}

		Collections.INamedCollection<ParameterExpression> Parameters
		{
			get;
		}
	}
}
