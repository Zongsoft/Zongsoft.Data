using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpdateStatement
	{
		public TableIdentifier Table
		{
			get;
		}

		public ICollection<FieldValue> Fields
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
