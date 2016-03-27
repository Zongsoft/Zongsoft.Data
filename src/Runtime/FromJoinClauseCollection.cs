using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Data.Runtime
{
	public class FromJoinClauseCollection : Zongsoft.Collections.NamedCollectionBase<FromJoinClause>
	{
		private object _owner;

		public FromJoinClauseCollection(FromClause from)
		{
			_owner = from;
		}

		public FromJoinClauseCollection(FromJoinClause parent)
		{
			_owner = parent;
		}

		protected override string GetKeyForItem(FromJoinClause item)
		{
			return item.Alias;
		}
	}
}
