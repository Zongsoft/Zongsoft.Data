using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Tests.Models
{
	/// <summary>
	/// 表示资产设施的实体基类。
	/// </summary>
	public class AssetBase
	{
		/// <summary>
		/// 获取或设置资产设施编号，主键。
		/// </summary>
		public uint AssetId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置资产设施的名称。
		/// </summary>
		public string Name
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置资产设施的责任人编号。
		/// </summary>
		public uint PrincipalId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置资产设施的责任人对象。
		/// </summary>
		public UserProfile Principal
		{
			get; set;
		}
	}
}
