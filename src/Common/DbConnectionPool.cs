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
		private readonly ConcurrentDictionary<IDataAccessContextBase, Collections.ObjectPool<DbConnection>> _pools;
		#endregion

		#region 构造函数
		public DbConnectionPool(IDataSource source)
		{
			_source = source ?? throw new ArgumentNullException(nameof(source));
			_pools = new ConcurrentDictionary<IDataAccessContextBase, Collections.ObjectPool<DbConnection>>();
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _pools.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return _pools.IsEmpty;
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

			return _pools.GetOrAdd(context, ctx => this.CreatePool()).GetObject();
		}

		/// <summary>
		/// 释放指定数据访问上下文关联的数据连接。
		/// </summary>
		/// <param name="context">指定关联的数据访问上下文对象。</param>
		public void Release(IDataAccessContextBase context)
		{
			if(context == null)
				return;

			if(_pools.TryRemove(context, out var pool))
			{
				pool.Clear();
				pool.Dispose();
			}
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private Collections.ObjectPool<DbConnection> CreatePool()
		{
			return new Collections.ObjectPool<DbConnection>(() => _source.Driver.CreateConnection(_source.ConnectionString), connection => connection.Dispose());
		}
		#endregion
	}
}
