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
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	public abstract class DataMutateExecutor<TStatement> : IDataExecutor<TStatement> where TStatement : IMutateStatement
	{
		#region 执行方法
		public void Execute(IDataAccessContext context, TStatement statement)
		{
			if(context is IDataMutateContext ctx)
				this.OnExecute(ctx, statement);
		}

		protected virtual void OnExecute(IDataMutateContext context, TStatement statement)
		{
			//保存初始的操作数据
			var data = context.Data;

			//根据生成的脚本创建对应的数据命令
			var command = context.Session.Build(statement);

			var count = 0;
			var keeping = true;
			var isMultiple = context.IsMultiple;

			if(statement.Schema != null)
				isMultiple = statement.Schema.Token.IsMultiple;

			if(isMultiple && context.Data != null)
			{
				foreach(var item in (IEnumerable)context.Data)
				{
					//更新当前操作数据
					context.Data = item;

					//调用写入操作开始方法
					this.OnMutating(context, statement);

					//绑定命令参数
					statement.Bind(command, item);

					//执行数据命令操作
					count = command.ExecuteNonQuery();

					//累加总受影响的记录数
					context.Count += count;

					//调用写入操作完成方法
					keeping = this.OnMutated(context, statement, count);

					//如果需要继续并且有子句则执行子句操作
					if(keeping && statement.HasSlaves)
						this.Mutate(context, statement.Slaves);
				}
			}
			else
			{
				//调用写入操作开始方法
				this.OnMutating(context, statement);

				//绑定命令参数
				statement.Bind(command, context.Data);

				//执行数据命令操作
				count = command.ExecuteNonQuery();

				//累加总受影响的记录数
				context.Count += count;

				//调用写入操作完成方法
				keeping = this.OnMutated(context, statement, count);

				//如果需要继续并且有子句则执行子句操作
				if(keeping && statement.HasSlaves)
					this.Mutate(context, statement.Slaves);
			}

			//还原上下文的初始数据
			context.Data = data;
		}
		#endregion

		#region 写入操作
		protected virtual bool OnMutated(IDataMutateContext context, TStatement statement, int count)
		{
			return count > 0;
		}

		protected virtual void OnMutating(IDataMutateContext context, TStatement statement)
		{
		}
		#endregion

		#region 私有方法
		private void Mutate(IDataMutateContext context, IEnumerable<IStatementBase> statements)
		{
			//保存子句集的上级数据
			var data = context.Data;

			foreach(var statement in statements)
			{
				if(statement is IMutateStatement mutation)
				{
					//设置子新增语句中的关联参数值
					this.SetLinkedParameters(mutation, context.Data);

					//重新计算当前的操作数据
					context.Data = mutation.Schema.Token.GetValue(context.Data);
				}

				//通过内部执行器执行从属语句
				context.Provider.Executor.Execute(context, statement);

				//还原数据上下文的数据
				context.Data = data;
			}
		}

		private void SetLinkedParameters(IMutateStatement statement, object data)
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
