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
using System.Data;
using System.Collections.Generic;

using Zongsoft.Collections;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class TableDefinition : Expression, IStatement
	{
		#region 成员字段
		private IList<IStatement> _slaves;
		#endregion

		#region 构造函数
		public TableDefinition(string name, IEnumerable<IEntitySimplexPropertyMetadata> fields = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name;
			this.Fields = new List<FieldDefinition>();

			if(fields != null)
			{
				foreach(var field in fields)
				{
					this.Field(field);
				}
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取表的名称。
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示当前表是否为临时表或表变量。
		/// </summary>
		public bool IsTemporary
		{
			get;
			private set;
		}

		/// <summary>
		/// 获取表的字段定义列表。
		/// </summary>
		public IList<FieldDefinition> Fields
		{
			get;
		}
		#endregion

		#region 公共方法
		public FieldDefinition Field(IEntitySimplexPropertyMetadata property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var field = new FieldDefinition(property.GetFieldName(out _), property.Type, property.Nullable)
			{
				Length = property.Length,
				Precision = property.Precision,
				Scale = property.Scale,
			};

			this.Fields.Add(field);
			return field;
		}

		public FieldDefinition Field(string name, DbType dbType, bool nullable = true)
		{
			var field = new FieldDefinition(name, dbType, nullable);
			this.Fields.Add(field);
			return field;
		}

		public FieldDefinition Field(string name, DbType dbType, int length, bool nullable = true)
		{
			var field = new FieldDefinition(name, dbType, nullable)
			{
				Length = length,
			};

			this.Fields.Add(field);
			return field;
		}

		public FieldDefinition Field(string name, DbType dbType, byte precision, byte scale, bool nullable = true)
		{
			var field = new FieldDefinition(name, dbType, nullable)
			{
				Precision = precision,
				Scale = scale,
			};

			this.Fields.Add(field);
			return field;
		}
		#endregion

		#region 显式实现
		Schema IStatement.Schema
		{
			get => null;
			set => throw new NotSupportedException();
		}

		bool IStatement.HasParameters => false;

		INamedCollection<ParameterExpression> IStatement.Parameters => throw new NotSupportedException();

		public bool HasSlaves
		{
			get
			{
				return _slaves != null && _slaves.Count > 0;
			}
		}

		public ICollection<IStatement> Slaves
		{
			get
			{
				if(_slaves == null)
					System.Threading.Interlocked.CompareExchange(ref _slaves, new List<IStatement>(), null);

				return _slaves;
			}
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个临时表的定义。
		/// </summary>
		/// <param name="fields">指定新建临时表的字段定义集。</param>
		/// <returns>返回新建的临时表定义。</returns>
		public static TableDefinition Temporary(IEnumerable<IEntitySimplexPropertyMetadata> fields = null)
		{
			return new TableDefinition("T" + Zongsoft.Common.RandomGenerator.GenerateString(), fields)
			{
				IsTemporary = true
			};
		}

		/// <summary>
		/// 创建一个临时表的定义。
		/// </summary>
		/// <param name="name">指定的要新建的临时表名。</param>
		/// <param name="fields">指定新建临时表的字段定义集。</param>
		/// <returns>返回新建的临时表定义。</returns>
		public static TableDefinition Temporary(string name, IEnumerable<IEntitySimplexPropertyMetadata> fields = null)
		{
			return new TableDefinition(name, fields)
			{
				IsTemporary = true
			};
		}
		#endregion
	}
}
