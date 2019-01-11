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
	public abstract class StatementVisitorBase<TStatement> : IStatementVisitor<TStatement> where TStatement : IStatement
	{
		#region 构造函数
		protected StatementVisitorBase()
		{
		}
		#endregion

		#region 公共方法
		public void Visit(IExpressionVisitor visitor, TStatement statement)
		{
			//通知当前语句开始访问
			this.OnVisiting(visitor, statement);

			//调用具体的访问方法
			this.OnVisit(visitor, statement);

			//通知当前语句访问完成
			this.OnVisited(visitor, statement);
		}
		#endregion

		#region 抽象方法
		protected abstract void OnVisit(IExpressionVisitor visitor, TStatement statement);
		#endregion

		#region 虚拟方法
		protected virtual void OnVisiting(IExpressionVisitor visitor, TStatement statement)
		{
		}

		protected virtual void OnVisited(IExpressionVisitor visitor, TStatement statement)
		{
		}
		#endregion
	}
}
