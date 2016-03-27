using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataAssociationCollection : MetadataElementCollectionBase<MetadataAssociation>
	{
		#region 构造函数
		public MetadataAssociationCollection(MetadataContainer container) : base(container)
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
		protected override string GetKeyForItem(MetadataAssociation item)
		{
			return item.Name;
		}
		#endregion
	}
}
