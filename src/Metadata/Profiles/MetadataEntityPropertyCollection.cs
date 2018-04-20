/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data.Metadata.Profiles
{
	/// <summary>
	/// 表示数据实体属性元数据的集合类。
	/// </summary>
	public class MetadataEntityPropertyCollection : Zongsoft.Collections.NamedCollectionBase<IEntityProperty>, IEntityPropertyCollection
	{
		#region	成员字段
		private IEntity _entity;
		private IDictionary<string, IEntityProperty> _fields;
		#endregion

		#region 构造函数
		public MetadataEntityPropertyCollection(IEntity entity) : base()
		{
			_entity = entity ?? throw new ArgumentNullException(nameof(entity));
			_fields = new Dictionary<string, IEntityProperty>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public IEntity Entity
		{
			get
			{
				return _entity;
			}
		}
		#endregion

		#region 公共方法
		public IEntityProperty Find(string path, Action<string, IEntityProperty> matched = null)
		{
			if(string.IsNullOrEmpty(path))
				return null;

			var parts = path.Split('.');

			IEntityProperty property = null;
			IEntityPropertyCollection properties = this;

			for(int i = 0; i < parts.Length; i++)
			{
				if(properties == null)
					return null;

				if(properties.TryGet(parts[i], out property))
				{
					if(property.IsComplex)
						properties = ((IEntityComplexProperty)property).GetForeignEntity().Properties;
					else
						properties = null;

					matched?.Invoke(string.Join(".", parts, 0, i + 1), property);
				}
				else
				{
					property = this.FindBaseProperty(ref properties, parts[i]);

					if(property == null)
						return null;

					matched?.Invoke(string.Join(".", parts, 0, i + 1), property);
				}
			}

			return property;
		}

		public IEntityProperty GetProperty(string fieldName)
		{
			if(_fields.TryGetValue(fieldName, out var property))
				return property;

			if(this.TryGet(fieldName, out property))
				return property;

			return null;
		}
		#endregion

		#region 私有方法
		private IEntityProperty FindBaseProperty(ref IEntityPropertyCollection properties, string name)
		{
			if(properties == null)
				return null;

			while(!string.IsNullOrEmpty(properties.Entity.BaseName) &&
			      DataEnvironment.Metadata.Entities.TryGet(properties.Entity.BaseName, out var baseEntity))
			{
				properties = baseEntity.Properties;

				if(properties.TryGet(name, out var property))
					return property;
			}

			return null;
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(IEntityProperty item)
		{
			return item.Name;
		}

		protected override void AddItem(IEntityProperty item)
		{
			if(item is MetadataEntityProperty property)
				property.Entity = _entity;

			//如果指定的属性有别名，则将该属性加入到别名映射字典中
			if(!string.IsNullOrEmpty(item.Alias))
				_fields.Add(item.Alias, item);

			//调用基类同名方法
			base.AddItem(item);
		}

		protected override void ClearItems()
		{
			_fields.Clear();

			//调用基类同名方法
			base.ClearItems();
		}

		protected override bool RemoveItem(string name)
		{
			if(base.TryGetItem(name, out var item))
				_fields.Remove(item.Alias);

			//调用基类同名方法
			return base.RemoveItem(name);
		}

		protected override void SetItem(string name, IEntityProperty item)
		{
			if(item is MetadataEntityProperty property)
				property.Entity = _entity;

			if(item != null && !string.IsNullOrEmpty(item.Alias))
				_fields[item.Alias] = item;

			//调用基类同名方法
			base.SetItem(name, item);
		}
		#endregion
	}
}
