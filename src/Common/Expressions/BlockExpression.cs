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
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// <summary>
	/// 表示由多个表达式组成的块级表达式类。
	/// </summary>
	public class BlockExpression : Expression, ICollection<IExpression>
	{
		#region 成员字段
		private IList<IExpression> _items;
		#endregion

		#region 构造函数
		public BlockExpression(BlockExpressionDelimiter delimiter = BlockExpressionDelimiter.None)
		{
			this.Delimiter = delimiter;
			_items = new List<IExpression>();
		}

		public BlockExpression(BlockExpressionDelimiter delimiter, IEnumerable<IExpression> items)
		{
			this.Delimiter = delimiter;

			if(items == null)
				_items = new List<IExpression>();
			else
				_items = new List<IExpression>(items);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取块级表达式的元素分割符。
		/// </summary>
		public BlockExpressionDelimiter Delimiter
		{
			get;
		}

		public int Count
		{
			get => _items.Count;
		}

		bool ICollection<IExpression>.IsReadOnly
		{
			get => false;
		}

		public IExpression this[int index]
		{
			get => _items[index];
		}
		#endregion

		#region 公共方法
		public void Add(IExpression item)
		{
			if(item != null)
				_items.Add(item);
		}

		public void Insert(int index, IExpression item)
		{
			_items.Insert(index, item);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool Remove(IExpression item)
		{
			return _items.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
		}
		#endregion

		#region 显式实现
		bool ICollection<IExpression>.Contains(IExpression item)
		{
			return _items.Contains(item);
		}

		void ICollection<IExpression>.CopyTo(IExpression[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}
		#endregion

		#region 迭代遍历
		public IEnumerator<IExpression> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion
	}

	/// <summary>
	/// 表示块级表达式的分割符枚举。
	/// </summary>
	public enum BlockExpressionDelimiter
	{
		/// <summary>无分隔。</summary>
		None,

		/// <summary>空格符。</summary>
		Space,

		/// <summary>换行符。</summary>
		Break,

		/// <summary>语句终结符，通常为分号。</summary>
		Terminator,
	}
}
