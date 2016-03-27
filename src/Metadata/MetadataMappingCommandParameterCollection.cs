using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataMappingCommandParameterCollection : MetadataElementCollectionBase<MetadataMappingCommandParameter>
	{
		public MetadataMappingCommandParameterCollection(MetadataMappingCommand mapping) : base(mapping)
		{
		}

		protected override string GetKeyForItem(MetadataMappingCommandParameter item)
		{
			return item.Name;
		}
	}
}
