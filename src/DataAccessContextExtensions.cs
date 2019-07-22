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
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data
{
	public static class DataAccessContextExtension
	{
		/// <summary>
		/// 创建语句对应的 <see cref="DbCommand"/> 数据命令。
		/// </summary>
		/// <param name="context">指定的数据访问上下文。</param>
		/// <param name="statement">指定要创建命令的语句。</param>
		/// <returns>返回创建的数据命令。</returns>
		public static DbCommand Build(this IDataAccessContext context, IStatementBase statement)
		{
			//创建指定语句的数据命令对象
			var command = context.Source.Driver.CreateCommand(statement);

			//设置数据命令关联的数据库连接及上下文事务
			context.Transaction.Bind(command);

			//返回数据命令对象
			return command;
		}

		private class AmbientCommand : DbCommand
		{
			#region 成员字段
			private readonly DbCommand _command;
			private AmbientTransaction _ambient;
			#endregion

			#region 构造函数
			internal AmbientCommand(DbCommand command)
			{
				_command = command ?? throw new ArgumentNullException(nameof(command));
			}

			internal AmbientCommand(DbCommand command, AmbientTransaction ambient)
			{
				_command = command ?? throw new ArgumentNullException(nameof(command));
				_ambient = ambient ?? throw new ArgumentNullException(nameof(ambient));
			}
			#endregion

			#region 公共属性
			public AmbientTransaction Ambient
			{
				get => _ambient;
				set => _ambient = value ?? throw new ArgumentNullException();
			}
			#endregion

			#region 重写属性
			public override string CommandText
			{
				get => _command.CommandText;
				set => _command.CommandText = value;
			}

			public override CommandType CommandType
			{
				get => _command.CommandType;
				set => _command.CommandType = value;
			}

			public override int CommandTimeout
			{
				get => _command.CommandTimeout;
				set => _command.CommandTimeout = value;
			}

			protected override DbConnection DbConnection
			{
				get => _command.Connection;
				set => _command.Connection = value;
			}

			protected override DbTransaction DbTransaction
			{
				get => _command.Transaction;
				set => _command.Transaction = value;
			}

			protected override DbParameterCollection DbParameterCollection
			{
				get => _command.Parameters;
			}

			public override bool DesignTimeVisible
			{
				get => _command.DesignTimeVisible;
				set => _command.DesignTimeVisible = value;
			}

			public override UpdateRowSource UpdatedRowSource
			{
				get => _command.UpdatedRowSource;
				set => _command.UpdatedRowSource = value;
			}
			#endregion

			#region 重写方法
			public override void Cancel()
			{
				_command.Cancel();
			}

			public override void Prepare()
			{
				_command.Prepare();
			}

			protected override DbParameter CreateDbParameter()
			{
				return _command.CreateParameter();
			}

			public override object ExecuteScalar()
			{
				//确认数据连接已打开
				this.EnsureConnect();

				//返回数据命令执行结果
				return _command.ExecuteScalar();
			}

			public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
			{
				//确认数据连接已打开
				await this.EnsureConnectAsync(cancellationToken);

				//返回数据命令执行结果
				return await _command.ExecuteScalarAsync(cancellationToken);
			}

			public override int ExecuteNonQuery()
			{
				//确认数据连接已打开
				this.EnsureConnect();

				//返回数据命令执行结果
				return _command.ExecuteNonQuery();
			}

			public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
			{
				//确认数据连接已打开
				await this.EnsureConnectAsync(cancellationToken);

				//返回数据命令执行结果
				return await _command.ExecuteNonQueryAsync(cancellationToken);
			}

			protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
			{
				//尝试进入环境事务的读取临界区
				//如果进入成功，则表明该事务支持多活动结果集(MARS)，或者当前操作是环境事务中的首个读取请求
				if(_ambient.EnterRead())
				{
					//确认数据连接已打开
					this.EnsureConnect();

					//执行数据命令的读取方法，并构建一个环境数据读取器。注意：该读取器在关闭时会退出读取临界区。
					return new AmbientReader(_command.ExecuteReader(behavior), _ambient);
				}

				//读取临界区进入失败：创建一个新的数据连接给当前命令
				_command.Connection = _ambient.Source.Driver.CreateConnection(_ambient.Connection.ConnectionString);
				_command.Connection.Open();

				return _command.ExecuteReader(behavior | CommandBehavior.CloseConnection);
			}

			protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
			{
				//尝试进入环境事务的读取临界区
				//如果进入成功，则表明该事务支持多活动结果集(MARS)，或者当前操作是环境事务中的首个读取请求
				if(_ambient.EnterRead())
				{
					//确认数据连接已打开
					await this.EnsureConnectAsync(cancellationToken);

					//执行数据命令的读取方法，并构建一个环境数据读取器。注意：该读取器在关闭时会退出读取临界区。
					return new AmbientReader(await _command.ExecuteReaderAsync(behavior, cancellationToken), _ambient);
				}

				//读取临界区进入失败：创建一个新的数据连接给当前命令
				_command.Connection = _ambient.Source.Driver.CreateConnection(_ambient.Connection.ConnectionString);
				await _command.Connection.OpenAsync();

				return await _command.ExecuteReaderAsync(behavior | CommandBehavior.CloseConnection, cancellationToken);
			}
			#endregion

			#region 私有方法
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private void EnsureConnect()
			{
				if(_command.Connection.State == ConnectionState.Closed || _command.Connection.State == ConnectionState.Broken)
					_command.Connection.Open();
			}

			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private Task EnsureConnectAsync(CancellationToken cancellationToken)
			{
				if(_command.Connection.State == ConnectionState.Closed || _command.Connection.State == ConnectionState.Broken)
					return _command.Connection.OpenAsync(cancellationToken);

				return Task.CompletedTask;
			}
			#endregion
		}

		private class AmbientReader : DbDataReader
		{
			#region 成员字段
			private readonly DbDataReader _reader;
			private readonly AmbientTransaction _ambient;
			#endregion

			#region 构造函数
			public AmbientReader(DbDataReader reader, AmbientTransaction ambient)
			{
				_reader = reader;
				_ambient = ambient;
			}
			#endregion

			#region 重写属性
			public override object this[int ordinal] => _reader[ordinal];

			public override object this[string name] => _reader[name];

			public override int Depth => _reader.Depth;

			public override int FieldCount => _reader.FieldCount;

			public override bool HasRows => _reader.HasRows;

			public override bool IsClosed => _reader.IsClosed;

			public override int RecordsAffected => _reader.RecordsAffected;
			#endregion

			#region 重写方法
			public override bool GetBoolean(int ordinal)
			{
				return _reader.GetBoolean(ordinal);
			}

			public override byte GetByte(int ordinal)
			{
				return _reader.GetByte(ordinal);
			}

			public override long GetBytes(int ordinal, long offset, byte[] buffer, int bufferOffset, int length)
			{
				return _reader.GetBytes(ordinal, offset, buffer, bufferOffset, length);
			}

			public override char GetChar(int ordinal)
			{
				return _reader.GetChar(ordinal);
			}

			public override long GetChars(int ordinal, long offset, char[] buffer, int bufferOffset, int length)
			{
				return _reader.GetChars(ordinal, offset, buffer, bufferOffset, length);
			}

			public override DateTime GetDateTime(int ordinal)
			{
				return _reader.GetDateTime(ordinal);
			}

			public override decimal GetDecimal(int ordinal)
			{
				return _reader.GetDecimal(ordinal);
			}

			public override double GetDouble(int ordinal)
			{
				return _reader.GetDouble(ordinal);
			}

			public override float GetFloat(int ordinal)
			{
				return _reader.GetFloat(ordinal);
			}

			public override Guid GetGuid(int ordinal)
			{
				return _reader.GetGuid(ordinal);
			}

			public override short GetInt16(int ordinal)
			{
				return _reader.GetInt16(ordinal);
			}

			public override int GetInt32(int ordinal)
			{
				return _reader.GetInt32(ordinal);
			}

			public override long GetInt64(int ordinal)
			{
				return _reader.GetInt64(ordinal);
			}

			public override string GetString(int ordinal)
			{
				return _reader.GetString(ordinal);
			}

			public override object GetValue(int ordinal)
			{
				return _reader.GetValue(ordinal);
			}

			public override int GetValues(object[] values)
			{
				return _reader.GetValues(values);
			}

			public override string GetName(int ordinal)
			{
				return _reader.GetName(ordinal);
			}

			public override int GetOrdinal(string name)
			{
				return _reader.GetOrdinal(name);
			}

			public override Type GetFieldType(int ordinal)
			{
				return _reader.GetFieldType(ordinal);
			}

			public override string GetDataTypeName(int ordinal)
			{
				return _reader.GetDataTypeName(ordinal);
			}

			public override IEnumerator GetEnumerator()
			{
				return _reader.GetEnumerator();
			}

			public override bool IsDBNull(int ordinal)
			{
				return _reader.IsDBNull(ordinal);
			}

			public override bool NextResult()
			{
				return _reader.NextResult();
			}

			public override bool Read()
			{
				return _reader.Read();
			}

			public override void Close()
			{
				//关闭数据读取器
				_reader.Close();

				//退出环境事务的读取临界区（即重置环境事务的读取标记）
				var disconnectRequired = _ambient.ExitRead();

				/*
				 * 如果环境事务已经完结，则需要关闭释放对应的数据连接。
				 * 因为当事务完成(提交或回滚)时，如果该事务正处于读取状态(即位于读取临界区内)，
				 * 完成操作是不会关闭数据连接的，因为读取操作还需要使用它，即该数据连接由读取器进行关闭。
				 */
				if(disconnectRequired && _ambient.IsCompleted)
				{
					var connection = _ambient.Connection;

					if(connection != null)
						connection.Dispose();
				}
			}
			#endregion
		}
	}
}
