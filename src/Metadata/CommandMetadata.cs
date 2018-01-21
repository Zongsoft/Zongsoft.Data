/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据命令的元数据类。
	/// </summary>
	public class CommandMetadata
	{
		#region 成员字段
		private string _name;
		private string _alias;
		private string _text;
		private Type _resultType;
		private ICollection<CommandParameterMetadata> _parameters;
		#endregion

		#region 构造函数
		public CommandMetadata(string name, string alias = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_alias = alias;
			_parameters = new List<CommandParameterMetadata>();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置数据命令的名称。
		/// </summary>
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
		/// 获取或设置数据命令的别名。
		/// </summary>
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
			}
		}

		/// <summary>
		/// 获取或设置数据命令的文本（脚本）。
		/// </summary>
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

		/// <summary>
		/// 获取数据命令的参数集合。
		/// </summary>
		public ICollection<CommandParameterMetadata> Parameters
		{
			get
			{
				return _parameters;
			}
		}
		#endregion
	}
}
