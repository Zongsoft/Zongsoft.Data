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

namespace Zongsoft.Data.Common.Expressions
{
	public class StatementCollection : Expression, IList<IStatement>
	{
		#region 成员字段
		private IList<IStatement> _statements;
		private Collections.INamedCollection<ParameterExpression> _parameters;
		#endregion

		#region 构造函数
		public StatementCollection()
		{
			_statements = new List<IStatement>();
		}
		#endregion

		#region 公共属性
		public virtual bool HasParameters
		{
			get
			{
				return _parameters != null && _parameters.Count > 0;
			}
		}

		public virtual Collections.INamedCollection<ParameterExpression> Parameters
		{
			get
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
		}

		public IStatement this[int index]
		{
			get => _statements[index];
			set => _statements[index] = value;
		}

		public int Count => _statements.Count;

		public bool IsReadOnly => false;
		#endregion

		#region 公共方法
		public void Add(IStatement statement)
		{
			if(statement == null)
				throw new ArgumentNullException(nameof(statement));

			_statements.Add(statement);
		}

		public void Clear()
		{
			_statements.Clear();
		}

		public bool Contains(IStatement statement)
		{
			return _statements.Contains(statement);
		}

		public void CopyTo(IStatement[] array, int arrayIndex)
		{
			_statements.CopyTo(array, arrayIndex);
		}

		public int IndexOf(IStatement statement)
		{
			return _statements.IndexOf(statement);
		}

		public void Insert(int index, IStatement item)
		{
			_statements.Insert(index, item);
		}

		public bool Remove(IStatement item)
		{
			return _statements.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_statements.RemoveAt(index);
		}
		#endregion

		#region 重写方法
		protected internal override IExpression Accept(IExpressionVisitor visitor)
		{
			foreach(var statement in _statements)
			{
				visitor.Visit(statement);
			}

			return this;
		}
		#endregion

		#region 虚拟方法
		protected virtual Collections.INamedCollection<ParameterExpression> CreateParameters()
		{
			return new ParameterCollection();
		}
		#endregion

		#region 枚举遍历
		public IEnumerator<IStatement> GetEnumerator()
		{
			return _statements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _statements.GetEnumerator();
		}
		#endregion

		#region 嵌套子类
		private class ParameterCollection : Collections.NamedCollectionBase<ParameterExpression>
		{
			private int _index;

			protected override string GetKeyForItem(ParameterExpression item)
			{
				return item.Name;
			}

			protected override void AddItem(ParameterExpression item)
			{
				if(string.IsNullOrEmpty(item.Name) || item.Name == "?")
				{
					var index = System.Threading.Interlocked.Increment(ref _index);
					item.Name = "p" + index.ToString();
				}

				base.AddItem(item);
			}
		}
		#endregion
	}
}
