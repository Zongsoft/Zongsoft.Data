using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Configuration
{
	public class DataProviderContainerElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_NAME_ATTRIBUTE = "name";
		private const string XML_NAMESPACE_ATTRIBUTE = "namespace";
		private const string XML_ENTITIES_COLLECTION = "entities";
		private const string XML_COMMANDS_COLLECTION = "commands";
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

		[OptionConfigurationProperty(XML_NAMESPACE_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsKey)]
		public string Namespace
		{
			get
			{
				return (string)this[XML_NAMESPACE_ATTRIBUTE];
			}
			set
			{
				this[XML_NAMESPACE_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_ENTITIES_COLLECTION, ElementName = "entity")]
		public DataProviderItemElementCollection Entities
		{
			get
			{
				return (DataProviderItemElementCollection)this[XML_ENTITIES_COLLECTION];
			}
		}

		[OptionConfigurationProperty(XML_COMMANDS_COLLECTION, ElementName = "command")]
		public DataProviderItemElementCollection Commands
		{
			get
			{
				return (DataProviderItemElementCollection)this[XML_COMMANDS_COLLECTION];
			}
		}
		#endregion
	}
}
