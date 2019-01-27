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
using System.Data;
using System.Data.Common;
using System.Collections.Concurrent;
using System.Threading;

namespace Zongsoft.Data.Common
{
	public class Transaction : IDbTransaction, IDisposable
	{
		#region 常量定义
		private const int COMPLETED_FLAG = 1;
		#endregion

		#region 私有变量
		private int _flag;
		private bool _isLazy;
		private readonly AutoResetEvent _semaphore;
		private readonly ConcurrentBag<DbCommand> _commands;
		#endregion

		#region 成员字段
		private IDataSource _source;
		private DbConnection _connection;
		private DbTransaction _transaction;
		private IsolationLevel _isolationLevel;
		private Transactions.Transaction _ambient;
		#endregion

		#region 构造函数
		internal Transaction(IDataSource source, Zongsoft.Transactions.Transaction ambient = null)
		{
			_source = source ?? throw new ArgumentNullException(nameof(source));
			_commands = new ConcurrentBag<DbCommand>();
			_semaphore = new AutoResetEvent(true);
			_ambient = ambient;
			_isolationLevel = ambient == null ? IsolationLevel.RepeatableRead : GetIsolationLevel(ambient.IsolationLevel);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get => _source;
		}

		public IDbConnection Connection
		{
			get => _connection;
		}

