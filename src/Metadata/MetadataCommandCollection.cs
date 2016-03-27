using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示命令元素的集合类。
	/// </summary>
	public class MetadataCommandCollection : MetadataElementCollectionBase<MetadataCommand>
	{
		#region 构造函数
		public MetadataCommandCollection(MetadataContainer container) : base(container)
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
		protected override string GetKeyForItem(MetadataCommand item)
		{
			return item.Name;
		}
		#endregion
	}
}
