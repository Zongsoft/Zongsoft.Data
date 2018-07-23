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
	public abstract class MetadataEntityProperty : IEntityPropertyMetadata
	{
		#region 成员字段
		private IEntityMetadata _entity;
		private string _name;
		private string _alias;
		private System.Data.DbType _type;
		#endregion

		#region 构造函数
		protected MetadataEntityProperty(IEntityMetadata entity, string name, System.Data.DbType type)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_entity = entity ?? throw new ArgumentNullException(nameof(entity));
			_name = name.Trim();
			_type = type;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取所属的数据实体。
		/// </summary>
		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
			internal set
			{
				_entity = value;
			}
		}

		/// <summary>
		/// 获取数据实体属性的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取数据实体属性的别名。
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
		/// 获取或设置数据实体属性的类型。
		/// </summary>
		public System.Data.DbType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
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
		public override string ToString()
		{
			return $"{_name}({_type.ToString()})@{_entity.Name}";
		}
		#endregion
	}
}
