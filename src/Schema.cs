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
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	public class Schema : ISchema, ISchema<SchemaEntry>
	{
		#region 单例字段
		public static readonly Schema Empty = new Schema();
		#endregion

		#region 成员字段
		private ICollection<SchemaEntry> _entries;
		#endregion

		#region 构造函数
		private Schema()
		{
			_entries = new SchemaEntry[0];
		}

		public Schema(ICollection<SchemaEntry> entries)
		{
			_entries = entries ?? throw new ArgumentNullException(nameof(entries));
		}
		#endregion

		#region 公共属性
		public bool IsEmpty
		{
			get => _entries == null || _entries.Count == 0;
		}

		public ICollection<SchemaEntry> Entries
		{
			get => _entries;
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			if(!_entries.IsReadOnly)
				_entries.Clear();
		}

		public bool Exists(string path)
		{
			if(string.IsNullOrEmpty(path) || this.IsEmpty)
				return false;

			var parts = path.Split('.', '/');
			var entries = _entries;

			for(int i = 0; i < parts.Length; i++)
			{
				if(entries == null)
					return false;

				foreach(var entry in entries)
				{
					if(string.Equals(entry.Name, parts[i], StringComparison.OrdinalIgnoreCase))
					{
						if(i == parts.Length - 1)
							return true;

						entries = entry.Children;
						break;
					}
				}
			}

			return false;
		}

		public ISchema Include(string path)
		{
			if(string.IsNullOrEmpty(path))
				return this;

			return this;
		}

		public ISchema Exclude(string path)
		{
			if(string.IsNullOrEmpty(path))
				return this;

			return this;
		}
		#endregion
	}
}
