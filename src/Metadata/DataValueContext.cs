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

namespace Zongsoft.Data.Metadata
{
	public class DataValueContext : IDataValueContext
	{
		#region 成员字段
		private readonly Func<object, Type, object> _valueFactory;
		#endregion

		#region 构造函数
		public DataValueContext(IDataMutateContext context, DataAccessMethod method, IDataEntitySimplexProperty property, object data, Func<object, Type, object> valueFacotry)
		{
			this.Context = context;
			this.Method = method;
			this.Property = property;
			this.Data = data;
			_valueFactory = valueFacotry;
		}
		#endregion

		#region 公共属性
		public IDataMutateContextBase Context
		{
			get;
		}

		public DataAccessMethod Method
		{
			get;
		}

		public IDataEntitySimplexProperty Property
		{
			get;
		}

		public object Data
		{
			get;
		}

		public object GetValue(Type conversionType = null)
		{
			return _valueFactory(this.Data, conversionType);
		}
		#endregion
	}
}
