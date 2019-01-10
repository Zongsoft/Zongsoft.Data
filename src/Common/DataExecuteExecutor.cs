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

using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataExecuteExecutor : IDataExecutor<ExecutionStatement>
	{
		#region 执行方法
		public void Execute(IDataAccessContext context, ExecutionStatement statement)
		{
			if(context is DataExecuteContext ctx)
				this.OnExecute(ctx, statement);
		}

		protected virtual void OnExecute(DataExecuteContext context, ExecutionStatement statement)
		{
			//根据生成的脚本创建对应的数据命令
			var command = context.Build(statement, true);

			if(context.IsScalar)
			{
				context.Result = command.ExecuteScalar();
				return;
			}

			using(var reader = command.ExecuteReader())
			{
				context.Result = this.Populate(reader, context.ResultType);
			}
		}
		#endregion

		#region 私有方法
		private System.Collections.IEnumerable Populate(IDataReader reader, Type entityType)
		{
			return null;
		}
		#endregion
	}
}
