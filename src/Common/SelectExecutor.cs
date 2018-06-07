/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class SelectExecutor : IDataExecutor<DataSelectionContext>
	{
		#region 单例字段
		public static readonly SelectExecutor Instance = new SelectExecutor();
		#endregion

		#region 公共方法
		public void Execute(DataSelectionContext context)
		{
			var provider = DataEnvironment.Providers.GetProvider(context);
			var statement = provider.Builder.Build(context);
			var command = provider.Scriptor.Command(statement, out var script);

			using(var connection = provider.CreateConnection())
			{
				//设置命令的数据连接
				command.Connection = connection;

				using(var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
				{
					reader.NextResult();
				}
			}
		}
		#endregion
	}
}
