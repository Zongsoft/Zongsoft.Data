using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataElementAttributeCollection : Zongsoft.Collections.NamedCollectionBase<MetadataElementAttribute>
	{
		protected override string GetKeyForItem(MetadataElementAttribute item)
		{
			if(string.IsNullOrWhiteSpace(item.NamespaceUri))
				return item.LocalName;
			else
				return item.LocalName + "@" + item.NamespaceUri;
		}

		public MetadataElementAttribute this[string name, string namespaceUri]
		{
			get
			{
				if(string.IsNullOrWhiteSpace(namespaceUri))
					return base[name];

				return base[name + "@" + namespaceUri];
			}
		}
	}
}
