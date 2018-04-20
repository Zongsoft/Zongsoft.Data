using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体关联成员的元数据类。
	/// </summary>
	public struct AssociationLink
	{
		#region 构造函数
		public AssociationLink(string name, string role)
		{
			this.Name = name;
			this.Role = role;
			this.Value = null;
		}

		public AssociationLink(string name, object value)
		{
			this.Name = name;
			this.Value = value;
			this.Role = null;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关联成员的目标属性名。
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取关联成员的来源属性名。
		/// </summary>
		public string Role
		{
			get;
		}

		/// <summary>
		/// 获取关联成员的目标值。
		/// </summary>
		public object Value
		{
			get;
		}
		#endregion
	}
}
