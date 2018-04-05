using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class JoinClauseCollection : Collections.NamedCollectionBase<JoinClause>
	{
		protected override string GetKeyForItem(JoinClause item)
		{
			return item.Name;
		}
	}
}
