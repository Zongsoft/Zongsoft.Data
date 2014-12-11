using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataAssociationEndCollection : MetadataElementCollectionBase<MetadataAssociationEnd>
	{
		public MetadataAssociationEndCollection(MetadataAssociation owner) : base(owner)
		{
		}

		protected override string GetKeyForItem(MetadataAssociationEnd item)
		{
			return item.Name;
		}
	}
}
