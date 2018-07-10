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
		private string _alias;
		private SlaveCollection _slaves;
		private INamedCollection<ParameterExpression> _parameters;
		#endregion

		#region 构造函数
		public SelectStatement(IEntityMetadata entity, params ISource[] sources)
		{
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.Select = new SelectClause();
			this.From = new SourceCollection();

			if(sources != null && sources.Length > 0)
			{
				foreach(var source in sources)
					this.From.Add(source);
			}
		}

		private SelectStatement(SlaveInfo info, params ISource[] sources)
		{
			this.Slaver = info ?? throw new ArgumentNullException(nameof(info));

			var foreignProperty = info.Umbilical.GetForeignProperty();
			this.Entity = foreignProperty == null ? info.Umbilical.GetForeignEntity() : foreignProperty.Entity;

			this.Select = new SelectClause();
			this.From = new SourceCollection();

			if(sources != null && sources.Length > 0)
			{
				foreach(var source in sources)
					this.From.Add(source);
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取查询语句的入口实体。
		/// </summary>
		/// <remarks>
		///		<para>表示当前查询语句对应的入口实体。注意：如果是从属查询的话，该入口实体为一对多导航属性的<seealso cref="Metadata.IEntityComplexPropertyMetadata.Role"/>指定的实体，它未必对应为<see cref="From"/>属性值中的第一个元素。</para>
		/// </remarks>
		public IEntityMetadata Entity
		{
			get;
		}

		/// <summary>
		/// 获取查询语句的别名。
		/// </summary>
		/// <remarks>
		///		<para>从属查询语句主要通过依附的主查询语句的别名来做关联数据过滤。</para>
		/// </remarks>
		public string Alias
		{
			get
			{
				return _alias;
			}
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

		/// <summary>
		/// 获取从属查询语句的从属信息，如果该属性值不为空(null)，则表示当前查询语句是一个从属查询语句。。
		/// </summary>
		public SlaveInfo Slaver
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示当前查询语句是否为一个从属查询语句。
		/// </summary>
		public bool IsSlave
		{
			get
			{
				return this.Slaver != null;
			}
		}

		/// <summary>
		/// 获取一个值值，指示当前查询语句是否有依附于自己的从属查询。
		/// </summary>
		public bool HasSlaves
		{
			get
			{
				return _slaves != null && _slaves.Count > 0;
			}
		}

		/// <summary>
		/// 获取依附于当前查询语句的从属查询语句集合。
		/// </summary>
		/// <remarks>
		///		<para>对于只是获取从属查询语句的使用者，应先使用<see cref="HasSlaves"/>属性进行判断成功后再使用该属性，这样可避免创建不必要的集合对象。</para>
		/// </remarks>
		public IReadOnlyNamedCollection<SelectStatement> Slaves
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

		public override bool HasParameters
		{
			get
			{
				if(this.Slaver == null)
					return _parameters != null && _parameters.Count > 0;
				else
					return this.Slaver.Master.HasParameters;
			}
		}

		public override INamedCollection<ParameterExpression> Parameters
		{
			get
			{
				if(this.Slaver == null)
				{
					if(_parameters == null)
					{
						lock(this)
						{
							if(_parameters == null)
								_parameters = this.CreateParameters();
						}
					}

					return _parameters;
				}

				return this.Slaver.Master.Parameters;
			}
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

		public FieldIdentifier CreateField(string name, string alias = null)
		{
			return new FieldIdentifier(this, name, alias);
		}

		public SelectStatement CreateSlave(string name, IEntityComplexPropertyMetadata umbilical, ISource source, IExpression where = null)
		{
			var slave = ((SlaveCollection)this.Slaves).Add(name, umbilical, source, where);

			if(slave != null)
			{
				if(string.IsNullOrEmpty(_alias))
					System.Threading.Interlocked.CompareExchange(ref _alias, "T_" + Zongsoft.Common.RandomGenerator.GenerateString(), null);
			}

			return slave;
		}

		public ISource CreateTemporaryReference(params string[] fields)
		{
			if(string.IsNullOrEmpty(_alias))
				System.Threading.Interlocked.CompareExchange(ref _alias, "T_" + Zongsoft.Common.RandomGenerator.GenerateString(), null);

			if(fields == null || fields.Length == 0)
				return TableIdentifier.Temporary(_alias);

			//构建一个新的临时表查询语句
			var statement = new SelectStatement(this.Entity, TableIdentifier.Temporary(_alias));

			if(fields != null && fields.Length > 0)
			{
				foreach(var field in fields)
				{
					statement.Select.Members.Add(statement.CreateField(field));
				}
			}

			return statement;
		}
		#endregion

		#region 嵌套子类
		/// <summary>
		/// 表示从属查询语句的信息类。
		/// </summary>
		public class SlaveInfo
		{
			#region 构造函数
			public SlaveInfo(SelectStatement master, string name, Metadata.IEntityComplexPropertyMetadata umbilical)
			{
				this.Master = master ?? throw new ArgumentNullException(nameof(master));
				this.Name = name ?? throw new ArgumentNullException(nameof(name));
				this.Umbilical = umbilical ?? throw new ArgumentNullException(nameof(umbilical));
			}
			#endregion

			#region 公共属性
			/// <summary>
			/// 获取从属查询语句的名称。
			/// </summary>
			public string Name
			{
				get;
			}

			/// <summary>
			/// 获取从属查询语句所依附的主查询语句。
			/// </summary>
			public SelectStatement Master
			{
				get;
			}

			/// <summary>
			/// 获取从属查询语句的关联线（即关联的一对多导航属性）。
			/// </summary>
			public Metadata.IEntityComplexPropertyMetadata Umbilical
			{
				get;
			}
			#endregion
		}

		private class SlaveCollection : ReadOnlyNamedCollectionBase<SelectStatement>
		{
			#region 成员字段
			private SelectStatement _master;
			#endregion

			#region 构造函数
			public SlaveCollection(SelectStatement master)
			{
				_master = master ?? throw new ArgumentNullException(nameof(master));
			}
			#endregion

			#region 重写方法
			protected override string GetKeyForItem(SelectStatement item)
			{
				return item.Slaver.Name;
			}
			#endregion

			#region 公共方法
			public SelectStatement Add(string name, IEntityComplexPropertyMetadata umbilical, ISource source, IExpression where = null)
			{
				if(string.IsNullOrEmpty(name))
					throw new ArgumentNullException(nameof(name));

				if(umbilical == null)
					throw new ArgumentNullException(nameof(umbilical));

				var info = new SlaveInfo(_master, name, umbilical);

				//创建一个附属查询语句
				var slave = new SelectStatement(info, source)
				{
					Where = where,
				};

				//将新建的附属查询语句加入到当前主查询语句的附属子集中
				this.InnerDictionary.Add(name, slave);

				//返回新建的附属查询语句
				return slave;
			}
			#endregion
		}

		private class SourceCollection : NamedCollectionBase<ISource>
		{
			#region 重写方法
			protected override string GetKeyForItem(ISource item)
			{
				return item.Alias;
			}

			protected override ISource GetItem(string name)
			{
				if(this.TryGetItem(name, out var item))
					return item;

				throw new KeyNotFoundException();
			}

			protected override bool TryGetItem(string name, out ISource value)
			{
				if(base.TryGetItem(name, out value))
					return true;

				foreach(var entry in this.InnerDictionary)
				{
					if(entry.Value is JoinClause joining)
					{
						if(string.Equals(joining.Name, name, StringComparison.OrdinalIgnoreCase))
						{
							value = joining;
							return true;
						}
					}
				}

				return false;
			}

			protected override bool ContainsName(string name)
			{
				if(base.ContainsName(name))
					return true;

				foreach(var entry in this.InnerDictionary)
				{
					if(entry.Value is JoinClause joining)
					{
						if(string.Equals(joining.Name, name, StringComparison.OrdinalIgnoreCase))
							return true;
					}
				}

				return false;
			}
			#endregion
		}
		#endregion
	}
}
