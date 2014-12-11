using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataCommandParameter : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _typeName;
		private MetadataCommandParameterDirection _direction;
		#endregion

		#region 构造函数
		public MetadataCommandParameter(string name, string typeName)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(string.IsNullOrWhiteSpace(typeName))
				throw new ArgumentNullException("typeName");

			_name = name.Trim();
			_typeName = typeName.Trim();
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

		public string TypeName
		{
			get
			{
				return _typeName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_typeName = value.Trim();
			}
		}

		public MetadataCommandParameterDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		public MetadataCommand Command
		{
			get
			{
				return (MetadataCommand)base.Owner;
			}
		}
		#endregion
	}
}
