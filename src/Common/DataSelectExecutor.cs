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
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataSelectExecutor : IDataExecutor<SelectStatement>
	{
		#region 执行方法
		public void Execute(IDataAccessContext context, SelectStatement statement)
		{
			if(context is DataSelectContext ctx)
				this.OnExecute(ctx, statement);
		}

		protected virtual void OnExecute(DataSelectContext context, SelectStatement statement)
		{
			var command = context.Build(statement);

			foreach(var parameter in statement.Parameters)
				parameter.Attach(command);

			context.Result = (IEnumerable)System.Activator.CreateInstance(typeof(LazyCollection<>).MakeGenericType(context.EntityType), new object[] { command });

			var member = Zongsoft.Reflection.MemberTokenProvider.Default.GetMember(context.EntityType, statement.Alias);

			if(statement.HasSlaves)
			{
				foreach(var slave in statement.Slaves)
				{
					//slave.Execute(context);
				}
			}
		}
		#endregion

		#region 嵌套子类
		public class LazyCollection<T> : IEnumerable<T>, IEnumerable
		{
			#region 成员变量
			private readonly DbCommand _command;
			#endregion

			#region 构造函数
			public LazyCollection(DbCommand command)
			{
				_command = command ?? throw new ArgumentNullException(nameof(command));

				if(command.Connection == null)
					throw new ArgumentException("Missing db-connection of the command.");
			}
			#endregion

			#region 遍历迭代
			public IEnumerator<T> GetEnumerator()
			{
				if(_command.Connection.State == ConnectionState.Closed)
					_command.Connection.Open();

				return new LazyIterator(_command.ExecuteReader());
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion

			#region 数据迭代
			private class LazyIterator : IEnumerator<T>, IDisposable
			{
				#region 成员变量
				private IDataReader _reader;
				private IDataPopulator _populator;
				#endregion

				#region 构造函数
				public LazyIterator(IDataReader reader)
				{
					_reader = reader;
					_populator = DataEnvironment.Populators.GetProvider(typeof(T)).GetPopulator(typeof(T), reader);
				}
				#endregion

				#region 公共成员
				public T Current
				{
					get
					{
						return (T)_populator.Populate(_reader);
					}
				}

				public bool MoveNext()
				{
					return _reader.Read();
				}

				public void Reset()
				{
					throw new NotSupportedException();
				}
				#endregion

				#region 显式实现
				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				void IDisposable.Dispose()
				{
					var reader = System.Threading.Interlocked.Exchange(ref _reader, null);

					if(reader != null)
						reader.Dispose();
				}
				#endregion
			}
			#endregion
		}
		#endregion
	}
}
