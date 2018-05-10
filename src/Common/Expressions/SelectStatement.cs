/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

namespace Zongsoft.Data.Common.Expressions
{
	public class SelectStatement : ISource, IExpression
	{
		#region 私有变量
		private int _aliasIndex;
		private string _alias;
		private Collections.INamedCollection<SelectStatement> _slaves;
		#endregion

		#region 构造函数
		public SelectStatement(params ISource[] sources) : this(null, sources)
		{
		}

		public SelectStatement(string name, params ISource[] sources)
		{
			this.Name = name;
			this.Select = new SelectClause();
			this.From = new SourceCollection();

			if(sources != null && sources.Length > 0)
			{
				foreach(var source in sources)
					this.From.Add(source);
			}
		}

		public SelectStatement(IEnumerable<ISource> sources) : this(null, sources)
		{
		}

		public SelectStatement(string name, IEnumerable<ISource> sources)
		{
			this.Name = name;
			this.Select = new SelectClause();
			this.From = new SourceCollection();

			if(sources != null)
			{
				foreach(var source in sources)
					this.From.Add(source);
			}
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get;
		}

		public string Alias
		{
			get
			{
				return _alias;
			}
		}

		public SelectClause Select
		{
			get;
		}

		public Collections.INamedCollection<ISource> From
		{
			get;
		}

		public IIdentifier Into
		{
			get;
			set;
		}

		public IExpression Where
		{
			get;
			set;
		}

		public GroupByClause GroupBy
		{
			get;
			set;
		}

		public OrderByClause OrderBy
		{
			get;
			set;
		}

		public bool HasSlaves
		{
			get
			{
				return _slaves != null && _slaves.Count > 0;
			}
		}

		public Collections.INamedCollection<SelectStatement> Slaves
		{
			get
			{
				if(_slaves == null)
				{
					lock(this)
					{
						if(_slaves == null)
							_slaves = new Collections.NamedCollection<SelectStatement>(p => p.Name, StringComparer.OrdinalIgnoreCase);
					}
				}

				return _slaves;
			}
		}
		#endregion

		#region 公共方法
		public TableIdentifier CreateTable(Metadata.IEntity entity)
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

		public SelectStatement GetSubquery(params string[] fields)
		{
			if(string.IsNullOrEmpty(_alias))
				System.Threading.Interlocked.CompareExchange(ref _alias, "T_" + Zongsoft.Common.RandomGenerator.GenerateString(), null);

			//构建一个新的临时表查询语句
			var statement = new SelectStatement(TableIdentifier.Temporary(_alias));

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
		private class SourceCollection : Collections.NamedCollectionBase<ISource>
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
