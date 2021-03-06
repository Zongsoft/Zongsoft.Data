﻿/*
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
	public class MetadataElementAttribute
	{
		#region 成员字段
		private string _localName;
		private string _prefix;
		private string _name;
		private string _namespaceUri;
		private string _value;
		#endregion

		#region 构造函数
		public MetadataElementAttribute(string prefix, string localName, string value, string namespaceUri)
		{
			if(string.IsNullOrWhiteSpace(localName))
				throw new ArgumentNullException("localName");

			_prefix = prefix;
			_localName = localName;
			_name = string.IsNullOrWhiteSpace(prefix) ? localName : (prefix + ":" + localName);
			_value = value;
			_namespaceUri = namespaceUri;
		}
		#endregion

		#region 公共属性
		public string LocalName
		{
			get
			{
				return _localName;
			}
		}

		public string Prefix
		{
			get
			{
				return _prefix;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return _namespaceUri;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
		}
		#endregion
	}
}
