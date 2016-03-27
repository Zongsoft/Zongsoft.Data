using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataAssociationEndConstraintCollection : MetadataElementCollectionBase<MetadataAssociationEndConstraint>
	{
		public MetadataAssociationEndConstraintCollection(MetadataAssociationEnd owner) : base(owner)
		{
		}

		protected override string GetKeyForItem(MetadataAssociationEndConstraint item)
		{
			return item.PropertyName;
		}
	}
}
