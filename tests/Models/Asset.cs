using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Tests.Models
{
	/// <summary>
	/// 表示资产/设施的实体类。
	/// </summary>
	public class Asset : AssetBase
	{
		/// <summary>
		/// 获取或设置建筑物编号。
		/// </summary>
		public uint BuildingId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置所在楼层。
		/// </summary>
		public short Floor
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置资产设施的位置信息。
		/// </summary>
		public string Location
		{
			get; set;
		}
	}
}
