using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示属性元素的集合类。
	/// </summary>
	public class MetadataEntityPropertyCollection : MetadataElementCollectionBase<MetadataEntityProperty>
	{
		#region 构造函数
		public MetadataEntityPropertyCollection(MetadataEntity entity) : base(entity)
		{
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(MetadataEntityProperty item)
		{
			return item.Name;
		}
		#endregion
	}
}
