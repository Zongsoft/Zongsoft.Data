using System;

namespace Zongsoft.Data.Common.Expressions
{
	public interface IIdentifier : IExpression
	{
		string Name
		{
			get;
		}
	}
}
