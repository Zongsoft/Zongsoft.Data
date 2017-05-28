/*
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

namespace Zongsoft.Data.Metadata
{
	public abstract class MetadataElementCollectionBase<TElement> : Zongsoft.Collections.NamedCollectionBase<TElement> where TElement : MetadataElementBase
	{
		#region 成员字段
		private object _owner;
		#endregion

		#region 构造函数
		protected MetadataElementCollectionBase(object owner)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");

			_owner = owner;
		}
		#endregion

		#region 保护属性
		/// <summary>
		/// 获取当前集合的所有者对象。
		/// </summary>
		protected object Owner
		{
			get
			{
				return _owner;
			}
		}
		#endregion

		#region 重写方法
		protected override void InsertItems(int index, IEnumerable<TElement> items)
		{
			if(items == null)
				throw new ArgumentNullException("items");

			foreach(var item in items)
			{
				if(item.Owner != null && !object.ReferenceEquals(_owner, item.Owner))
					throw new InvalidOperationException("The element is invalid.");

				if(item.Owner == null)
					item.Owner = _owner;
			}

			//调用基类同名方法
			base.InsertItems(index, items);
		}
		#endregion
	}
}
