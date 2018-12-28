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
using System.Data.Common;
using System.Collections.Concurrent;

namespace Zongsoft.Data.Common
{
	/// <summary>
	/// 提供数据连接对象池功能的类。
	/// </summary>
	public class DbConnectionPool
	{
		#region 成员字段
		private readonly IDataSource _source;
		private readonly ConcurrentDictionary<IDataAccessContextBase, DbConnection> _pool;
		#endregion

		#region 构造函数
		public DbConnectionPool(IDataSource source)
		{
			_source = source ?? throw new ArgumentNullException(nameof(source));
			_pool = new ConcurrentDictionary<IDataAccessContextBase, DbConnection>();
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _pool.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return _pool.IsEmpty;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取或创建指定数据访问上下文关联的数据连接。
		/// </summary>
		/// <param name="context">指定关联的数据访问上下文对象。</param>
		/// <returns>返回关联于指定上下文的数据连接对象。</returns>
		public DbConnection Acquire(IDataAccessContextBase context)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			return _pool.GetOrAdd(context, ctx => _source.Driver.CreateConnection(_source.ConnectionString));
		}

		/// <summary>
		/// 释放指定数据访问上下文关联的数据连接。
		/// </summary>
		/// <param name="context">指定关联的数据访问上下文对象。</param>
		public void Release(IDataAccessContextBase context)
		{
			if(context == null)
				return;

			if(_pool.TryRemove(context, out var connection))
				connection.Dispose();
		}
		#endregion
	}
}
