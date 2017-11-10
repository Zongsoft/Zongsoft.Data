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
using System.IO;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata.Schema
{
	public class MetadataFile : MarshalByRefObject
	{
		#region 成员字段
		private string _url;
		private string _namespace;
		private MetadataStorageContainerCollection _storages;
		private MetadataConceptContainerCollection _concepts;
		private MetadataMappingCollection _mappings;
		#endregion

		#region 构造函数
		public MetadataFile() : this(string.Empty)
		{
		}

		public MetadataFile(string url)
		{
			_url = url;
			_storages = new MetadataStorageContainerCollection(this);
			_concepts = new MetadataConceptContainerCollection(this);
			_mappings = new MetadataMappingCollection(this);
		}
		#endregion

		#region 公共属性
		public string Url
		{
			get
			{
				return _url;
			}
		}

		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}

		public MetadataStorageContainerCollection Storages
		{
			get
			{
				return _storages;
			}
		}

		public MetadataConceptContainerCollection Concepts
		{
			get
			{
				return _concepts;
			}
		}

		public MetadataMappingCollection Mappings
		{
			get
			{
				return _mappings;
			}
		}
		#endregion

		#region 静态方法
		public static MetadataFile Load(Stream stream)
		{
			return MetadataResolver.Default.Resolve(stream);
		}

		public static MetadataFile Load(TextReader reader)
		{
			return MetadataResolver.Default.Resolve(reader);
		}

		public static MetadataFile Load(System.Xml.XmlReader reader)
		{
			return MetadataResolver.Default.Resolve(reader);
		}

		public static MetadataFile Load(string filePath)
		{
			return MetadataResolver.Default.Resolve(filePath);
		}
		#endregion
	}
}
