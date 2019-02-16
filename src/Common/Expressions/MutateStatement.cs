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
	/// <summary>
	/// 表示写入语句（包括更新、删除等语句）的基类。
	/// </summary>
	public class MutateStatement : MutateStatementBase
	{
		#region 私有变量
		private int _aliasIndex;
		#endregion

		#region 构造函数
		protected MutateStatement(IEntityMetadata entity, SchemaMember schema = null) : base(entity, schema)
		{
			this.From = new SourceCollection();
			this.From.Add(this.Table);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个数据源的集合，可以在 Where 子句中引用的字段源。
		/// </summary>
		public INamedCollection<ISource> From
		{
			get;
		}

		/// <summary>
		/// 获取或设置写入（删除、更新）的条件子句。
		/// </summary>
		public IExpression Where
		{
			get;
			set;
		}
		#endregion

		#region 保护方法
		protected TableIdentifier CreateTable(IEntityMetadata entity)
		{
			return new TableIdentifier(entity, "T" + (++_aliasIndex).ToString());
		}
		#endregion
	}
}
