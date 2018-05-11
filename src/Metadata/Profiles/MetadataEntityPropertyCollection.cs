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
		#endregion

		#region 构造函数
		public MetadataEntityPropertyCollection(IEntity entity) : base()
		{
			_entity = entity ?? throw new ArgumentNullException(nameof(entity));
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

		#region 重写方法
		protected override string GetKeyForItem(IEntityProperty item)
		{
			return item.Name;
		}

		protected override void AddItem(IEntityProperty item)
		{
			if(item is MetadataEntityProperty property)
				property.Entity = _entity;

			//调用基类同名方法
			base.AddItem(item);
		}

		protected override void SetItem(string name, IEntityProperty item)
		{
			if(item is MetadataEntityProperty property)
				property.Entity = _entity;

			//调用基类同名方法
			base.SetItem(name, item);
		}
		#endregion
	}
}
