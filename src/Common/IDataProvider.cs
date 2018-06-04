/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
		/// 获取数据驱动程序的名称。
		/// </summary>
		string DriverName
		{
			get;
		}

		/// <summary>
		/// 获取或设置连接字符串内容。
		/// </summary>
		string ConnectionString
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置数据提供程序支持的访问模式。
		/// </summary>
		DataAccessMode AccessMode
		{
			get;
			set;
		}

		/// <summary>
		/// 获取数据提供程序关联的元数据集。
		/// </summary>
		Metadata.IMetadataProvider Metadata
		{
			get;
		}

		/// <summary>
		/// 获取语句创建器。
		/// </summary>
		Expressions.IStatementBuilder Builder
		{
			get;
		}

		/// <summary>
		/// 获取脚本生成器。
		/// </summary>
		Expressions.IStatementScriptor Scriptor
		{
			get;
		}

		/// <summary>
		/// 创建一个数据命令对象。
		/// </summary>
		/// <returns>返回创建的数据命令对象。</returns>
		IDbCommand CreateCommand();

		/// <summary>
		/// 创建一个数据连接对象。
		/// </summary>
		/// <returns>返回创建的数据连接对象，该连接对象的连接字符串为<see cref="ConnectionString"/>属性值。</returns>
		IDbConnection CreateConnection();
	}
}
