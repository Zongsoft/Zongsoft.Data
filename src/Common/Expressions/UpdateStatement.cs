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
	public class UpdateStatement : MutateStatement
	{
		#region 构造函数
		public UpdateStatement(IEntityMetadata entity) : base(entity)
		{
			this.Fields = new List<FieldValue>();
			this.Tables = new List<TableIdentifier>();
			this.Tables.Add(this.Table);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个表标识的集合，表示要修改的表。
		/// </summary>
		public IList<TableIdentifier> Tables
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
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取或创建指定源与实体的继承关联子句。
		/// </summary>
		/// <param name="source">指定要创建关联子句的源。</param>
		/// <param name="target">指定要创建关联子句的目标实体。</param>
		/// <param name="fullPath">指定的 <paramref name="target"/> 参数对应的目标实体关联的成员的完整路径。</param>
		/// <returns>返回已存在或新创建的继承表关联子句。</returns>
		public JoinClause Join(ISource source, IEntityMetadata target, string fullPath = null)
		{
			return JoinClause.Create(source,
				target,
				fullPath,
				name => this.From.TryGet(name, out var join) ? (JoinClause)join : null,
				entity => this.CreateTable(entity));
		}

		/// <summary>
		/// 获取或创建指定导航属性的关联子句。
		/// </summary>
		/// <param name="source">指定要创建关联子句的源。</param>
		/// <param name="complex">指定要创建关联子句对应的导航属性。</param>
		/// <param name="fullPath">指定的 <paramref name="complex"/> 参数对应的成员完整路径。</param>
		/// <returns>返回已存在或新创建的导航关联子句。</returns>
		public JoinClause Join(ISource source, IEntityComplexPropertyMetadata complex, string fullPath = null)
		{
			var joins = JoinClause.Create(source,
				complex,
				fullPath,
				name => this.From.TryGet(name, out var join) ? (JoinClause)join : null,
				entity => this.CreateTable(entity));

			JoinClause result = null;

			foreach(var join in joins)
			{
				if(!this.From.Contains(join))
					this.From.Add(join);

				result = join;
			}

			//返回最后一个Join子句
			return result;
		}

		/// <summary>
		/// 获取或创建导航属性的关联子句。
		/// </summary>
		/// <param name="source">指定要创建关联子句的源。</param>
		/// <param name="schema">指定要创建关联子句对应的数据模式成员。</param>
		/// <returns>返回已存在或新创建的导航关联子句，如果 <paramref name="schema"/> 参数指定的数据模式成员对应的不是导航属性则返回空(null)。</returns>
		public JoinClause Join(ISource source, SchemaMember schema)
		{
			if(schema.Token.Property.IsSimplex)
				return null;

			return this.Join(source, (IEntityComplexPropertyMetadata)schema.Token.Property, schema.FullPath);
		}
		#endregion
	}
}
