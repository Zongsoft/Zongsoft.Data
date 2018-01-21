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
using System.Collections;

namespace Zongsoft.Data.Common
{
	/// <summary>
	/// 提供数据实体装配的接口。
	/// </summary>
	public interface IDataPopulator
	{
		/// <summary>
		/// 获取装配的数据实体类型。
		/// </summary>
		Type EntityType
		{
			get;
		}

		/// <summary>
		/// 获取或设置数据实体实例的创建器。
		/// </summary>
		IDataEntityCreator EntityCreator
		{
			get;
			set;
		}

		/// <summary>
		/// 数据实体装配方法。
		/// </summary>
		/// <param name="reader">装配的数据读取器。</param>
		/// <param name="context">数据操作上下文对象。</param>
		/// <returns>返回装配成功的数据实体集。</returns>
		IEnumerable Populate(IDataReader reader, DataAccessContextBase context);
	}
}
