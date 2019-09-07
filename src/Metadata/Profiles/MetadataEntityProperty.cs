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
	/// 表示数据实体属性的元数据抽象基类。
	/// </summary>
	public abstract class MetadataEntityProperty : IEntityPropertyMetadata, IEquatable<IEntityPropertyMetadata>
	{
		#region 构造函数
		protected MetadataEntityProperty(IEntityMetadata entity, string name, System.Data.DbType type, bool immutable)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.Name = name.Trim();
			this.Type = type;
			this.Immutable = immutable;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取所属的数据实体。
		/// </summary>
		public IEntityMetadata Entity
		{
			get; internal set;
		}

		/// <summary>
		/// 获取数据实体属性的名称。
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取数据实体属性的别名。
		/// </summary>
		public string Alias
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置数据实体属性的类型。
		/// </summary>
		public System.Data.DbType Type
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置数据实体属性是否为不可变更属性。
		/// </summary>
		public bool Immutable
		{
			get; set;
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为主键。
		/// </summary>
		public abstract bool IsPrimaryKey
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为单值类型。
		/// </summary>
		public abstract bool IsSimplex
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为复合类型。
		/// </summary>
		public abstract bool IsComplex
		{
			get;
		}
		#endregion

		#region 重写方法
		public virtual bool Equals(IEntityPropertyMetadata other)
		{
			return object.Equals(this.Entity, other.Entity) &&
			       string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return this.Equals((IEntityPropertyMetadata)obj);
		}

		public override int GetHashCode()
		{
			return this.Entity.GetHashCode() ^ this.Name.GetHashCode();
		}

		public override string ToString()
		{
			if(this.Entity == null)
				return $"{this.Name}({this.Type.ToString()})";
			else
				return $"{this.Name}({this.Type.ToString()})@{this.Entity.Name}";
		}
		#endregion
	}
}
