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
	/// 表示数据连接器的接口。
	/// </summary>
	public interface IDataConnector : IEnumerable<IDataSource>
	{
		/// <summary>
		/// 获取数据源提供程序。
		/// </summary>
		IDataSourceProvider Provider
		{
			get;
		}

		/// <summary>
		/// 获取数据源选择程序。
		/// </summary>
		IDataSourceSelector Selector
		{
			get;
		}

		/// <summary>
		/// 根据当前数据访问上下文选取合适的数据源。
		/// </summary>
		/// <param name="context">指定的数据访问上下文。</param>
		/// <returns>返回对应的数据源。</returns>
		IDataSource GetSource(IDataAccessContextBase context);
	}
}
