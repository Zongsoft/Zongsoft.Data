using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Data.Runtime
{
	public class DataProviderSchemaCollection : Zongsoft.Collections.NamedCollectionBase<DataProviderSchema>
	{
		protected override string GetKeyForItem(DataProviderSchema item)
		{
			return item.Namespace;
		}
	}
}
