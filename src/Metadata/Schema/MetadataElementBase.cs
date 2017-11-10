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

namespace Zongsoft.Data.Metadata.Schema
{
	public abstract class MetadataElementBase
	{
		#region 成员字段
		private object _owner;
		private MetadataElementKind? _kind;
		private MetadataElementAttributeCollection _attributes;
		#endregion

		#region 构造函数
		protected MetadataElementBase()
		{
		}

		protected MetadataElementBase(MetadataElementKind kind, object owner)
		{
			_kind = kind;
			_owner = owner;
		}
		#endregion

		#region 公共属性
		public MetadataElementKind Kind
		{
			get
			{
				if(_kind == null)
				{
					var parent = _owner as MetadataElementBase;

					if(parent == null)
						_kind = MetadataElementKind.None;
					else
						_kind = parent.Kind;
				}

				return _kind.Value;
			}
		}

		public MetadataElementAttributeCollection Attributes
		{
			get
			{
				if(_attributes == null)
					System.Threading.Interlocked.CompareExchange(ref _attributes, new MetadataElementAttributeCollection(), null);

				return _attributes;
			}
		}
		#endregion

		#region 公共方法
		public string GetAttributeValue(string name)
		{
			if(_kind == MetadataElementKind.Concept)
				return this.GetAttributeValue(name, MetadataUri.Concept);

			if(_kind == MetadataElementKind.Storage)
				return this.GetAttributeValue(name, MetadataUri.Storage);

			return this.GetAttributeValue(name, null);
		}

		public string GetAttributeValue(string name, string namespaceUri)
		{
			var attributes = _attributes;

			if(attributes != null)
			{
				var attribute = attributes[name, namespaceUri];

				if(attribute != null)
					return attribute.Value;
			}

			return null;
		}
		#endregion

		#region 保护属性
		/// <summary>
		/// 获取当前元素的所有者对象。
		/// </summary>
		internal protected object Owner
		{
			get
			{
				return _owner;
			}
			internal set
			{
				_owner = value;
			}
		}
		#endregion
	}
}
