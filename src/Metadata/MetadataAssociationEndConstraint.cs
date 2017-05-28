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

namespace Zongsoft.Data.Metadata
{
	public class MetadataAssociationEndConstraint : MetadataElementBase
	{
		#region 成员字段
		private string _propertyName;
		private string _value;
		private ConditionOperator _operator;
		#endregion

		#region 构造函数
		public MetadataAssociationEndConstraint(string propertyName, string value, ConditionOperator @operator = ConditionOperator.Equal)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName");

			if(string.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException("value");

			_propertyName = propertyName.Trim();
			_value = value.Trim();
			_operator = @operator;
		}
		#endregion

		#region 公共属性
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
		}

		public ConditionOperator Operator
		{
			get
			{
				return _operator;
			}
		}
		#endregion
	}
}
