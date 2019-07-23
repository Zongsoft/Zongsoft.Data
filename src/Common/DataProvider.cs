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
using Zongsoft.Data.Metadata.Profiles;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataProvider : IDataProvider
	{
		#region 事件声明
		public event EventHandler<DataAccessErrorEventArgs> Error;
		public event EventHandler<DataAccessEventArgs<IDataAccessContext>> Executing;
		public event EventHandler<DataAccessEventArgs<IDataAccessContext>> Executed;
		#endregion

		#region 成员字段
		private IDataExecutor _executor;
		private IMetadataManager _metadata;
		private IDataMultiplexer _multiplexer;
		#endregion

		#region 构造函数
		public DataProvider(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();

			_executor = DataExecutor.Instance;
			_metadata = new MetadataFileManager(this.Name);
			_multiplexer = new DataMultiplexer(this.Name);
		}
		#endregion

		#region 公共属性
		public string Name { get; }

		public IDataExecutor Executor
		{
			get => _executor;
			set => _executor = value ?? throw new ArgumentNullException();
		}

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
		public void Execute(IDataAccessContext context)
		{
			//激发“Executing”事件
			this.OnExecuting(context);

			try
			{
				//进行具体的执行处理
				this.OnExecute(context);

				//尝试提交当前数据会话
				context.Session.Commit();
			}
			catch(Exception ex)
			{
				//尝试回滚当前数据会话
				context.Session.Rollback();

				//激发“Error”事件
				ex = this.OnError(context, ex);

				//重新抛出异常
				if(ex != null)
					throw new DataException("The data execution error has occurred.", ex);
			}

			//激发“Executed”事件
			this.OnExecuted(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnExecute(IDataAccessContext context)
		{
			//根据上下文生成对应执行语句集
			var statements = context.Source.Driver.Builder.Build(context);

			foreach(var statement in statements)
			{
				//由执行器执行语句
				_executor.Execute(context, statement);
			}
		}
		#endregion

		#region 激发事件
		protected virtual Exception OnError(IDataAccessContext context, Exception exception)
		{
			//通知数据驱动器发生了一个异常
			exception = context.Source.Driver.OnError(exception);

			//如果驱动器已经处理了异常则返回空
			if(exception == null)
				return null;

			var error = this.Error;

			if(error == null)
				return exception;

			//构建错误事件参数对象
			var args = new DataAccessErrorEventArgs(context, exception);

			//激发“Error”事件
			error(this, args);

			//如果异常处理已经完成则返回空，否则返回处理后的异常
			return args.ExceptionHandled ? null : args.Exception;
		}

		protected virtual void OnExecuting(IDataAccessContext context)
		{
			this.Executing?.Invoke(this, new DataAccessEventArgs(context));
		}

		protected virtual void OnExecuted(IDataAccessContext context)
		{
			this.Executed?.Invoke(this, new DataAccessEventArgs(context));
		}
		#endregion

		#region 嵌套子类
		private class DataAccessEventArgs : DataAccessEventArgs<IDataAccessContext>
		{
			public DataAccessEventArgs(IDataAccessContext context) : base(context)
			{
			}
		}

		private class DataExecutor : IDataExecutor
		{
			#region 单例字段
			public static readonly DataExecutor Instance = new DataExecutor();
			#endregion

			#region 成员字段
			private readonly DataSelectExecutor _select;
			private readonly DataDeleteExecutor _delete;
			private readonly DataInsertExecutor _insert;
			private readonly DataUpdateExecutor _update;
			private readonly DataUpsertExecutor _upsert;

			private readonly DataCountExecutor _count;
			private readonly DataExistExecutor _exist;
			private readonly DataExecuteExecutor _execution;
			#endregion

			#region 私有构造
			private DataExecutor()
			{
				_select = new DataSelectExecutor();
				_delete = new DataDeleteExecutor();
				_insert = new DataInsertExecutor();
				_update = new DataUpdateExecutor();
				_upsert = new DataUpsertExecutor();

				_count = new DataCountExecutor();
				_exist = new DataExistExecutor();
				_execution = new DataExecuteExecutor();
			}
			#endregion

			#region 执行方法
			public void Execute(IDataAccessContext context, IStatementBase statement)
			{
				switch(statement)
				{
					case SelectStatement select:
						_select.Execute(context, select);
						break;
					case DeleteStatement delete:
						_delete.Execute(context, delete);
						break;
					case InsertStatement insert:
						_insert.Execute(context, insert);
						break;
					case UpdateStatement update:
						_update.Execute(context, update);
						break;
					case UpsertStatement upsert:
						_upsert.Execute(context, upsert);
						break;
					case CountStatement count:
						_count.Execute(context, count);
						break;
					case ExistStatement exist:
						_exist.Execute(context, exist);
						break;
					case ExecutionStatement execution:
						_execution.Execute(context, execution);
						break;
					default:
						context.Session.Build(statement).ExecuteNonQuery();
						break;
				}
			}
			#endregion
		}

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
