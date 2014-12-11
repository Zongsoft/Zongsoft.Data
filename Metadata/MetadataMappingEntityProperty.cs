using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示实体属性映射元素的类。
	/// </summary>
	public class MetadataMappingEntityProperty : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _fieldName;
		#endregion

		#region 构造函数
		public MetadataMappingEntityProperty(string name, string fieldName)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(string.IsNullOrWhiteSpace(fieldName))
				throw new ArgumentNullException("fieldName");

			_name = name.Trim();
			_fieldName = fieldName.Trim();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		public string FieldName
		{
			get
			{
				return _fieldName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_fieldName = value.Trim();
			}
		}

		public MetadataMappingEntity Mapping
		{
			get
			{
				return (MetadataMappingEntity)base.Owner;
			}
		}
		#endregion
	}
}
