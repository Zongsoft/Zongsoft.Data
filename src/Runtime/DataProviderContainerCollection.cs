using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Data.Runtime
{
	public class DataProviderContainerCollection : Zongsoft.Collections.NamedCollectionBase<DataProviderContainer>
	{
		protected override string GetKeyForItem(DataProviderContainer item)
		{
			return item.Name;
		}
	}
}
