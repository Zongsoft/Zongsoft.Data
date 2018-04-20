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
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class DataAccess : DataAccessBase
	{
		#region 构造函数
		public DataAccess()
		{
		}
		#endregion

		#region 获取主键
		public override string[] GetKey(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//获取指定名称的数据实体定义
			var entity = DataEnvironment.Metadata.Entities.Get(name);

			if(entity == null)
				return null;

			//创建返回的主键成员名的数组
			var members = new string[entity.Key.Length];

			for(var i = 0; i < members.Length; i++)
			{
				members[i] = entity.Key[i].Name;
			}

			return members;
		}
		#endregion

		#region 执行方法
		protected override void OnExecute<T>(DataExecutionContext context)
		{
			throw new NotImplementedException();
		}

		protected override void OnExecuteScalar(DataExecutionContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 存在方法
		protected override void OnExists(DataExistenceContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 计数方法
		protected override void OnCount(DataCountContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 递增方法
		protected override void OnIncrement(DataIncrementContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 删除方法
		protected override void OnDelete(DataDeletionContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 插入方法
		protected override void OnInsert(DataInsertionContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 更新方法
		protected override void OnUpdate(DataUpdationContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 查询方法
		protected override void OnSelect<T>(DataSelectionContext context)
		{
			var provider = DataEnvironment.Providers.GetProvider(context);
			var builder = DataEnvironment.Builders.GetBuilder(context);
			var operation = builder.Build(context, provider);

			var scoping = Scoping.Parse(context.Scope);
			var members = scoping.Map(_ => provider.Metadata.Entities.Get(context.Name).Properties.Where(p => p.IsSimplex).Select(p => p.Name));

			operation.Execute(null);

			throw new NotImplementedException();
		}
		#endregion
	}
}
