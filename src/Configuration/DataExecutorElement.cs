using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Configuration
{
	public class DataExecutorElement : OptionConfigurationElement
	{
		#region 公共属性
		[OptionConfigurationProperty("", ElementName = "provider")]
		public DataProviderElementCollection Providers
		{
			get
			{
				return (DataProviderElementCollection)this[""];
			}
		}
		#endregion
	}
}
