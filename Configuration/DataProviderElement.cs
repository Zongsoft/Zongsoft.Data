using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Configuration
{
	public class DataProviderElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_NAME_ATTRIBUTE = "name";
		private const string XML_DRIVERNAME_ATTRIBUTE = "driverName";
		private const string XML_ACCESSMODE_ATTRIBUTE = "accessMode";
		private const string XML_CONNECTIONSTRING_ATTRIBUTE = "connectionString";
		#endregion

		#region 公共属性
		[OptionConfigurationProperty(XML_NAME_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsKey)]
		public string Name
		{
			get
			{
				return (string)this[XML_NAME_ATTRIBUTE];
			}
			set
			{
				this[XML_NAME_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_DRIVERNAME_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsRequired)]
		public string DriverName
		{
			get
			{
				return (string)this[XML_DRIVERNAME_ATTRIBUTE];
			}
			set
			{
				this[XML_DRIVERNAME_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_ACCESSMODE_ATTRIBUTE, DefaultValue = DataAccessMode.ReadWrite)]
		public DataAccessMode AccessMode
		{
			get
			{
				return (DataAccessMode)this[XML_ACCESSMODE_ATTRIBUTE];
			}
			set
			{
				this[XML_ACCESSMODE_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_CONNECTIONSTRING_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsRequired)]
		public string ConnectionString
		{
			get
			{
				return (string)this[XML_CONNECTIONSTRING_ATTRIBUTE];
			}
			set
			{
				this[XML_CONNECTIONSTRING_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty("", ElementName = "container")]
		public DataProviderContainerElementCollection Containers
		{
			get
			{
				return (DataProviderContainerElementCollection)this[""];
			}
		}
		#endregion
	}
}
