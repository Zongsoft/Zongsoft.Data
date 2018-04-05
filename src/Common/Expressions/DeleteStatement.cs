using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class DeleteStatement
	{
		public TableIdentifier Table
		{
			get;
		}

		public IExpression Where
		{
			get;
			set;
		}
	}
}
