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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Collections;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	/// <summary>
	/// 表示查询语句的类。
	/// </summary>
	public class SelectStatement : Statement, ISource
	{
		#region 私有变量
		private int _aliasIndex;
		#endregion

		#region 构造函数
		public SelectStatement(ISource source, string alias = null)
		{
			if(source == null)
				throw new ArgumentNullException(nameof(source));

			this.Alias = alias ?? source.Alias;
			this.Table = source as TableIdentifier;
			this.Select = new SelectClause();
			this.From = new SourceCollection(source);
		}

		public SelectStatement(IEntityMetadata entity, string alias = null)
		{
			this.Alias = alias ?? entity.Name;
			this.Table = new TableIdentifier(entity, "T");
			this.Select = new SelectClause();
			this.From = new SourceCollection(this.Table);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取查询语句的主表标识。
		/// </summary>
		public TableIdentifier Table
		{
			get;
		}

		/// <summary>
		/// 获取查询语句的别名。
		/// </summary>
		public string Alias
		{
			get;
		}

		/// <summary>
		/// 获取查询语句的选择子句。
		/// </summary>
		public SelectClause Select
		{
			get;
		}

		/// <summary>
		/// 获取查询语句的来源集合。
		/// </summary>
		/// <remarks>
		///		<para>来源集合中的第一个元素被称为主源，其他的则都是关联查询（即<see cref="JoinClause"/>关联子句）。</para>
		/// </remarks>
		public INamedCollection<ISource> From
		{
			get;
		}

		/// <summary>
		/// 获取或设置查询语句的输出表标识。
		/// </summary>
		/// <remarks>
		///		<para>在一对多关系（即含有从属查询语句）中，通常需要通过主查询语句的输出表标识来进行关联数据过滤。</para>
		/// </remarks>
		public IIdentifier Into
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置查询语句的过滤条件表达式。
		/// </summary>
		public IExpression Where
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置查询语句的分组子句。
		/// </summary>
		public GroupByClause GroupBy
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置查询语句的排序子句。
		/// </summary>
		public OrderByClause OrderBy
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置查询语句的分页信息。
		/// </summary>
		public Paging Paging
		{
			get;
			set;
		}
		#endregion

		#region 公共方法
		public FieldIdentifier CreateField(string name, string alias = null)
		{
			return new FieldIdentifier(this, name, alias);
		}

		public FieldIdentifier CreateField(IEntityPropertyMetadata property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			return new FieldIdentifier(this, property.GetFieldName(out var alias), alias);
		}

		/// <summary>
		/// 创建指定表与它父类（如果有的话）的继承关联子句。
		/// </summary>
		/// <param name="table">指定要创建的关联子句的子表标识。</param>
		/// <param name="fullPath">指定的 <paramref name="table"/> 参数对应的成员完整路径。</param>
		/// <returns>返回创建的继承表关联子句，如果指定的表实体没有父实体则返回空(null)。</returns>
		public JoinClause Join(TableIdentifier table, string fullPath = null)
		{
			return JoinClause.Create(table,
				fullPath,
				name => this.From.TryGet(name, out var clause) ? (JoinClause)clause : null,
				entity => this.CreateTableReference(entity));
		}

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
				entity => this.CreateTableReference(entity));
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
				entity => this.CreateTableReference(entity));

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
		public JoinClause Join(ISource source, SchemaEntry schema)
		{
			if(schema.Token.Property.IsSimplex)
				return null;

			return this.Join(source, (IEntityComplexPropertyMetadata)schema.Token.Property, schema.FullPath);
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private TableIdentifier CreateTableReference(IEntityMetadata entity)
		{
			return new TableIdentifier(entity, "T" + (++_aliasIndex).ToString());
		}
		#endregion
	}
}
