﻿/*
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
	/// <summary>
	/// 表示数据表定义的表达式类。
	/// </summary>
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
			this.Fields = new NamedCollection<FieldDefinition>(field => field.Name);

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
		/// 获取表的字段定义集。
		/// </summary>
		public INamedCollection<FieldDefinition> Fields
		{
			get;
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 创建一个字段定义并添加到当前表定义的 <see cref="Fields"/> 集中，如果同名字段已经定义则返回空(null)。
		/// </summary>
		/// <param name="property">指定的要添加字段的单值属性元信息。</param>
		/// <returns>返回的新增字段定义项，如果指定属性对应的字段已经存在则返回空(null)。</returns>
		public FieldDefinition Field(IEntitySimplexPropertyMetadata property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var fieldName = property.GetFieldName(out _);

			if(this.Fields.Contains(fieldName))
				return null;

			var field = new FieldDefinition(fieldName, property.Type, property.Nullable)
			{
				Length = property.Length,
				Precision = property.Precision,
				Scale = property.Scale,
			};

			this.Fields.Add(field);
			return field;
		}

		/// <summary>
		/// 创建一个字段定义并添加到当前表定义的 <see cref="Fields"/> 集中，如果同名字段已经定义则返回空(null)。
		/// </summary>
		/// <param name="name">要添加字段的名称。</param>
		/// <param name="dbType">要添加字段的数据类型。</param>
		/// <param name="nullable">要添加字段的可空性（即字段是否允许为空），默认为允许(True)。</param>
		/// <returns>返回的新增字段定义项。</returns>
		public FieldDefinition Field(string name, DbType dbType, bool nullable = true)
		{
			if(this.Fields.Contains(name))
				return null;

			var field = new FieldDefinition(name, dbType, nullable);
			this.Fields.Add(field);
			return field;
		}

		/// <summary>
		/// 创建一个字段定义并添加到当前表定义的 <see cref="Fields"/> 集中，如果同名字段已经定义则返回空(null)。
		/// </summary>
		/// <param name="name">要添加字段的名称。</param>
		/// <param name="dbType">要添加字段的数据类型。</param>
		/// <param name="length">要添加字段的最大长度。</param>
		/// <param name="nullable">要添加字段的可空性（即字段是否允许为空），默认为允许(True)。</param>
		/// <returns>返回的新增字段定义项。</returns>
		public FieldDefinition Field(string name, DbType dbType, int length, bool nullable = true)
		{
			if(this.Fields.Contains(name))
				return null;

			var field = new FieldDefinition(name, dbType, nullable)
			{
				Length = length,
			};

			this.Fields.Add(field);
			return field;
		}

		/// <summary>
		/// 创建一个字段定义并添加到当前表定义的 <see cref="Fields"/> 集中，如果同名字段已经定义则返回空(null)。
		/// </summary>
		/// <param name="name">要添加字段的名称。</param>
		/// <param name="dbType">要添加字段的数据类型。</param>
		/// <param name="precision">要添加字段的数字精度。</param>
		/// <param name="scale">要添加字段的小数点位数。</param>
		/// <param name="nullable">要添加字段的可空性（即字段是否允许为空），默认为允许(True)。</param>
		/// <returns>返回的新增字段定义项。</returns>
		public FieldDefinition Field(string name, DbType dbType, byte precision, byte scale, bool nullable = true)
		{
			if(this.Fields.Contains(name))
				return null;

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
		SchemaEntry IStatement.Schema
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
