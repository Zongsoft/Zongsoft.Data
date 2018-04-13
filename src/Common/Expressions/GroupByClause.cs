using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class GroupByClause
	{
		public ICollection<FieldIdentifier> Members
		{
			get;
		}

		public IExpression Having
		{
			get;
			set;
		}
	}
}
