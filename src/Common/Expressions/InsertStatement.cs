using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class InsertStatement : Statement
	{
		public TableIdentifier Table
		{
			get;
		}

		public ICollection<FieldIdentifier> Fields
		{
			get;
		}

		public IEnumerable<IExpression> Values
		{
			get;
		}
	}
}
