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
	public class ParameterExpressionCollection : Collections.NamedCollectionBase<ParameterExpression>
	{
		#region 私有变量
		private int _index;
		#endregion

		#region 公共方法
		public ParameterExpression Add(string name, object value = null, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(string.IsNullOrEmpty(name) || name == "?")
			{
				var index = System.Threading.Interlocked.Increment(ref _index);
				name = "p" + index.ToString();
			}

			var parameter = Expression.Parameter(name, value, direction);
			base.AddItem(parameter);
			return parameter;
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(ParameterExpression item)
		{
			return item.Name;
		}

		protected override void AddItem(ParameterExpression item)
		{
			if(string.IsNullOrEmpty(item.Name) || item.Name == "?")
			{
				var index = System.Threading.Interlocked.Increment(ref _index);
				item.Name = "p" + index.ToString();
			}

			base.AddItem(item);
		}
		#endregion
	}
}
