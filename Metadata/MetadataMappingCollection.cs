using System;
using System.Collections.Generic;
using System.Linq;

namespace Zongsoft.Data.Metadata
{
	public class MetadataMappingCollection : MetadataElementCollectionBase<MetadataMapping>
	{
		#region 构造函数
		public MetadataMappingCollection(MetadataFile file) : base(file)
		{
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(MetadataMapping item)
		{
			return item.ConceptElementPath;
		}
		#endregion
	}
}
