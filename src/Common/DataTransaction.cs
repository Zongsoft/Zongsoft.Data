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
	/// <summary>
	/// 透明事务（伪事务），即不提供事务操作。
	/// </summary>
	public class TransparentTransaction : IDataTransaction, IDisposable
	{
		#region 成员字段
		private IDataSource _source;
		#endregion

		#region 构造函数
		public TransparentTransaction(IDataSource source)
		{
			_source = source;
		}
		#endregion

		#region 公共属性
		public bool InAmbient
		{
			get => false;
		}

		public IDataSource Source
		{
			get => _source;
		}
		#endregion

		#region 公共方法
		public void Bind(IDbCommand command)
		{
			if(command == null)
				throw new ArgumentNullException(nameof(command));

			//始终创建一个新的数据库连接对象
			command.Connection = _source.Driver.CreateConnection(_source.ConnectionString);
		}

		public void Commit()
		{
		}

		public void Rollback()
		{
		}

		public void Dispose()
		{
		}
		#endregion
	}

	/// <summary>
	/// 独立事务，即每个数据访问对应一个单独的事务。
	/// </summary>
	public class IndependentTransaction : IDataTransaction, IDisposable
	{
		#region 成员字段
		private IDataSource _source;
		private DbConnection _connection;
		#endregion

		#region 构造函数
		public IndependentTransaction(IDataSource source)
		{
			_source = source;
		}
		#endregion

		#region 公共属性
		public bool InAmbient
		{
			get => false;
		}

		public IDataSource Source
		{
			get => _source;
		}
		#endregion

		#region 公共方法
		public void Bind(IDbCommand command)
		{
			if(command == null)
				throw new ArgumentNullException(nameof(command));

			if(_connection == null)
			{
				lock(this)
				{
					if(_connection == null)
					{
						_connection = _source.Driver.CreateConnection(_source.ConnectionString);
					}
				}
			}

			//设置命令的数据连接对象
			command.Connection = _connection;
		}

		public void Commit()
		{
			var connection = _connection;

			if(connection != null)
				connection.Dispose();
		}

		public void Rollback()
		{
			var connection = _connection;

			if(connection != null)
				connection.Dispose();
		}

		public void Dispose()
		{
			var connection = _connection;

			if(connection != null)
				connection.Dispose();
		}
		#endregion
	}

	/// <summary>
	/// 环境事务，环境中所有数据访问操作均共享相同的数据连接及事务。
	/// </summary>
	public class AmbientTransaction : IDataTransaction, IDisposable
	{
		#region 常量定义
		private const int COMPLETED_FLAG = 1;
		#endregion

		#region 私有变量
		private int _flag;
		private readonly AutoResetEvent _semaphore;
		private readonly ConcurrentBag<IDbCommand> _commands;
		#endregion

		#region 成员字段
		private IDataSource _source;
		private DbConnection _connection;
		private DbTransaction _transaction;
		private IsolationLevel _isolationLevel;
		private Transactions.Transaction _ambient;
		#endregion

		#region 构造函数
		internal AmbientTransaction(IDataSource source, Zongsoft.Transactions.Transaction ambient)
		{
			_source = source ?? throw new ArgumentNullException(nameof(source));
			_ambient = ambient ?? throw new ArgumentNullException(nameof(ambient));

			_semaphore = new AutoResetEvent(true);
			_commands = new ConcurrentBag<IDbCommand>();
			_isolationLevel = GetIsolationLevel(ambient.IsolationLevel);
		}
		#endregion

		#region 公共属性
		public bool InAmbient
		{
			get => true;
		}

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
		public void Bind(IDbCommand command)
		{
			if(command == null)
				throw new ArgumentNullException(nameof(command));

			//如果已经终止则返回
			if(_flag == COMPLETED_FLAG || _ambient.IsCompleted)
				throw new DataException("The ambient transaction have been completed.");

			//等待信号量
			_semaphore.WaitOne();

			try
			{
				//如果已经终止则返回
				if(_flag == COMPLETED_FLAG || _ambient.IsCompleted)
					throw new DataException("The ambient transaction have been completed.");

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
			}
			finally
			{
				//释放当前持有的信号
				_semaphore.Set();
			}
		}

		public void Commit()
		{
		}

		public void Rollback()
		{
		}

		public void Dispose()
		{
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
					transaction.Rollback();

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

			//释放信号量资源
			_semaphore.Dispose();
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
				_transaction = connection.BeginTransaction(_isolationLevel);

				//依次设置待绑定命令的事务
				while(_commands.TryTake(out var command))
				{
					command.Transaction = _transaction;
				}

				_ambient.Enlist(new Enlistment(_transaction));
			}
		}
		#endregion

		#region 私有方法
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
			private readonly DbTransaction _transaction;

			public Enlistment(DbTransaction transaction)
			{
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

				//释放数据连接
				_transaction.Connection.Dispose();
				_transaction.Dispose();
			}
		}
		#endregion
	}
}
