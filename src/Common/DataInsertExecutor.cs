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
using System.Collections.Generic;

namespace Zongsoft.Data.Common
{
	public class DataInsertExecutor : DataExecutorBase<DataInsertContext>
	{
		#region 单例字段
		public static readonly DataInsertExecutor Instance = new DataInsertExecutor();
		#endregion

		#region 执行方法
		protected override void OnExecute(DataInsertContext context, IEnumerable<Expressions.IStatement> statements)
		{
			foreach(var statement in statements)
			{
				//根据生成的脚本创建对应的数据命令
				var command = Expressions.StatementExtension.CreateCommand(statement, context);
				//var command = context.Source.Driver.CreateCommand(statement);

				//设置数据命令的连接对象
				//command.Connection = connection;

				//执行命令，并累加受影响的记录数
				context.Count += this.ExecuteCommand(command, statement, context.Data);
			}
		}
		#endregion

		private int ExecuteCommand(DbCommand command, Expressions.IStatement statement, object data)
		{
			this.Bind(statement, command, data);
			var count = command.ExecuteNonQuery();

			if(statement.HasSlaves)
			{
				foreach(var slave in statement.Slaves)
				{
					var insertStatement = slave as Expressions.InsertStatement;
					data = insertStatement.Schema.Token.GetValue(data);

					var slaveCommand = Expressions.StatementExtension.CreateCommand(slave);
					slaveCommand.Connection = command.Connection;
					slaveCommand.Transaction = command.Transaction;

					if(insertStatement.Schema.Token.IsMultiple)
					{
						var items = data as System.Collections.IEnumerable;

						foreach(var item in items)
						{
							count += this.ExecuteCommand(slaveCommand, slave, item);
						}
					}
					else
					{
						count += this.ExecuteCommand(slaveCommand, slave, data);
					}
				}
			}

			return count;
		}

		private void Bind(Expressions.IStatement statement, DbCommand command, object data)
		{
			if(!statement.HasParameters)
				return;

			foreach(var parameter in statement.Parameters)
			{
				var dbParameter = command.Parameters[parameter.Name];

				if(dbParameter.Direction == ParameterDirection.Input || dbParameter.Direction == ParameterDirection.InputOutput)
				{
					if(parameter.Schema == null)
						dbParameter.Value = parameter.Value;
					else
						dbParameter.Value = parameter.Schema.Token.GetValue(data);
				}
			}
		}
	}
}
