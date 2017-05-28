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
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataInsertParameter : DataParameter
	{
		#region 成员字段
		private object _value;
		private string _scope;
		#endregion

		#region 构造函数
		public DataInsertParameter(string qualifiedName, object value, string scope) : base(qualifiedName)
		{
			_value = value;
			_scope = scope;
		}
		#endregion

		#region 公共属性
		public object Value
		{
			get
			{
				return _value;
			}
		}

		public IEnumerable<object> Values
		{
			get
			{
				if(_value != null && Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), _value.GetType()))
					return _value as IEnumerable<object>;

				return System.Linq.Enumerable.Empty<object>();
			}
		}

		public string Scope
		{
			get
			{
				return _scope;
			}
		}
		#endregion
	}
}
