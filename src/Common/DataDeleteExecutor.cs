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

using Zongsoft.Common;
using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataDeleteExecutor : IDataExecutor<DataDeleteContext>
	{
		#region 单例字段
		public static readonly DataDeleteExecutor Instance = new DataDeleteExecutor();
		#endregion

		#region 执行方法
		public void Execute(DataDeleteContext context)
		{
			//获取当前操作对应的数据源
			var source = context.Provider.Connector.GetSource(context);

			//根据上下文生成对应删除语句
			var statements = source.Build(context);

			foreach(var statement in statements)
			{
				//根据生成的脚本创建对应的数据命令
				var command = source.Driver.CreateCommand(statement);

				//设置数据命令的连接对象
				if(command.Connection == null)
					command.Connection = source.Driver.CreateConnection(source.ConnectionString);

				try
				{
					context.Count = command.ExecuteNonQuery();
				}
				finally
				{
					if(command.Connection != null)
						command.Connection.Dispose();
				}
			}
		}
		#endregion
	}
}
