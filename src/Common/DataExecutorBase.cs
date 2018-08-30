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

namespace Zongsoft.Data.Common
{
	public abstract class DataExecutorBase<TContext> : IDataExecutor<TContext> where TContext : IDataAccessContext
	{
		#region 事件声明
		public event EventHandler<DataExecutingEventArgs> Executing;
		public event EventHandler<DataExecutedEventArgs> Executed;
		#endregion

		#region 构造函数
		protected DataExecutorBase()
		{
		}
		#endregion

		#region 执行方法
		public virtual void Execute(TContext context)
		{
			//根据上下文生成对应执行语句
			var statements = this.Build(context);

			//激发“Executing”事件
			this.OnExecuting(context, statements);

			try
			{
				//调用具体的执行操作
				this.OnExecute(context, statements);
			}
			finally
			{
				foreach(var statement in statements)
				{
					statement.Source.ConnectionManager.Release(context);
				}
			}

			//激发“Executed”事件
			this.OnExecuted(context, statements);
		}
		#endregion

		#region 抽象方法
		protected abstract void OnExecute(TContext context, IEnumerable<Expressions.IStatement> statements);
		#endregion

		#region 构建语句
		protected virtual IEnumerable<Expressions.IStatement> Build(TContext context)
		{
			return context.Provider.Connector.GetSource(context).Build(context);
		}
		#endregion

		#region 激发事件
		protected virtual void OnExecuting(TContext context, IEnumerable<Expressions.IStatement> statements)
		{
			this.Executing?.Invoke(this, new DataExecutingEventArgs(context, statements));
		}

		protected virtual void OnExecuted(TContext context, IEnumerable<Expressions.IStatement> statements)
		{
			this.Executed?.Invoke(this, new DataExecutedEventArgs(context, statements));
		}
		#endregion
	}
}
