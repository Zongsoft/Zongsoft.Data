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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体属性元数据的集合类。
	/// </summary>
	public class EntityPropertyMetadataCollection : Zongsoft.Collections.NamedCollectionBase<EntityPropertyMetadata>
	{
		#region	成员字段
		private EntityMetadata _entity;
		#endregion

		#region 构造函数
		public EntityPropertyMetadataCollection(EntityMetadata entity) : base()
		{
			_entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}
		#endregion

		#region 公共方法
		public IEntityProperty Find(string path, Action<string, IEntityProperty> matched = null)
		{
			if(string.IsNullOrEmpty(path))
				return null;

			var parts = path.Split('.');

			IEntityProperty property = null;
			IEntityPropertyCollection properties = (IEntityPropertyCollection)this;

			for(int i = 0; i < parts.Length; i++)
			{
				if(properties != null && properties.TryGet(parts[i], out property))
				{
					if(property.IsComplex)
						properties = ((IEntityComplexProperty)property).Relationship.Foreign.Properties;
					else
						properties = null;

					matched?.Invoke(string.Join(".", parts, 0, i + 1), property);
				}
				else
				{
					return null;
				}
			}

			return property;
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(EntityPropertyMetadata item)
		{
			return item.Name;
		}

		protected override void AddItem(EntityPropertyMetadata item)
		{
			if(item != null)
				item.Entity = _entity;

			base.AddItem(item);
		}
		#endregion
	}
}
