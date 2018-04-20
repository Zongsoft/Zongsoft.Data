using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class GroupByClause
	{
		public GroupByClause()
		{
			this.Keys = new List<FieldIdentifier>();
		}

		public ICollection<FieldIdentifier> Keys
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
