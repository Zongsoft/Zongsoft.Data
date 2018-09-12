using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Tests.Models
{
	/// <summary>
	/// 表示部门信息的实体类。
	/// </summary>
	public class Department
	{
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
		/// 获取或设置部门经理的用户编号。
		/// </summary>
		public uint ManagerId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置部门经理对象。
		/// </summary>
		public UserProfile Manager
		{
			get; set;
		}
	}
}
