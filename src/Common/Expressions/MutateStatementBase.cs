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
	/// 表示写入语句（包括新增、更新、删除等语句）的基类。
	/// </summary>
	public class MutateStatementBase : Statement
	{
		#region 构造函数
		protected MutateStatementBase(IEntityMetadata entity, SchemaMember schema = null)
		{
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.Table = new TableIdentifier(entity, "T");
			this.Schema = schema;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取写入语句对应的模式成员。
		/// </summary>
		public SchemaMember Schema
		{
			get; set;
		}

		/// <summary>
		/// 获取写入语句的入口实体。
		/// </summary>
		/// <remarks>
		/// 	<para>表示当前查询语句对应的入口实体。注意：如果是从属插入的话，该入口实体为导航属性的<seealso cref="Metadata.IEntityComplexPropertyMetadata.Role"/>指定的实体。</para>
		/// </remarks>
		public IEntityMetadata Entity
		{
			get;
		}

		/// <summary>
		/// 获取写入语句对应的表。
		/// </summary>
		public TableIdentifier Table
		{
			get;
		}

		/// <summary>
		/// 获取或设置写入语句的输出子句。
		/// </summary>
		public ReturningClause Returning
		{
			get; set;
		}
		#endregion
	}
}
