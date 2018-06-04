using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class DeleteStatement : Expression
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
