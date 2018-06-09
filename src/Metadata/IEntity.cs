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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体的元数据类。
	/// </summary>
	public interface IEntity : IEquatable<IEntity>
	{
		#region 属性声明
		/// <summary>
		/// 获取元数据所属的提供程序。
		/// </summary>
		IMetadataProvider Provider
		{
			get;
		}

		/// <summary>
		/// 获取数据实体的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取数据实体映射的别名（表名）。
		/// </summary>
		string Alias
		{
			get;
		}

		/// <summary>
		/// 获取数据实体的主键属性数组。
		/// </summary>
		IEntitySimplexProperty[] Key
		{
			get;
		}

		/// <summary>
		/// 获取数据实体继承的父实体名。
		/// </summary>
		string BaseName
		{
			get;
		}

		/// <summary>
		/// 获取数据实体的属性元数据集合。
		/// </summary>
		IEntityPropertyCollection Properties
		{
			get;
		}
		#endregion
	}
}
