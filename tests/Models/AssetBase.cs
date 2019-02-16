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

		public string FullName
		{
			get; set;
		}

		public string Namespace
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

		public uint CreatorId
		{
			get; set;
		}

		public UserProfile Creator
		{
			get; set;
		}

		public DateTime CreatedTime
		{
			get; set;
		}

		public uint ModifierId
		{
			get; set;
		}

		public UserProfile Modifier
		{
			get; set;
		}

		public DateTime? ModifiedTime
		{
			get; set;
		}

		public string Remark
		{
			get; set;
		}
	}
}
