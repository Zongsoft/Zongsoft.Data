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
		public ParameterExpression Add(string name, System.Data.DbType type, System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input)
		{
			var parameter = Expression.Parameter(this.GetParameterName(name), type, direction);
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
			//处理下参数名为空或问号(?)的情况
			item.Name = this.GetParameterName(item.Name);

			//调用基类同名方法
			base.AddItem(item);
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetParameterName(string name)
		{
			if(string.IsNullOrEmpty(name) || name == "?")
			{
				var index = System.Threading.Interlocked.Increment(ref _index);
				return "p" + index.ToString();
			}

			return name;
		}
		#endregion
	}
}
