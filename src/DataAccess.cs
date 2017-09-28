/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	public class DataAccess : DataAccessBase
	{
		#region 构造函数
		public DataAccess()
		{
		}

		public DataAccess(DataAccessEnvironment environment)
		{
		}
		#endregion

		#region 获取主键
		public override string[] GetKey(string name)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}
		#endregion

		#region 重写方法
		protected override DataCountContext CreateCountContext(string name, ICondition condition, string includes)
		{
			return base.CreateCountContext(name, condition, includes);
		}
		#endregion
	}
}
