using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataContainerCollection : MetadataElementCollectionBase<MetadataContainer>
	{
		#region 构造函数
		public MetadataContainerCollection(MetadataFile file) : base(file)
		{
		}
		#endregion

		#region 公共属性
		public MetadataFile File
		{
			get
			{
				return (MetadataFile)base.Owner;
			}
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(MetadataContainer item)
		{
			return item.Name;
		}
		#endregion
	}
}
