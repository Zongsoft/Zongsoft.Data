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
	public class DataInsertExecutor : DataExecutorBase<DataInsertContext>
	{
		#region 单例字段
		public static readonly DataInsertExecutor Instance = new DataInsertExecutor();
		#endregion

		#region 执行方法
		protected override void OnExecute(DataInsertContext context, IEnumerable<IStatement> statements)
		{
			foreach(var statement in statements)
			{
				//根据生成的脚本创建对应的数据命令
				var command = context.Build(statement);

				if(context.IsMultiple)
				{
					foreach(var item in (IEnumerable)context.Data)
					{
						//执行命令，并累加受影响的记录数
						context.Count += this.ExecuteCommand(context, statement, item);
					}
				}
				else
				{
					//执行命令，并累加受影响的记录数
					context.Count += this.ExecuteCommand(context, statement, context.Data);
				}
			}
		}
		#endregion

		private int ExecuteCommand(DataInsertContext context, IStatement statement, object data)
		{
			//根据生成的脚本创建对应的数据命令
			var command = context.Build(statement);

			if(command.Connection.State == ConnectionState.Closed)
				command.Connection.Open();

			//绑定参数
			statement.Bind(command, data);

			var count = command.ExecuteNonQuery();

			if(statement.HasSlaves)
			{
				foreach(var slave in statement.Slaves)
				{
					data = slave.Schema.Token.GetValue(data);

					if(slave.Schema.Token.IsMultiple)
					{
						foreach(var item in (IEnumerable)data)
						{
							count += this.ExecuteCommand(context, slave, item);
						}
					}
					else
					{
						count += this.ExecuteCommand(context, slave, data);
					}
				}
			}

			return count;
		}
	}
}
