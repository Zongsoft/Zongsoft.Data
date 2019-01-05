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
		protected override void OnExecute(DataExecuteContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 存在方法
		protected override void OnExists(DataExistContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 计数方法
		protected override void OnCount(DataCountContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 递增方法
		protected override void OnIncrement(DataIncrementContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 删除方法
		protected override void OnDelete(DataDeleteContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 插入方法
		protected override void OnInsert(DataInsertContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 更新方法
		protected override void OnUpdate(DataUpdateContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 查询方法
		protected override void OnSelect(DataSelectContextBase context)
		{
			this.Provider.Execute(context);
		}
		#endregion

		#region 模式解析
		protected override ISchemaParser CreateSchema()
		{
			return new SchemaParser(this.Provider);
		}
		#endregion

		#region 上下文法
		protected override DataCountContextBase CreateCountContext(string name, ICondition condition, string member, object state)
		{
			return new DataCountContext(this, name, condition, member, state);
		}

		protected override DataExistContextBase CreateExistContext(string name, ICondition condition, object state)
		{
			return new DataExistContext(this, name, condition, state);
		}

		protected override DataExecuteContextBase CreateExecuteContext(string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, object state)
		{
			return new DataExecuteContext(this, name, isScalar, resultType, inParameters, null, state);
		}

		protected override DataIncrementContextBase CreateIncrementContext(string name, string member, ICondition condition, int interval, object state)
		{
			return new DataIncrementContext(this, name, member, condition, interval, state);
		}

		protected override DataDeleteContextBase CreateDeleteContext(string name, ICondition condition, ISchema schema, object state)
		{
			return new DataDeleteContext(this, name, condition, schema, state);
		}

		protected override DataInsertContextBase CreateInsertContext(string name, bool isMultiple, object data, ISchema schema, object state)
		{
			//if(isMultiple)
			//	data = GetDataDictionaries(data);
			//else
			//	data = GetDataDictionary(data);

			return new DataInsertContext(this, name, isMultiple, data, schema, state);
		}

		protected override DataUpsertContextBase CreateUpsertContext(string name, bool isMultiple, object data, ISchema schema, object state)
		{
			//if(isMultiple)
			//	data = GetDataDictionaries(data);
			//else
			//	data = GetDataDictionary(data);

			return new DataUpsertContext(this, name, isMultiple, data, schema, state);
		}

		protected override DataUpdateContextBase CreateUpdateContext(string name, bool isMultiple, object data, ICondition condition, ISchema schema, object state)
		{
			//if(isMultiple)
			//	data = GetDataDictionaries(data);
			//else
			//	data = GetDataDictionary(data);

			return new DataUpdateContext(this, name, isMultiple, data, condition, schema, state);
		}

		protected override DataSelectContextBase CreateSelectContext(string name, Type entityType, ICondition condition, Grouping grouping, ISchema schema, Paging paging, Sorting[] sortings, object state)
		{
			return new DataSelectContext(this, name, entityType, grouping, condition, schema, paging, sortings, state);
		}
		#endregion
	}
}
