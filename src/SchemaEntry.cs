/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
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

using Zongsoft.Collections;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public class SchemaEntry : SchemaEntryBase
	{
		#region 成员字段
		private SchemaEntry _parent;
		private INamedCollection<SchemaEntry> _children;
		#endregion

		#region 构造函数
		internal SchemaEntry(EntityPropertyToken token, IEnumerable<IEntityMetadata> ancestors = null)
		{
			this.Token = token;
			this.Ancestors = ancestors;
		}
		#endregion

		#region 公共属性
		public override string Name
		{
			get
			{
				return this.Token.Property.Name;
			}
		}

		public EntityPropertyToken Token
		{
			get;
		}

		public SchemaEntry Parent
		{
			get
			{
				return _parent;
			}
		}

		public IEnumerable<IEntityMetadata> Ancestors
		{
			get;
		}

		public override bool HasChildren
		{
			get
			{
				return _children != null && _children.Count > 0;
			}
		}

		public INamedCollection<SchemaEntry> Children
		{
			get
			{
				return _children;
			}
		}
		#endregion

		#region 重写方法
		protected override SchemaEntryBase GetParent()
		{
			return _parent;
		}

		protected override void SetParent(SchemaEntryBase parent)
		{
			_parent = (parent as SchemaEntry) ?? throw new ArgumentException();
		}

		protected override bool TryGetChild(string name, out SchemaEntryBase child)
		{
			child = null;

			if(_children != null && _children.TryGet(name, out var schema))
			{
				child = schema;
				return true;
			}

			return false;
		}

		protected override void AddChild(SchemaEntryBase child)
		{
			if(!(child is SchemaEntry schema))
				throw new ArgumentNullException();

			if(_children == null)
				System.Threading.Interlocked.CompareExchange(ref _children, new NamedCollection<SchemaEntry>(item => item.Name), null);

			_children.Add(schema);
			schema._parent = this;
		}

		protected override void RemoveChild(string name)
		{
			_children?.Remove(name);
		}

		protected override void ClearChildren()
		{
			_children?.Clear();
		}

		public override string ToString()
		{
			var index = 0;
			var text = this.Name;

			if(this.Paging != null)
			{
				if(Paging.IsDisabled(this.Paging))
					text += ":*";
				else
					text += ":" + (this.Paging.PageIndex == 1 ?
								   this.Paging.PageSize.ToString() :
								   this.Paging.PageIndex.ToString() + "/" + this.Paging.PageSize.ToString());
			}

			if(this.Sortings != null && this.Sortings.Length > 0)
			{
				index = 0;
				text += "(";

				foreach(var sorting in this.Sortings)
				{
					if(index++ > 0)
						text += ", ";

					if(sorting.Mode == SortingMode.Ascending)
						text += sorting.Name;
					else
						text += "~" + sorting.Name;
				}

				text += ")";
			}

			if(_children != null && _children.Count > 0)
			{
				index = 0;
				text += "{";

				foreach(var child in _children)
				{
					if(index++ > 0)
						text += ", ";

					text += child.ToString();
				}

				text += "}";
			}

			return text;
		}
		#endregion
	}
}
