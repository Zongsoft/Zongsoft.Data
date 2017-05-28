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
	public class MetadataContainer : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private MetadataCommandCollection _commands;
		private MetadataEntityCollection _entities;
		private MetadataAssociationCollection _associations;
		#endregion

		#region 构造函数
		public MetadataContainer(string name, MetadataFile file, MetadataElementKind kind) : base(kind, file)
		{
			_name = name == null ? string.Empty : name.Trim();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取容器的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取容器所属的映射文件。
		/// </summary>
		public MetadataFile File
		{
			get
			{
				return (MetadataFile)base.Owner;
			}
		}

		public MetadataCommandCollection Commands
		{
			get
			{
				if(_commands == null)
					System.Threading.Interlocked.CompareExchange(ref _commands, new MetadataCommandCollection(this), null);

				return _commands;
			}
		}

		public MetadataEntityCollection Entities
		{
			get
			{
				if(_entities == null)
					System.Threading.Interlocked.CompareExchange(ref _entities, new MetadataEntityCollection(this), null);

				return _entities;
			}
		}

		public MetadataAssociationCollection Associations
		{
			get
			{
				if(_associations == null)
					System.Threading.Interlocked.CompareExchange(ref _associations, new MetadataAssociationCollection(this), null);

				return _associations;
			}
		}
		#endregion
	}
}
