using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示属性元素的类。
	/// </summary>
	public class MetadataEntityProperty : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		#endregion

		#region 构造函数
		protected MetadataEntityProperty(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置属性/字段的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		/// <summary>
		/// 获取属性/字段所属的实体类型元素。
		/// </summary>
		public MetadataEntity Entity
		{
			get
			{
				return (MetadataEntity)base.Owner;
			}
		}
		#endregion
	}
}
