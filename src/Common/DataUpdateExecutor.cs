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
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataUpdateExecutor : IDataExecutor<UpdateStatement>
	{
		#region 执行方法
		public void Execute(IDataAccessContext context, UpdateStatement statement)
		{
			if(context is DataUpdateContext ctx)
				this.OnExecute(ctx, statement);
		}

		protected virtual void OnExecute(DataUpdateContext context, UpdateStatement statement)
		{
			//保存初始的插入数据
			var data = context.Data;

			//根据生成的脚本创建对应的数据命令
			var command = context.Build(statement);

			//确保数据命令的连接被打开（注意：不用关闭数据连接，因为它可能关联了其他子事务）
			if(command.Connection.State == System.Data.ConnectionState.Closed)
				command.Connection.Open();

			if(context.IsMultiple)
			{
				foreach(var item in (IEnumerable)context.Data)
				{
					context.Data = item;
					statement.Bind(command, item);
					context.Count += command.ExecuteNonQuery();

					//如果有子句则执行子句操作
					if(statement.HasSlaves)
						this.Upsert(context, statement.Slaves);
				}
			}
			else
			{
				statement.Bind(command, context.Data);
				context.Count = command.ExecuteNonQuery();

				//如果有子句则执行子句操作
				if(statement.HasSlaves)
					this.Upsert(context, statement.Slaves);
			}

			//还原上下文的初始数据
			context.Data = data;
		}
		#endregion

		#region 私有方法
		private void Upsert(DataUpdateContext context, IEnumerable<IStatement> statements)
		{
			//保存子句集的上级数据
			var data = context.Data;

			foreach(var statement in statements)
			{
				if(statement is MutateStatement insertion)
				{
					//设置子新增语句中的关联参数值
					this.SetLinkedParameters(insertion, context.Data);

					//重新计算当前的操作数据
					context.Data = insertion.Schema.Token.GetValue(context.Data);
				}

				if(context.Data != null)
					context.Provider.Executor.Execute(context, statement);

				//还原数据上下文的数据
				context.Data = data;
			}
		}

		private void SetLinkedParameters(MutateStatement statement, object data)
		{
			if(statement.Schema == null || statement.Schema.Token.Property.IsSimplex)
				return;

			var complex = (IEntityComplexPropertyMetadata)statement.Schema.Token.Property;

			foreach(var link in complex.Links)
			{
				var parameter = statement.Parameters[link.Foreign.Name];
				parameter.Value = this.GetValue(data, link.Principal.Name);
			}
		}

		private object GetValue(object target, string name)
		{
			if(target is IDictionary<string, object> generic)
				return generic.TryGetValue(name, out var value) ? value : null;

			if(target is IDictionary classic)
				return classic.Contains(name) ? classic[name] : null;

			return Reflection.Reflector.GetValue(target, name);
		}
		#endregion
	}
}
