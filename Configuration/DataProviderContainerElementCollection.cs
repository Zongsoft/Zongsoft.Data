using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Configuration
{
	public class DataProviderContainerElementCollection : OptionConfigurationElementCollection<DataProviderContainerElement>
	{
		public DataProviderContainerElementCollection() : base("container")
		{
		}

		public DataProviderContainerElement this[string name, string @namespace]
		{
			get
			{
				return this[this.GetElementKey(name, @namespace)];
			}
		}

		protected override OptionConfigurationElement CreateNewElement()
		{
			return new DataProviderContainerElement();
		}

		protected override string GetElementKey(OptionConfigurationElement element)
		{
			var containerElement = (DataProviderContainerElement)element;
			return this.GetElementKey(containerElement.Name, containerElement.Namespace);
		}

		private string GetElementKey(string name, string @namespace)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(string.IsNullOrWhiteSpace(@namespace))
				return name.Trim();
			else
				return name.Trim() + "@" + @namespace.Trim();
		}
	}
}
