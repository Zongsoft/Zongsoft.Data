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
	/// <summary>
	/// 表示实体映射元素的类。
	/// </summary>
	public class MetadataMapping : MetadataElementBase
	{
		#region 成员字段
		private string _conceptElementName;
		private string _storageElementName;
		#endregion

		#region 构造函数
		protected MetadataMapping(MetadataFile file, string conceptElementName, string storageElementName) : base(MetadataElementKind.Mapping, file)
		{
			if(string.IsNullOrWhiteSpace(conceptElementName))
				throw new ArgumentNullException("conceptElementName");

			if(string.IsNullOrWhiteSpace(storageElementName))
				throw new ArgumentNullException("storageElementName");

			_conceptElementName = conceptElementName.Trim();
			_storageElementName = storageElementName.Trim();
		}
		#endregion

		#region 公共属性
		public string ConceptElementPath
		{
			get
			{
				return _conceptElementName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_conceptElementName = value.Trim();
			}
		}

		public string StorageElementPath
		{
			get
			{
				return _storageElementName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_storageElementName = value.Trim();
			}
		}

		public MetadataFile File
		{
			get
			{
				return (MetadataFile)base.Owner;
			}
		}
		#endregion

		#region 保护方法
		protected MetadataElementBase GetMappedElement(string qualifiedName, Func<string, string, MetadataConceptContainer> getContainer, Func<MetadataConceptContainer, string, MetadataElementBase> getElement)
		{
			var name = DataName.Parse(qualifiedName);
			var @namespace = string.IsNullOrWhiteSpace(name.Namespace) ? this.File.Namespace : name.Namespace;

			var container = getContainer(name.ContainerName, @namespace);

			if(container != null)
				return getElement(container, name.ElementName);

			return null;
		}
		#endregion
	}
}
