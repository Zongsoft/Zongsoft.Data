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
using System.Data;

namespace Zongsoft.Data.Common
{
	/// <summary>
	/// 表示数据事务的接口。
	/// </summary>
	public interface IDataTransaction : IDisposable
	{
		/// <summary>
		/// 获取一个值，指示当前事务是否位于环境中。
		/// </summary>
		bool InAmbient
		{
			get;
		}

		/// <summary>
		/// 获取当前事务的数据源对象。 
		/// </summary>
		IDataSource Source
		{
			get;
		}

		/// <summary>
		/// 绑定指定的命令到当前事务。
		/// </summary>
		/// <param name="command">指定要绑定的数据命令对象。</param>
		void Bind(IDbCommand command);

		/// <summary>
		/// 提交事务。
		/// </summary>
		void Commit();

		/// <summary>
		/// 回滚事务。
		/// </summary>
		void Rollback();
	}
}
