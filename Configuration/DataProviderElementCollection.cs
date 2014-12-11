using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Configuration
{
	public class DataProviderElementCollection : OptionConfigurationElementCollection<DataProviderElement>
	{
		public DataProviderElementCollection() : base("provider")
		{
		}

		protected override OptionConfigurationElement CreateNewElement()
		{
			return new DataProviderElement();
		}

		protected override string GetElementKey(OptionConfigurationElement element)
		{
			return ((DataProviderElement)element).Name;
		}
	}
}
