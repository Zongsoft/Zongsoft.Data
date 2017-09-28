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
	public class MetadataStorageContainer : MetadataContainerBase
	{
		#region 成员字段
		private string _providerName;
		private MetadataStorageEntityCollection _entities;
		#endregion

		#region 构造函数
		public MetadataStorageContainer(string name, string providerName, MetadataFile file) : base(name, file, MetadataElementKind.Storage)
		{
			if(string.IsNullOrWhiteSpace(providerName))
				throw new ArgumentNullException(nameof(providerName));

			_providerName = providerName;
			_entities = new MetadataStorageEntityCollection(this);
		}
		#endregion

		#region 公共属性
		public string ProviderName
		{
			get
			{
				return _providerName;
			}
		}

		public MetadataStorageEntityCollection Entities
		{
			get
			{
				return _entities;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var container = (MetadataStorageContainer)obj;

			return string.Equals(container.Name, this.Name, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(container.ProviderName, _providerName, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode() ^ _providerName.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name + "@" + _providerName;
		}
		#endregion
	}
}
