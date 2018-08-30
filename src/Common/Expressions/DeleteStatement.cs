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
	public class DeleteStatement : Statement
	{
		#region 私有变量
		private int _aliasIndex;
		#endregion

		#region 构造函数
		public DeleteStatement(IDataSource source, IEntityMetadata entity, params TableIdentifier[] tables) : base(source)
		{
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.From = new SourceCollection();

			if(tables != null)
				this.Tables = new List<TableIdentifier>(tables);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取删除语句的入口实体。
		/// </summary>
		public IEntityMetadata Entity
		{
			get;
		}

		/// <summary>
		/// 获取或设置输出子句。
		/// </summary>
		public IExpression Output
		{
			get;
			set;
		}

		/// <summary>
		/// 获取一个表标识的集合，表示要删除的表。
		/// </summary>
		public ICollection<TableIdentifier> Tables
		{
			get;
		}

		/// <summary>
		/// 获取一个数据源的集合，可以在 Where 子句中引用的字段源。
		/// </summary>
		public INamedCollection<ISource> From
		{
			get;
		}

		/// <summary>
		/// 获取或设置删除条件子句。
		/// </summary>
		public IExpression Where
		{
			get;
			set;
		}
		#endregion

		#region 公共方法
		public TableIdentifier CreateTable(IEntityMetadata entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			if(string.IsNullOrEmpty(entity.Alias))
				return this.CreateTable(entity.Name.Replace('.', '_'));
			else
				return this.CreateTable(entity.Alias);
		}

		public TableIdentifier CreateTable(string name)
		{
			return new TableIdentifier(name, "T" + (++_aliasIndex).ToString());
		}
		#endregion
	}
}
