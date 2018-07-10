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

namespace Zongsoft.Data.Metadata.Profiles
{
	/// <summary>
	/// 表示数据命令的元数据类。
	/// </summary>
	public class MetadataCommand : ICommandMetadata, IEquatable<ICommandMetadata>
	{
		#region 成员字段
		private string _name;
		private string _text;
		private string _alias;
		private IMetadata _provider;
		private Collections.INamedCollection<ICommandParameterMetadata> _parameters;
		#endregion

		#region 构造函数
		public MetadataCommand(IMetadata provider, string name, string alias = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_alias = alias;
			_provider = provider ?? throw new ArgumentNullException(nameof(provider));
			_parameters = new Collections.NamedCollection<ICommandParameterMetadata>(p => p.Name);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据命令所属的提供程序。
		/// </summary>
		public IMetadata Metadata
		{
			get
			{
				return _provider;
			}
		}

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
		/// 获取或设置数据命令的类型。
		/// </summary>
		public CommandType Type
		{
			get;
			set;
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
		public Collections.INamedCollection<ICommandParameterMetadata> Parameters
		{
			get
			{
				return _parameters;
			}
		}
		#endregion

		#region 重写方法
		public bool Equals(ICommandMetadata other)
		{
			return other != null && string.Equals(other.Name, _name);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return this.Equals((ICommandMetadata)obj);
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode();
		}

		public override string ToString()
		{
			return $"{this.Type}:{this.Name}()";
		}
		#endregion
	}
}
