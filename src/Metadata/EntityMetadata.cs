using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class EntityMetadata
	{
		#region 成员字段
		private string _name;
		private string _alias;
		private EntityMetadata _baseEntity;
		private EntityPropertyMetadata[] _key;
		private EntityPropertyMetadataCollection _properties;
		#endregion

		#region 构造函数
		public EntityMetadata(string name, EntityMetadata baseEntity = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_baseEntity = baseEntity;
			_properties = new EntityPropertyMetadataCollection(this);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
			}
		}

		public EntityPropertyMetadata[] Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		public EntityMetadata BaseEntity
		{
			get
			{
				return _baseEntity;
			}
			set
			{
				_baseEntity = value;
			}
		}

		public EntityPropertyMetadataCollection Properties
		{
			get
			{
				return _properties;
			}
		}
		#endregion
	}
}
