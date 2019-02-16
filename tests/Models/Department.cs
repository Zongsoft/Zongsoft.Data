using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Tests.Models
{
	/// <summary>
	/// 表示部门信息的实体类。
	/// </summary>
	public class Department
	{
		#region 常规属性
		/// <summary>
		/// 获取或设置部门所属的企业编号，主键。
		/// </summary>
		public uint CorporationId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置部门编号，主键。
		/// </summary>
		public ushort DepartmentId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置部门名称。
		/// </summary>
		public string Name
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置部门电话。
		/// </summary>
		public string PhoneNumber
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置部门地址编号。
		/// </summary>
		public uint AddressId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置部门的详细地址。
		/// </summary>
		public string AddressDetail
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置部门备注信息。
		/// </summary>
		public string Description
		{
			get; set;
		}
		#endregion

		#region 集合属性
		/// <summary>
		/// 获取或设置部门管理人员集。
		/// </summary>
		public IEnumerable<UserProfile> Managers
		{
			get; set;
		}
		#endregion

		#region 嵌套子类
		public struct DepartmentManager
		{
			public uint CorporationId;
			public ushort DepartmentId;
			public uint UserId;
			public UserProfile User;
			public string Title;
		}
		#endregion
	}
}
