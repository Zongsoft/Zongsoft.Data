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
		/// 获取或设置资产设施的位置信息。
		/// </summary>
		public string Location
		{
			get; set;
		}
	}
}
