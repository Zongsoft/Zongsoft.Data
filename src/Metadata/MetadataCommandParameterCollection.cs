using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataCommandParameterCollection : MetadataElementCollectionBase<MetadataCommandParameter>
	{
		#region 构造函数
		public MetadataCommandParameterCollection(MetadataCommand command) : base(command)
		{
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(MetadataCommandParameter item)
		{
			return item.Name;
		}
		#endregion
	}
}
