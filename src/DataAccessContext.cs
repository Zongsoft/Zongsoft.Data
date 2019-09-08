﻿/*
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
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问上下文的接口。
	/// </summary>
	public interface IDataAccessContext : IDataAccessContextBase
	{
		/// <summary>
		/// 获取当前上下文对应的数据源。
		/// </summary>
		IDataSource Source
		{
			get;
		}

		/// <summary>
		/// 获取数据提供程序。
		/// </summary>
		IDataProvider Provider
		{
			get;
		}

		/// <summary>
		/// 获取当前上下文关联的数据会话。
		/// </summary>
		DataSession Session
		{
			get;
		}
	}

	/// <summary>
	/// 表示数据写入操作上下文的接口。
	/// </summary>
	public interface IDataMutateContext : IDataAccessContext, IDataMutateContextBase
	{
	}

	public class DataCountContext : DataCountContextBase, IDataAccessContext
	{
		#region 构造函数
		public DataCountContext(IDataAccess dataAccess, string name, ICondition condition, string member, object state = null) : base(dataAccess, name, condition, member, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataExistContext : DataExistContextBase, IDataAccessContext
	{
		#region 构造函数
		public DataExistContext(IDataAccess dataAccess, string name, ICondition condition, object state = null) : base(dataAccess, name, condition, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataExecuteContext : DataExecuteContextBase, IDataAccessContext
	{
		#region 构造函数
		public DataExecuteContext(IDataAccess dataAccess, string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters, object state = null) : base(dataAccess, name, isScalar, resultType, inParameters, outParameters, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataIncrementContext : DataIncrementContextBase, IDataMutateContext
	{
		#region 构造函数
		public DataIncrementContext(IDataAccess dataAccess, string name, string member, ICondition condition, int interval, object state = null) : base(dataAccess, name, member, condition, interval, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataDeleteContext : DataDeleteContextBase, IDataMutateContext
	{
		#region 构造函数
		public DataDeleteContext(IDataAccess dataAccess, string name, ICondition condition, ISchema schema, object state = null) : base(dataAccess, name, condition, schema, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public new Schema Schema
		{
			get
			{
				return (Schema)base.Schema;
			}
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataInsertContext : DataInsertContextBase, IDataMutateContext
	{
		#region 构造函数
		public DataInsertContext(IDataAccess dataAccess, string name, bool isMultiple, object data, ISchema schema, object state = null) : base(dataAccess, name, isMultiple, data, schema, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public new Schema Schema
		{
			get
			{
				return (Schema)base.Schema;
			}
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataUpdateContext : DataUpdateContextBase, IDataMutateContext
	{
		#region 构造函数
		public DataUpdateContext(IDataAccess dataAccess, string name, bool isMultiple, object data, ICondition condition, ISchema schema, object state = null) : base(dataAccess, name, isMultiple, data, condition, schema, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public new Schema Schema
		{
			get
			{
				return (Schema)base.Schema;
			}
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataUpsertContext : DataUpsertContextBase, IDataMutateContext
	{
		#region 构造函数
		public DataUpsertContext(IDataAccess dataAccess, string name, bool isMultiple, object data, ISchema schema, object state = null) : base(dataAccess, name, isMultiple, data, schema, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public new Schema Schema
		{
			get
			{
				return (Schema)base.Schema;
			}
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	public class DataSelectContext : DataSelectContextBase, IDataAccessContext
	{
		#region 构造函数
		public DataSelectContext(IDataAccess dataAccess, string name, Type entityType, Grouping grouping, ICondition condition, ISchema schema, Paging paging, Sorting[] sortings, object state = null) : base(dataAccess, name, entityType, grouping, condition, schema, paging, sortings, state)
		{
			this.Provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);
			this.Session = DataAccessContextUtility.GetSession(() => this.Provider.Multiplexer.GetSource(this));
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				return this.Session.Source;
			}
		}

		public IDataProvider Provider
		{
			get;
		}

		public new Schema Schema
		{
			get
			{
				return (Schema)base.Schema;
			}
		}

		public DataSession Session
		{
			get;
		}
		#endregion
	}

	internal static class DataAccessContextUtility
	{
		#region 公共方法
		public static DataSession GetSession(Func<IDataSource> sourceFactory)
		{
			var ambient = Zongsoft.Transactions.Transaction.Current;

			if(ambient == null || ambient.IsCompleted)
				return new DataSession(sourceFactory());
			else
				return (DataSession)ambient.Information.Parameters.GetOrAdd("Zongsoft.Data:DataSession", _ => new DataSession(sourceFactory(), ambient));
		}
		#endregion
	}
}
