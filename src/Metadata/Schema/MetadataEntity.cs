﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata.Schema
{
	/// <summary>
	/// 表示实体类型元素的类。
	/// </summary>
	public class MetadataEntity : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private MetadataEntity _baseEntity;
		private string _baseEntityName;
		private HashSet<string> _keyMembers;
		private MetadataEntityProperty[] _key;
		private MetadataEntityPropertyCollection _properties;
		#endregion

		#region 构造函数
		protected MetadataEntity(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置实体元素的名称。
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
		/// 获取实体元素的限定名，即为“容器名.元素名”。
		/// </summary>
		public string QualifiedName
		{
			get
			{
				var container = this.Owner as MetadataContainerBase;

				if(container == null || string.IsNullOrEmpty(container.Name))
					return _name;

				return container.Name + "." + _name;
			}
		}

		/// <summary>
		/// 获取继承的实体元素对象，如果没有父类则返回空。
		/// </summary>
		public MetadataEntity BaseEntity
		{
			get
			{
				if(_baseEntity == null)
				{
					if(string.IsNullOrEmpty(_baseEntityName))
						return null;

					var qualifiedName = _baseEntityName;

					if(!qualifiedName.Contains("."))
						qualifiedName = ((MetadataContainerBase)this.Owner).Name + "." + _baseEntityName;

					switch(this.Kind)
					{
						case MetadataElementKind.Concept:
							_baseEntity = MetadataManager.Default.GetConceptElement<MetadataEntity>(qualifiedName);
							break;
						case MetadataElementKind.Storage:
							_baseEntity = MetadataManager.Default.GetStorageElement<MetadataEntity>(qualifiedName);
							break;
					}
				}

				return _baseEntity;
			}
		}

		/// <summary>
		/// 获取或设置继承的实体元素名，如果为空或空字符串("")则表示没有继承。
		/// </summary>
		public string BaseEntityName
		{
			get
			{
				return _baseEntityName;
			}
			set
			{
				_baseEntityName = value == null ? string.Empty : value.Trim();
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
				throw new ArgumentNullException(nameof(name));

			if(_keyMembers == null)
				System.Threading.Interlocked.CompareExchange(ref _keyMembers, new HashSet<string>(StringComparer.OrdinalIgnoreCase), null);

			if(_keyMembers.Add(name.Trim()))
				_key = null;
		}
		#endregion
	}
}