		public IsolationLevel IsolationLevel
		{
			get => _isolationLevel;
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 设置指定命令的数据连接和关联到当前事务。
		/// </summary>
		/// <param name="command">指定要绑定的命令对象。</param>
		/// <param name="requiresMultipleResults">指示要设置的命令是否为多活动结果集之用。</param>
		/// <returns>如果当前事务未终结（提交或回滚）则返回真(True)；否则返回假(False)。</returns>
		public bool Bind(DbCommand command, bool requiresMultipleResults)
		{
			if(command == null)
				throw new ArgumentNullException(nameof(command));

			//如果已经终止则返回
			if(_flag == COMPLETED_FLAG)
				return false;

			//等待信号量
			_semaphore.WaitOne();

			try
			{
				//如果已经终止则返回
				if(_flag == COMPLETED_FLAG)
					return false;

				//处理多活动结果集(MARS)的数据连接
				if(requiresMultipleResults && !_source.Features.Support(Feature.MultipleActiveResultSets))
				{
					command.Connection = _source.Driver.CreateConnection(_source.ConnectionString);
					command.Transaction = _transaction;

					return true;
				}

				if(_connection == null)
				{
					lock(_semaphore)
					{
						if(_connection == null)
						{
							_connection = _source.Driver.CreateConnection(_source.ConnectionString);
							_connection.StateChange += Connection_StateChange;
						}
					}
				}

				//设置命令的数据连接对象
				command.Connection = _connection;

				//如果当前事务已启动则更新命令否则将命令加入到待绑定集合中
				if(_transaction == null)
					_commands.Add(command); //将指定的命令加入到命令集，等待事务绑定
				else
					command.Transaction = _transaction;

				return true;
			}
			finally
			{
				//释放当前持有的信号
				_semaphore.Set();
			}
		}

		public void Commit()
		{
			this.Done(true);
		}

		public void Rollback()
		{
			this.Done(false);
		}

		public void Dispose()
		{
			this.Done(false);

			//释放信号量资源
			_semaphore.Dispose();
		}

		private void Done(bool isCommit)
		{
			/*
			 * 如果是环境事务或当前事务为延迟读取，则返回。
			 * 
			 * 原因：1)环境的事务和释放操作均待由环境事务的回调中再处理；
			 * 　　　2)延迟读取表示连接由延迟集合进行关闭和释放，且无需事务操作。
			 */
			if(_ambient != null || _isLazy)
				return;

			//设置完成标记
			var flag = Interlocked.Exchange(ref _flag, COMPLETED_FLAG);

			//如果已经完成过则返回
			if(flag == COMPLETED_FLAG)
				return;

			//等待信号量
			_semaphore.WaitOne();

			try
			{
				//获取并重置当前数据库事务对象
				var transaction = Interlocked.Exchange(ref _transaction, null);

				if(transaction != null)
				{
					if(isCommit)
						transaction.Commit();
					else
						transaction.Rollback();
				}

				//将数据连接对象置空
				var connection = Interlocked.Exchange(ref _connection, null);

				if(connection != null)
				{
					//取消连接事件处理
					connection.StateChange -= Connection_StateChange;

					//释放数据连接
					connection.Dispose();
				}
			}
			finally
			{
				//释放当前持有的信号
				_semaphore.Set();
			}
		}
		#endregion

		#region 内部方法
		internal void Lazy()
		{
			_isLazy = true;
		}
		#endregion

		#region 事件响应
		private void Connection_StateChange(object sender, StateChangeEventArgs e)
		{
			var connection = (DbConnection)sender;

			if(e.CurrentState == ConnectionState.Open)
			{
				//取消连接事件处理
				connection.StateChange -= Connection_StateChange;

				//开启一个数据库事务
				var transaction = connection.BeginTransaction(_isolationLevel);

				//依次设置待绑定命令的事务
				while(_commands.TryTake(out var command))
				{
					command.Transaction = transaction;
				}

				if(_ambient == null)
					_transaction = transaction;
				else
					_ambient.Enlist(new Enlistment(transaction, ref _isLazy));
			}
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static string GetTransactionKey(string identity)
		{
			return $"Zongsoft.Data:{identity}:Transaction";
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static IsolationLevel GetIsolationLevel(Zongsoft.Transactions.IsolationLevel level)
		{
			switch(level)
			{
				case Transactions.IsolationLevel.ReadCommitted:
					return IsolationLevel.ReadCommitted;
				case Transactions.IsolationLevel.ReadUncommitted:
					return IsolationLevel.ReadUncommitted;
				case Transactions.IsolationLevel.RepeatableRead:
					return IsolationLevel.RepeatableRead;
				case Transactions.IsolationLevel.Serializable:
					return IsolationLevel.Serializable;
			}

			return IsolationLevel.ReadCommitted;
		}
		#endregion

		#region 嵌套子类
		private class Enlistment : Zongsoft.Transactions.IEnlistment
		{
			private readonly bool _isLazy;
			private readonly DbTransaction _transaction;

			public Enlistment(DbTransaction transaction, ref bool isLazy)
			{
				_isLazy = isLazy;
				_transaction = transaction;
			}

			public void OnEnlist(Zongsoft.Transactions.EnlistmentContext context)
			{
				if(context.Phase == Zongsoft.Transactions.EnlistmentPhase.Prepare)
					return;

				switch(context.Phase)
				{
					case Transactions.EnlistmentPhase.Commit:
						_transaction.Commit();
						break;
					case Transactions.EnlistmentPhase.Abort:
					case Transactions.EnlistmentPhase.Rollback:
						_transaction.Rollback();
						break;
				}

				//如果不用延迟释放，则立即释放数据连接
				if(!_isLazy)
				{
					_transaction.Connection.Dispose();
					_transaction.Dispose();
				}
			}
		}
		#endregion
	}

	public class TransparentTransaction : DbTransaction
	{
		private IDataSource _source;
		private DbConnection _connection;

		public override IsolationLevel IsolationLevel
		{
			get => IsolationLevel.RepeatableRead;
		}

		protected override DbConnection DbConnection
		{
			get
			{
				if(_connection == null)
				{
					lock(this)
					{
						if(_connection == null)
							_connection = _source.Driver.CreateConnection(_source.ConnectionString);
					}
				}

				return _connection;
			}
		}

		public override void Commit()
		{
		}

		public override void Rollback()
		{
		}
	}

	public class OneselfTransaction
	{
	}

	public class AmbientTransaction
	{
	}
}
