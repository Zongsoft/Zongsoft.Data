using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示实体类型元素的类。
	/// </summary>
	public class MetadataEntity : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _baseEntityName;
		private HashSet<string> _keyMembers;
		private MetadataEntityProperty[] _key;
		private MetadataEntityPropertyCollection _properties;
		#endregion

		#region 构造函数
		public MetadataEntity(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置实体类型的名称。
		/// </summary>
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

		/// <summary>
		/// 获取实体类型的全称，即为“容器名.元素名”。
		/// </summary>
		public string FullName
		{
			get
			{
				var container = this.Container;

				if(container == null || string.IsNullOrWhiteSpace(container.Name))
					return _name;

				return container.Name + "." + _name;
			}
		}

		/// <summary>
		/// 获取类型的完全限定名，即为“容器名.元素名@命名空间”。
		/// </summary>
		public string QualifiedName
		{
			get
			{
				return this.FullName + "@" + this.Container.File.Namespace;
			}
		}

		/// <summary>
		/// 获取继承的实体元素对象，如果没有父类则返回空。
		/// </summary>
		public MetadataEntity BaseEntity
		{
			get
			{
				var baseQualifiedName = _baseEntityName;

				if(string.IsNullOrWhiteSpace(baseQualifiedName))
					return null;

				var container = this.Container;

				if(container == null)
					return null;

				if(!baseQualifiedName.Contains("@"))
					baseQualifiedName += "@" + container.File.Namespace;

				switch(container.Kind)
				{
					case MetadataElementKind.Concept:
						return MetadataManager.Default.GetConceptElement<MetadataEntity>(baseQualifiedName);
					case MetadataElementKind.Storage:
						return MetadataManager.Default.GetStorageElement<MetadataEntity>(baseQualifiedName);
					default:
						return null;
				}
			}
		}

		/// <summary>
		/// 获取或设置继承的实体元素的限定名，如果为空或空字符串("")则表示没有继承。
		/// </summary>
		public string BaseEntityName
		{
			get
			{
				return _baseEntityName;
			}
			set
			{
				_baseEntityName = value;
			}
		}
 
		/// <summary>
		/// 获取实体类型的主键集。
		/// </summary>
		public MetadataEntityProperty[] Key
		{
			get
			{
				if(_key == null)
				{
					var keyMembers = _keyMembers;
					var properties = _properties;

					if(keyMembers != null && properties != null)
					{
						var key = new MetadataEntityProperty[keyMembers.Count];
						int index = 0;

						foreach(var keyRef in keyMembers)
						{
							if(properties[keyRef] == null)
								throw new MetadataException(string.Format("The '{0}' key is not exists in this '{1}' entity.", keyRef, this.QualifiedName));

							key[index++] = properties[keyRef];
						}

						System.Threading.Interlocked.CompareExchange(ref _key, key, null);
					}
				}

				return _key;
			}
		}

		/// <summary>
		/// 获取实体类型的属性元素集。
		/// </summary>
		public MetadataEntityPropertyCollection Properties
		{
			get
			{
				if(_properties == null)
					System.Threading.Interlocked.CompareExchange(ref _properties, new MetadataEntityPropertyCollection(this), null);

				return _properties;
			}
		}

		/// <summary>
		/// 获取实体类型元素所属的容器元素。
		/// </summary>
		public MetadataContainer Container
		{
			get
			{
				return (MetadataContainer)base.Owner;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取指定名称的属性元素对象。
		/// </summary>
		/// <param name="name">指定的属性名称，支持以点(.)为分隔符的属性名路径。</param>
		/// <returns>返回找到的属性元素，如果没有找到指定路径的属性元素则返回空(null)。</returns>
		public MetadataEntityProperty GetProperty(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return null;

			var parts = name.Split('.');
			var entity = this;

			MetadataEntityProperty property = null;

			foreach(var part in parts)
			{
				while(entity != null)
				{
					property = entity.Properties[part];

					if(property == null)
						entity = entity.BaseEntity;
					else
					{
						var complexProperty = property as MetadataEntityComplexProperty;

						if(complexProperty != null)
							entity = complexProperty.Relationship.GetToEntity();

						break;
					}
				}

				if(property == null)
					return null;
			}

			return property;
		}
		#endregion

		#region 内部方法
		/// <summary>
		/// 设置当前实体类型的主键名。
		/// </summary>
		/// <param name="name">指定的主键名称。</param>
		internal void SetKey(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(_keyMembers == null)
				System.Threading.Interlocked.CompareExchange(ref _keyMembers, new HashSet<string>(StringComparer.OrdinalIgnoreCase), null);

			if(_keyMembers.Add(name.Trim()))
				_key = null;
		}
		#endregion
	}
}
