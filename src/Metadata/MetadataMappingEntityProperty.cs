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
	/// 表示实体属性映射元素的类。
	/// </summary>
	public class MetadataMappingEntityProperty : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _fieldName;
		#endregion

		#region 构造函数
		public MetadataMappingEntityProperty(string name, string fieldName)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(string.IsNullOrWhiteSpace(fieldName))
				throw new ArgumentNullException("fieldName");

			_name = name.Trim();
			_fieldName = fieldName.Trim();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		public string FieldName
		{
			get
			{
				return _fieldName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_fieldName = value.Trim();
			}
		}

		public MetadataMappingEntity Mapping
		{
			get
			{
				return (MetadataMappingEntity)base.Owner;
			}
		}
		#endregion
	}
}
