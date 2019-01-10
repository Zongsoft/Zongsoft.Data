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

namespace Zongsoft.Data.Common
{
	public class Feature : IEquatable<Feature>, IComparable<Feature>
	{
		#region 单例字段
		public static readonly Feature MultipleActiveResultSets = new Feature(nameof(MultipleActiveResultSets));
		#endregion

		#region 成员字段
		private readonly string _name;
		private readonly Version _version;
		#endregion

		#region 构造函数
		public Feature(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim().ToLowerInvariant();
		}

		public Feature(string name, Version version)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim().ToLowerInvariant();

			if(version != null && !(version.Major == 0 && version.Minor == 0 && version.Build <= 0 && version.Revision <= 0))
				_version = version;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取功能特性的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取功能特性的版本号。
		/// </summary>
		public Version Version
		{
			get
			{
				return _version;
			}
		}
		#endregion

		#region 重写方法
		public int CompareTo(Feature other)
		{
			if(other == null)
				return -1;

			if(other._name != this._name)
				return -1;

			if(_version == null)
				return other._version == null ? 0 : -1;
			else
				return _version.CompareTo(other._version);
		}

		public bool Equals(Feature other)
		{
			if(other == null)
				return false;

			return other._name == _name &&
			       other._version == _version;
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			if(_version == null)
				return _name.GetHashCode();
			else
				return _name.GetHashCode() ^ _version.GetHashCode();
		}

		public override string ToString()
		{
			if(_version == null)
				return _name;
			else
				return _name + " (" + _version.ToString() + ")";
		}
		#endregion
	}
}
