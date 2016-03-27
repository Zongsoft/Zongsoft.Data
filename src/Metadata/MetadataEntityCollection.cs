using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示实体类型元素的集合类。
	/// </summary>
	public class MetadataEntityCollection : MetadataElementCollectionBase<MetadataEntity>
	{
		#region 构造函数
		public MetadataEntityCollection(MetadataContainer container) : base(container)
		{
		}
		#endregion

		#region 公共属性
		public MetadataContainer Container
		{
			get
			{
				return (MetadataContainer)base.Owner;
			}
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(MetadataEntity item)
		{
			return item.Name;
		}
		#endregion
	}
}
