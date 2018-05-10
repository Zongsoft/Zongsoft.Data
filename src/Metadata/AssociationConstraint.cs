using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体关联约束的元数据类。
	/// </summary>
	public struct AssociationConstraint
	{
		#region 构造函数
		public AssociationConstraint(string name, object value)
		{
			this.Name = name;
			this.Value = value;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关联约束的目标成员名。
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取关联约束的目标值。
		/// </summary>
		public object Value
		{
			get;
		}
		#endregion
	}
}
