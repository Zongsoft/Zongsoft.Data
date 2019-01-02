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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common
{
	public class DataProvider : IDataProvider
	{
		#region 成员字段
		private IMetadataManager _metadata;
		private IDataMultiplexer _multiplexer;

		private IDataExecutor<DataSelectContext> _select;
		private IDataExecutor<DataDeleteContext> _delete;
		private IDataExecutor<DataInsertContext> _insert;
		private IDataExecutor<DataUpdateContext> _update;
		private IDataExecutor<DataUpsertContext> _upsert;

		private IDataExecutor<DataCountContext> _count;
		private IDataExecutor<DataExistContext> _exist;
		private IDataExecutor<DataExecuteContext> _execute;
		private IDataExecutor<DataIncrementContext> _increment;
		#endregion

		#region 构造函数
		public DataProvider(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();

			_metadata = new Metadata.Profiles.MetadataFileManager(this.Name);
			_multiplexer = new DataMultiplexer(this.Name);
		}
		#endregion

		#region 公共属性
		public string Name { get; }

		public IMetadataManager Metadata
		{
			get => _metadata;
			set => _metadata = value ?? throw new ArgumentNullException();
		}

		public IDataMultiplexer Multiplexer
		{
			get => _multiplexer;
			set => _multiplexer = value ?? throw new ArgumentNullException();
		}
		#endregion

		#region 执行方法
		public void Execute(IDataAccessContextBase context)
		{
			this.OnExecute(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnExecute(IDataAccessContextBase context)
		{
			switch(context)
			{
				case DataSelectContext select:
					this.GetExecutor(ref _select, select, ctx => this.CreateExecutor(ctx)).Execute(select);
					break;
				case DataDeleteContext delete:
					this.GetExecutor(ref _delete, delete, ctx => this.CreateExecutor(ctx)).Execute(delete);
					break;
				case DataInsertContext insert:
					this.GetExecutor(ref _insert, insert, ctx => this.CreateExecutor(ctx)).Execute(insert);
					break;
				case DataUpdateContext update:
					this.GetExecutor(ref _update, update, ctx => this.CreateExecutor(ctx)).Execute(update);
					break;
				case DataUpsertContext upsert:
					this.GetExecutor(ref _upsert, upsert, ctx => this.CreateExecutor(ctx)).Execute(upsert);
					break;
				case DataCountContext count:
					this.GetExecutor(ref _count, count, ctx => this.CreateExecutor(ctx)).Execute(count);
					break;
				case DataExistContext exist:
					this.GetExecutor(ref _exist, exist, ctx => this.CreateExecutor(ctx)).Execute(exist);
					break;
				case DataExecuteContext execute:
					this.GetExecutor(ref _execute, execute, ctx => this.CreateExecutor(ctx)).Execute(execute);
					break;
				case DataIncrementContext increment:
					this.GetExecutor(ref _increment, increment, ctx => this.CreateExecutor(ctx)).Execute(increment);
					break;
				default:
					throw new DataException("Invalid data access context.");
			}
		}

		protected virtual IDataExecutor<TContext> CreateExecutor<TContext>(TContext context) where TContext : IDataAccessContext
		{
			switch(context.Method)
			{
				case DataAccessMethod.Select:
					return (IDataExecutor<TContext>)new DataSelectExecutor();
				case DataAccessMethod.Delete:
					return (IDataExecutor<TContext>)new DataDeleteExecutor();
				case DataAccessMethod.Insert:
					return (IDataExecutor<TContext>)new DataInsertExecutor();
				case DataAccessMethod.Update:
					return (IDataExecutor<TContext>)new DataUpdateExecutor();
				case DataAccessMethod.Upsert:
					return (IDataExecutor<TContext>)new DataUpsertExecutor();
				case DataAccessMethod.Count:
					return (IDataExecutor<TContext>)new DataCountExecutor();
				case DataAccessMethod.Exists:
					return (IDataExecutor<TContext>)new DataExistExecutor();
				case DataAccessMethod.Execute:
					return (IDataExecutor<TContext>)new DataExecuteExecutor();
				case DataAccessMethod.Increment:
					return (IDataExecutor<TContext>)new DataIncrementExecutor();
				default:
					return null;
			}
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private IDataExecutor<TContext> GetExecutor<TContext>(ref IDataExecutor<TContext> executor, TContext context, Func<TContext, IDataExecutor<TContext>> factory) where TContext : IDataAccessContext
		{
			if(executor == null)
				executor = factory(context) ?? throw new InvalidOperationException();

			return executor;
		}
		#endregion

		#region 嵌套子类
		private class DataMultiplexer : IDataMultiplexer
		{
			#region 成员字段
			private string _name;
			private List<IDataSource> _sources;
			#endregion

			#region 构造函数
			public DataMultiplexer(string name)
			{
				_name = name;
			}
			#endregion

			#region 公共属性
			public IDataSourceProvider Provider => DataSourceProvider.Default;
			public IDataSourceSelector Selector => DataSourceSelector.Default;
			#endregion

			#region 重写方法
			public IDataSource GetSource(IDataAccessContextBase context)
			{
				if(this.EnsureSources())
					return this.Selector.GetSource(context, _sources) ?? throw new DataException("No matched data source for this data operation.");

				throw new DataException($"No data sources for the '{_name}' data provider was found.");
			}
			#endregion

			#region 枚举遍历
			public IEnumerator<IDataSource> GetEnumerator()
			{
				if(this.EnsureSources())
					return _sources.GetEnumerator();
				else
					return Enumerable.Empty<IDataSource>().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				if(this.EnsureSources())
					return _sources.GetEnumerator();
				else
					return Enumerable.Empty<IDataSource>().GetEnumerator();
			}
			#endregion

			#region 私有方法
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private bool EnsureSources()
			{
				if(_sources == null)
					_sources = new List<IDataSource>(this.Provider.GetSources(_name));
				else if(_sources.Count == 0)
					_sources.AddRange(this.Provider.GetSources(_name));

				return _sources.Count > 0;
			}
			#endregion
		}
		#endregion
	}
}
