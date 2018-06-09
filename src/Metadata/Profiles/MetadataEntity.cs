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
	/// 表示数据实体的元数据类。
	/// </summary>
	public class MetadataEntity : IEntity, IEquatable<IEntity>
	{
		#region 成员字段
		private string _name;
		private string _alias;
		private string _baseName;
		private IMetadataProvider _provider;
		private IEntitySimplexProperty[] _key;
		private IEntityPropertyCollection _properties;
		#endregion

		#region 构造函数
		public MetadataEntity(IMetadataProvider provider, string name, string baseName = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_baseName = baseName;
			_provider = provider ?? throw new ArgumentNullException(nameof(provider));
			_properties = new MetadataEntityPropertyCollection(this);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据实体所属的提供程序。
		/// </summary>
		public IMetadataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		/// <summary>
		/// 获取数据实体的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取或设置数据实体的别名。
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
		/// 获取或设置数据实体的主键属性数组。
		/// </summary>
		public IEntitySimplexProperty[] Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		/// <summary>
		/// 获取或设置数据实体继承的父实体名。
		/// </summary>
		public string BaseName
		{
			get
			{
				return _baseName;
			}
			set
			{
				_baseName = value;
			}
		}

		/// <summary>
		/// 获取数据实体的属性元数据集合。
		/// </summary>
		public IEntityPropertyCollection Properties
		{
			get
			{
				return _properties;
			}
		}
		#endregion

		#region 重写方法
		public bool Equals(IEntity other)
		{
			return other != null && string.Equals(other.Name, _name) && string.Equals(other.Alias, _alias);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return this.Equals((IEntity)obj);
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode() ^ _alias.GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrEmpty(_baseName))
				return $"{_name}";
			else
				return $"{_name}:{_baseName}";
		}
		#endregion
	}
}
