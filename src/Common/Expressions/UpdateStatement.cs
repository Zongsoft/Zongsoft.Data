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
	public class UpdateStatement : Statement
	{
		#region 私有变量
		private int _aliasIndex;
		#endregion

		#region 构造函数
		public UpdateStatement(IEntityMetadata entity)
		{
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.From = new SourceCollection();
			this.Tables = new List<TableIdentifier>();
			this.Fields = new List<FieldValue>();

			var table = new TableIdentifier(entity, "T");
			this.From.Add(table);
			this.Tables.Add(table);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取更新语句的入口实体。
		/// </summary>
		public IEntityMetadata Entity
		{
			get;
		}

		/// <summary>
		/// 获取或设置输出子句。
		/// </summary>
		public ReturningClause Returning
		{
			get;
			set;
		}

		/// <summary>
		/// 获取更新语句的主表（入口实体对应的表）。
		/// </summary>
		public TableIdentifier Table
		{
			get;
		}

		/// <summary>
		/// 获取一个表标识的集合，表示要修改的表。
		/// </summary>
		public IList<TableIdentifier> Tables
		{
			get;
		}

		/// <summary>
		/// 获取一个数据源的集合，可以在 Where 子句中引用的字段源。
		/// </summary>
		public ICollection<ISource> From
		{
			get;
		}

		/// <summary>
		/// 获取更新字段/值的集合。
		/// </summary>
		public ICollection<FieldValue> Fields
		{
			get;
		}

		/// <summary>
		/// 获取或设置更新条件子句。
		/// </summary>
		public IExpression Where
		{
			get;
			set;
		}
		#endregion

		#region 私有方法
		private TableIdentifier CreateTable(IEntityMetadata entity)
		{
			return new TableIdentifier(entity, "T" + (++_aliasIndex).ToString());
		}
		#endregion
	}
}
