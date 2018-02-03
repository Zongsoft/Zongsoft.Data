using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class SelectClause
	{
		public bool IsDistinct
		{
			get;
			set;
		}

		public ICollection<FieldIdentifier> Fields
		{
			get;
		}
	}
}
