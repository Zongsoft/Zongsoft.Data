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

namespace Zongsoft.Data.Common
{
	/// <summary>
	/// 表示数据源的接口。
	/// </summary>
	public interface IDataSource
	{
		/// <summary>
		/// 获取数据源的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取数据源的连接字符串。
		/// </summary>
		string ConnectionString
		{
			get;
		}

		/// <summary>
		/// 获取数据连接管理池。
		/// </summary>
		ConnectionPool ConnectionManager
		{
			get;
		}

		/// <summary>
		/// 获取数据源支持的访问方式。
		/// </summary>
		DataAccessMode Mode
		{
			get;
			set;
		}

		/// <summary>
		/// 获取数据源关联的数据驱动器。
		/// </summary>
		IDataDriver Driver
		{
			get;
		}
	}
}
