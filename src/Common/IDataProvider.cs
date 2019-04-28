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

namespace Zongsoft.Data.Common
{
	/// <summary>
	/// 表示数据提供程序的接口。
	/// </summary>
	public interface IDataProvider
	{
		#region 事件定义
		/// <summary>
		/// 提供数据访问错误的事件。
		/// </summary>
		event EventHandler<DataAccessErrorEventArgs> Error;

		/// <summary>
		/// 提供数据访问开始执行的事件。
		/// </summary>
		event EventHandler<DataAccessEventArgs<IDataAccessContext>> Executing;

		/// <summary>
		/// 提供数据访问执行完成的事件。
		/// </summary>
		event EventHandler<DataAccessEventArgs<IDataAccessContext>> Executed;
		#endregion

		#region 属性定义
		/// <summary>
		/// 获取数据提供程序的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取或设置数据执行器。
		/// </summary>
		IDataExecutor Executor
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置数据提供程序的连接器。
		/// </summary>
		IDataMultiplexer Multiplexer
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置数据提供程序的元数据管理器。
		/// </summary>
		Metadata.IMetadataManager Metadata
		{
			get;
			set;
		}
		#endregion

		#region 方法定义
		/// <summary>
		/// 执行数据操作。
		/// </summary>
		/// <param name="context">数据操作的上下文。</param>
		void Execute(IDataAccessContext context);
		#endregion
	}
}
