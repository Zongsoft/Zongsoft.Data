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

using Zongsoft.Collections;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class InsertStatement : Statement
	{
		#region 构造函数
		public InsertStatement(IEntityMetadata entity, SchemaMember schema)
		{
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.Schema = schema;
			this.Table = new TableIdentifier(entity);
			this.Fields = new List<FieldIdentifier>();
			this.Values = new List<IExpression>();
		}
		#endregion

		#region 公共属性
		public SchemaMember Schema
		{
			get; set;
		}

		public SelectStatement Sequence
		{
			get; set;
		}

		/// <summary>
		/// 获取插入语句的入口实体。
		/// </summary>
		/// <remarks>
		///		<para>表示当前查询语句对应的入口实体。注意：如果是从属插入的话，该入口实体为导航属性的<seealso cref="Metadata.IEntityComplexPropertyMetadata.Role"/>指定的实体。</para>
		/// </remarks>
		public IEntityMetadata Entity
		{
			get;
		}

		public ReturningClause Returning
		{
			get; set;
		}

		public TableIdentifier Table
		{
			get;
		}

		public IList<FieldIdentifier> Fields
		{
			get;
		}

		public ICollection<IExpression> Values
		{
			get;
		}

		public bool HasValues
		{
			get
			{
				return this.Values != null && this.Values.Count > 0;
			}
		}
		#endregion
	}
}
