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
		#region 成员字段
		private SlaveCollection _slaves;
		#endregion

		#region 构造函数
		public InsertStatement(IEntityMetadata entity)
		{
			this.Table = new TableIdentifier(entity);
			this.Fields = new List<FieldIdentifier>();
			this.Values = new List<IExpression>();
		}

		public InsertStatement(string name, IEntityMetadata entity)
		{
			this.Name = name;
			this.Table = new TableIdentifier(entity);
			this.Fields = new List<FieldIdentifier>();
			this.Values = new List<IExpression>();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get;
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

		public object Data
		{
			get;
			set;
		}

		public IExpression Output
		{
			get;
			set;
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

		/// <summary>
		/// 获取一个值，指示当前插入语句是否有依附于自己的从属语句。
		/// </summary>
		public bool HasSlaves
		{
			get
			{
				return _slaves != null && _slaves.Count > 0;
			}
		}

		/// <summary>
		/// 获取依附于当前插入语句的从属语句集合。
		/// </summary>
		/// <remarks>
		///		<para>对于只是获取从属语句的使用者，应先使用<see cref="HasSlaves"/>属性进行判断成功后再使用该属性，这样可避免创建不必要的集合对象。</para>
		/// </remarks>
		public INamedCollection<IStatement> Slaves
		{
			get
			{
				if(_slaves == null)
				{
					lock(this)
					{
						if(_slaves == null)
							_slaves = new SlaveCollection(this);
					}
				}

				return _slaves;
			}
		}
		#endregion

		private class SlaveCollection : NamedCollectionBase<InsertStatement>
		{
			#region 成员字段
			private InsertStatement _master;
			#endregion

			#region 构造函数
			public SlaveCollection(InsertStatement master)
			{
				_master = master ?? throw new ArgumentNullException(nameof(master));
			}
			#endregion

			#region 重写方法
			protected override string GetKeyForItem(InsertStatement item)
			{
				return item.Name;
			}
			#endregion
		}
	}
}
