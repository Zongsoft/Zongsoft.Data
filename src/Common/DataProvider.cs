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
		private string _name;
		private IDataConnector _connector;
		private IMetadataManager _metadata;

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

		public IDataConnector Connector
		{
			get
			{
				if(_connector == null)
					_connector = new DataConnector(this.Name);

				return _connector;
			}
			set
			{
				_connector = value ?? throw new ArgumentNullException();
			}
		}

		public IMetadataManager Metadata
		{
			get
			{
				if(_metadata == null)
					_metadata = new Metadata.Profiles.MetadataFileManager(this.Name);

				return _metadata;
			}
			set
			{
				_metadata = value ?? throw new ArgumentNullException();
			}
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
			switch(context.Method)
			{
				case DataAccessMethod.Select:
					return (IDataExecutor<TContext>)new DataSelectExecutor();
				case DataAccessMethod.Delete:
					return (IDataExecutor<TContext>)new DataDeleteExecutor();
				case DataAccessMethod.Insert:
					return (IDataExecutor<TContext>)new DataInsertExecutor();
				case DataAccessMethod.Upsert:
					return (IDataExecutor<TContext>)new DataUpsertExecutor();
				case DataAccessMethod.Update:
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
					this.GetExecutor(ref _select, select, ctx => this.CreateExecutor(ctx)).Execute(select);
					break;
				case DataDeleteContext delete:
					this.GetExecutor(ref _delete, delete, ctx => this.CreateExecutor(ctx)).Execute(delete);
					break;
				case DataInsertContext insert:
					this.GetExecutor(ref _insert, insert, ctx => this.CreateExecutor(ctx)).Execute(insert);
					break;
				case DataUpsertContext upsert:
					this.GetExecutor(ref _upsert, upsert, ctx => this.CreateExecutor(ctx)).Execute(upsert);
					break;
				case DataUpdateContext update:
					this.GetExecutor(ref _update, update, ctx => this.CreateExecutor(ctx)).Execute(update);
					break;
				default:
					throw new DataException("Invalid data access context.");
			}
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private IDataExecutor<TContext> GetExecutor<TContext>(ref IDataExecutor<TContext> executor, TContext context, Func<TContext, IDataExecutor<TContext>> factory) where TContext : DataAccessContextBase
		{
			if(executor == null)
				executor = factory(context) ?? throw new InvalidOperationException();

			return executor;
		}
		#endregion

		#region 嵌套子类
		private class DataConnector : IDataConnector
		{
			#region 成员字段
			private string _name;
			private ICollection<IDataSource> _sources;
			#endregion

			#region 构造函数
			public DataConnector(string name)
			{
				_name = name;
			}
			#endregion

			#region 公共属性
			public IDataSourceProvider Provider => DataSourceProvider.Default;

			public IDataSourceSelector Selector => DataSourceSelector.Default;
			#endregion

			#region 重写方法
			public IDataSource GetSource(DataAccessContextBase context)
			{
				if(this.EnsureSources())
					return this.Selector.GetSource(context, _sources);

				return null;
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
				if(_sources == null || _sources.Count == 0)
					_sources = new List<IDataSource>(this.Provider.GetSources(_name));

				return _sources.Count > 0;
			}
			#endregion
		}
		#endregion
	}
}
