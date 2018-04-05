using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class InsertStatement
	{
		public TableIdentifier Table
		{
			get;
		}

		public ICollection<FieldValue> Fields
		{
			get;
		}
	}
}
