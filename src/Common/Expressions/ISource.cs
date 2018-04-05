using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public interface ISource
	{
		string Alias
		{
			get;
		}

		FieldIdentifier CreateField(string name, string alias = null);
	}
}
