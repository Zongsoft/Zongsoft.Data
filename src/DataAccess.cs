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

using Zongsoft.Data.Common;

namespace Zongsoft.Data
{
	public class DataAccess : DataAccessBase
	{
		#region 成员字段
		private IDataProvider _provider;
		#endregion

		#region 构造函数
		public DataAccess(string name) : base(name)
		{
		}

		public DataAccess(string name, IEnumerable<IDataAccessFilter> filters) : base(name)
		{
			if(filters != null)
			{
				foreach(var filter in filters)
				{
					if(filter != null)
						this.Filters.Add(filter);
				}
			}
		}
		#endregion

		#region 公共属性
		public IDataProvider Provider
		{
			get
			{
				if(_provider == null)
					_provider = DataEnvironment.Providers.GetProvider(this.Name);

				return _provider;
			}
		}
		#endregion

		#region 获取主键
		public override string[] GetKey(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//获取指定名称的数据实体定义
			var entity = this.Provider.Metadata.Entities.Get(name);

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
		protected override void OnExecute<T>(DataExecuteContext context)
		{
			throw new NotImplementedException();
		}

		protected override void OnExecuteScalar(DataExecuteContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 存在方法
		protected override void OnExists(DataExistContext context)
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
		protected override void OnDelete(DataDeleteContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 插入方法
		protected override void OnInsert(DataInsertContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 更新方法
		protected override void OnUpdate(DataUpdateContext context)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 查询方法
		protected override void OnSelect<T>(DataSelectContext context)
		{
			this.Provider.Execute(context);
		}
		#endregion
	}
}
