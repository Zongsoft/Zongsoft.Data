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
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataInsertExecutor : IDataExecutor<InsertStatement>
	{
		#region 执行方法
		public void Execute(IDataAccessContext context, InsertStatement statement)
		{
			if(context is DataInsertContext ctx)
				this.OnExecute(ctx, statement);
		}

		protected virtual void OnExecute(DataInsertContext context, InsertStatement statement)
		{
			//保存初始的插入数据
			var data = context.Data;

			//执行具体的插入命令
			context.Count = this.Insert(context, statement, context.IsMultiple);

			//还原上下文的初始数据
			context.Data = data;
		}
		#endregion

		#region 私有方法
		private int Insert(DataInsertContext context, InsertStatement statement, bool isMultiple)
		{
			//根据生成的脚本创建对应的数据命令
			var command = context.Build(statement);

			//确保数据命令的连接被打开（注意：不用关闭数据连接，因为它可能关联了其他子事务）
			if(command.Connection.State == System.Data.ConnectionState.Closed)
				command.Connection.Open();

			int count = 0;

			if(isMultiple)
			{
				foreach(var item in (IEnumerable)context.Data)
				{
					context.Data = item;
					statement.Bind(command, item);
					count += command.ExecuteNonQuery();

					//执行获取新增后的自增型字段值
					if(statement.Sequence != null)
						context.Provider.Executor.Execute(context, statement.Sequence);

					//如果有子句则执行子句操作
					if(statement.HasSlaves)
						this.Insert(context, statement.Slaves);
				}
			}
			else
			{
				statement.Bind(command, context.Data);
				count = command.ExecuteNonQuery();

				//执行获取新增后的自增型字段值
				if(statement.Sequence != null)
					context.Provider.Executor.Execute(context, statement.Sequence);

				//如果有子句则执行子句操作
				if(statement.HasSlaves)
					this.Insert(context, statement.Slaves);
			}

			return count;
		}

		private void Insert(DataInsertContext context, IEnumerable<IStatement> statements)
		{
			foreach(var statement in statements)
			{
				if(statement is InsertStatement insertion)
				{
					context.Data = insertion.Schema.Token.GetValue(context.Data);
					this.Insert(context, insertion, insertion.Schema.Token.IsMultiple);
				}
				else
				{
					context.Provider.Executor.Execute(context, statement);
				}
			}
		}
		#endregion
	}
}
