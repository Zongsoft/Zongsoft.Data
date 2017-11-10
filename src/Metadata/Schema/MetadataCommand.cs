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
	public class MetadataCommand : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _text;
		private Type _resultType;
		private MetadataCommandParameterCollection _parameters;
		#endregion

		#region 构造函数
		public MetadataCommand(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_parameters = new MetadataCommandParameterCollection(this);
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

		/// <summary>
		/// 获取命令元素的全称，即为“容器名.元素名”。
		/// </summary>
		public string FullName
		{
			get
			{
				var container = this.Container;

				if(container == null || string.IsNullOrWhiteSpace(container.Name))
					return _name;

				return container.Name + "." + _name;
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public Type ResultType
		{
			get
			{
				return _resultType;
			}
			set
			{
				_resultType = value;
			}
		}

		public MetadataCommandParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// 获取关联元素所属的容器元素。
		/// </summary>
		public MetadataConceptContainer Container
		{
			get
			{
				return (MetadataConceptContainer)base.Owner;
			}
		}
		#endregion
	}
}
