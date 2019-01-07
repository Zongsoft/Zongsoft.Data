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

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	/// <summary>
	/// 表示表标识的表达式。
	/// </summary>
	public class TableIdentifier : Expression, IIdentifier, ISource
	{
		#region 构造函数
		public TableIdentifier(IEntityMetadata entity, string alias = null)
		{
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.Alias = alias;
			this.Name = EntityExtension.GetTableName(entity);
		}

		/// <summary>
		/// 私有构造函数，仅限构造临时表标识。
		/// </summary>
		/// <param name="name">指定的临时表名称。</param>
		/// <param name="alias">指定的临时表别名。</param>
		private TableIdentifier(string name, string alias = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name;
			this.Alias = alias;
			this.IsTemporary = true;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取对应的表元数据元素，如果是临时表则该属性为空(null)。
		/// </summary>
		public IEntityMetadata Entity
		{
			get;
		}

		/// <summary>
		/// 获取表的物理名称（即数据库中表的名称）。
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取表标识的别名。
		/// </summary>
		public string Alias
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示当前表标识是否为一个临时表。
		/// </summary>
		public bool IsTemporary
		{
			get;
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 创建一个关联当前表的字段标识。
		/// </summary>
		/// <param name="name">指定的字段名称。</param>
		/// <param name="alias">指定的字段别名。</param>
		/// <returns>返回创建成功的字段标识。</returns>
		public FieldIdentifier CreateField(string name, string alias = null)
		{
			return new FieldIdentifier(this, name, alias);
		}

		public FieldIdentifier CreateField(IEntityPropertyMetadata property)
		{
			return new FieldIdentifier(this, property.GetFieldName(out var alias), alias);
		}

		public FieldIdentifier CreateField(IEntitySimplexPropertyMetadata property, string alias = null)
		{
			if(string.IsNullOrEmpty(property.Alias))
				return new FieldIdentifier(this, property.Name, alias);
			else
				return new FieldIdentifier(this, property.Alias, alias);
		}

		public FieldIdentifier CreateField(EntityPropertyToken token, string alias = null)
		{
			if(token.Property.IsComplex)
				throw new ArgumentException();

			if(string.IsNullOrEmpty(token.Property.Alias))
				return new FieldIdentifier(this, token.Property.Name, alias) { Token = token };
			else
				return new FieldIdentifier(this, token.Property.Alias, alias) { Token = token };
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(string.IsNullOrEmpty(this.Alias))
				return (this.IsTemporary ? "#" : string.Empty) + this.Name;
			else
				return (this.IsTemporary ? "#" : string.Empty) + this.Name + " AS " + this.Alias;
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个临时表的标识。
		/// </summary>
		/// <param name="name">指定的要新建的临时表标识名。</param>
		/// <param name="alias">指定的要新建的临时表标识别名。</param>
		/// <returns>返回新建的临时表标识。</returns>
		public static TableIdentifier Temporary(string name, string alias = null)
		{
			return new TableIdentifier(name, string.IsNullOrEmpty(alias) ? name : alias);
		}
		#endregion
	}
}
