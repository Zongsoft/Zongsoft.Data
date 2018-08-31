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

namespace Zongsoft.Data.Common.Expressions
{
	/// <summary>
	/// 表示语句的接口。
	/// </summary>
	public interface IStatement : IExpression
	{
		Schema Schema
		{
			get;
			set;
		}

		bool HasParameters
		{
			get;
		}

		Collections.INamedCollection<ParameterExpression> Parameters
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示当前语句是否有依附于自己的从属语句。
		/// </summary>
		bool HasSlaves
		{
			get;
		}

		/// <summary>
		/// 获取依附于当前语句的从属语句集合。
		/// </summary>
		/// <remarks>
		///		<para>对于只是获取从属语句的使用者，应先使用<see cref="HasSlaves"/>属性进行判断成功后再使用该属性，这样可避免创建不必要的集合对象。</para>
		/// </remarks>
		ICollection<IStatement> Slaves
		{
			get;
		}
	}
}
