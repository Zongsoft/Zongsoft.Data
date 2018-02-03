using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class OrderByClause
	{
		public ICollection<OrderByMember> Members
		{
			get;
		}

		public struct OrderByMember
		{
			public FieldIdentifier Field;
			public SortingMode Mode;
		}
	}
}
