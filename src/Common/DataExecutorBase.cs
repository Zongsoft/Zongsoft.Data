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
using System.Linq;
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
			var tokens = this.Build(context).ToArray();

			//激发“Executing”事件
			this.OnExecuting(context, tokens);

			try
			{
				//调用具体的执行操作
				this.OnExecute(context, tokens);
			}
			finally
			{
				foreach(var token in tokens)
				{
					token.Source.ConnectionManager.Release(context);
				}
			}

			//激发“Executed”事件
			this.OnExecuted(context, tokens);
		}
		#endregion

		#region 抽象方法
		protected abstract void OnExecute(TContext context, IEnumerable<StatementToken> tokens);
		#endregion

		#region 构建语句
		protected virtual IEnumerable<StatementToken> Build(TContext context)
		{
			var source = context.Provider.Connector.GetSource(context);

			foreach(var statement in source.Build(context))
			{
				yield return new StatementToken(source, statement);
			}
		}
		#endregion

		#region 激发事件
		protected virtual void OnExecuting(TContext context, IEnumerable<StatementToken> tokens)
		{
			this.Executing?.Invoke(this, new DataExecutingEventArgs(context, tokens.Select(p => p.Statement)));
		}

		protected virtual void OnExecuted(TContext context, IEnumerable<StatementToken> tokens)
		{
			this.Executed?.Invoke(this, new DataExecutedEventArgs(context, tokens.Select(p => p.Statement)));
		}
		#endregion

		#region 嵌套子类
		public struct StatementToken
		{
			#region 公共字段
			public readonly IDataSource Source;
			public readonly Expressions.IStatement Statement;
			#endregion

			#region 构造函数
			public StatementToken(IDataSource source, Expressions.IStatement statement)
			{
				this.Source = source;
				this.Statement = statement;
			}
			#endregion

			#region 公共方法
			public StatementToken Create(Expressions.IStatement statement)
			{
				return new StatementToken(this.Source, statement);
			}

			public System.Data.Common.DbCommand CreateCommand(IDataAccessContextBase context = null)
			{
				var command = this.Source.Driver.CreateCommand(this.Statement);

				if(context != null)
					command.Connection = this.Source.ConnectionManager.Get(context);

				return command;
			}
			#endregion
		}
		#endregion
	}
}
