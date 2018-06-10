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
		/// <summary>
		/// 获取数据提供程序的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取数据提供程序的元数据。
		/// </summary>
		Metadata.IMetadataProviderManager Metadata
		{
			get;
		}

		ICollection<IDataSource> Sources
		{
			get;
		}

		IDataSourceSelector Selector
		{
			get;
		}

		Expressions.IStatementBuilder Builder
		{
			get;
		}

		/// <summary>
		/// 执行数据操作。
		/// </summary>
		/// <param name="context">数据操作的上下文。</param>
		void Execute(DataAccessContextBase context);
	}
}
