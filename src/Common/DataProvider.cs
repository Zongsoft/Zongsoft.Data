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

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataProvider : IDataProvider
	{
		#region 成员字段
		private string _name;
		private IMetadataProviderManager _metadata;

		private IDataExecutor<DataSelectContext> _select;
		private IDataExecutor<DataDeleteContext> _delete;
		private IDataExecutor<DataInsertContext> _insert;
		private IDataExecutor<DataUpsertContext> _upsert;
		private IDataExecutor<DataUpdateContext> _update;
		#endregion

		#region 构造函数
		public DataProvider(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public IStatementBuilder Builder
		{
			get;
			protected set;
		}

		public IMetadataProviderManager Metadata
		{
			get
			{
				return _metadata;
			}
			protected set
			{
				_metadata = value ?? throw new ArgumentNullException();
			}
		}

		public ICollection<IDataSource> Sources
		{
			get;
		}

		public IDataSourceSelector Selector
		{
			get;
		}
		#endregion

		#region 执行方法
		public void Execute(DataAccessContextBase context)
		{
			this.OnExecute(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual IDataExecutor<TContext> CreateExecutor<TContext>(TContext context) where TContext : DataAccessContextBase
		{
			switch((DataAccessContextBase)context)
			{
				case DataSelectContext select:
					return (IDataExecutor<TContext>)new DataSelectExecutor();
				case DataDeleteContext delete:
					return (IDataExecutor<TContext>)new DataDeleteExecutor();
				case DataInsertContext insert:
					return (IDataExecutor<TContext>)new DataInsertExecutor();
				case DataUpsertContext upsert:
					return (IDataExecutor<TContext>)new DataUpsertExecutor();
				case DataUpdateContext update:
					return (IDataExecutor<TContext>)new DataUpdateExecutor();
				default:
					return null;
			}
		}

		protected virtual void OnExecute(DataAccessContextBase context)
		{
			switch(context)
			{
				case DataSelectContext select:
					this.GetExecutor(ref _select, context, ctx => this.CreateExecutor(ctx)).Execute(select);
					break;
				case DataDeleteContext delete:
					this.GetExecutor(ref _delete, context, ctx => this.CreateExecutor(ctx)).Execute(delete);
					break;
				case DataInsertContext insert:
					this.GetExecutor(ref _insert, context, ctx => this.CreateExecutor(ctx)).Execute(insert);
					break;
				case DataUpsertContext upsert:
					this.GetExecutor(ref _upsert, context, ctx => this.CreateExecutor(ctx)).Execute(upsert);
					break;
				case DataUpdateContext update:
					this.GetExecutor(ref _update, context, ctx => this.CreateExecutor(ctx)).Execute(update);
					break;
				default:
					throw new DataException("Invalid data access context.");
			}
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private IDataExecutor<TContext> GetExecutor<TContext>(ref IDataExecutor<TContext> executor, DataAccessContextBase context, Func<DataAccessContextBase, IDataExecutor<TContext>> factory) where TContext : DataAccessContextBase
		{
			if(executor == null)
				executor = factory(context) ?? throw new InvalidOperationException();

			return executor;
		}
		#endregion
	}
}
