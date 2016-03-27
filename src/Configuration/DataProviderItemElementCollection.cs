using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Configuration
{
	public class DataProviderItemElementCollection : OptionConfigurationElementCollection<DataProviderItemElement>
	{
		public DataProviderItemElementCollection()
		{
		}

		protected override OptionConfigurationElement CreateNewElement()
		{
			return new DataProviderItemElement();
		}

		protected override string GetElementKey(OptionConfigurationElement element)
		{
			return ((DataProviderItemElement)element).Name;
		}
	}
}
