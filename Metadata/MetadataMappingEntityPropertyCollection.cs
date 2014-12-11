using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示实体映射属性元素的集合类。
	/// </summary>
	public class MetadataMappingEntityPropertyCollection : MetadataElementCollectionBase<MetadataMappingEntityProperty>
	{
		public MetadataMappingEntityPropertyCollection(MetadataMappingEntity mapping) : base(mapping)
		{
		}

		protected override string GetKeyForItem(MetadataMappingEntityProperty item)
		{
			return item.Name;
		}
	}
}
