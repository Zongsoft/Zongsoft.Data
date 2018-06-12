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
using System.Data;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Common;
using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public class DataSelectExecutor : IDataExecutor<DataSelectContext>
	{
		#region 单例字段
		public static readonly DataSelectExecutor Instance = new DataSelectExecutor();
		#endregion

		#region 执行方法
		public void Execute(DataSelectContext context)
		{
			var provider = context.GetProvider();
			var source = provider.Connector.GetSource(context);
			var statement = (SelectStatement)provider.Builder.Build(context);

			//根据生成的脚本创建对应的数据命令
			var command = source.Driver.CreateCommand(statement);

			//设置数据命令的连接对象
			if(command.Connection != null)
				command.Connection = source.Driver.CreateConnection();

			if(statement.HasSlaves)
			{
				context.Result = this.LoadResult(command, statement, context.ElementType, context.GetEntity());
			}
			else
			{
				context.Result = new LazyCollection(command, context.ElementType, this.GetPopulator);
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual IDataPopulator GetPopulator(Type type, IDataReader reader)
		{
			var provider = DataEnvironment.Populators.GetProvider(type);

			if(provider == null)
				throw new DataException($"Missing the data populator provider for the '{type.FullName}' type.");

			return provider.GetPopulator(type, reader);
		}
		#endregion

		#region 私有方法
		private IEnumerable LoadResult(IDbCommand command, SelectStatement statement, Type type, IEntity entity)
		{
			IList list = null;
			IDictionary<string, IList> slaveResults = null;
			IDataPopulator populator = null;

			try
			{
				command.Connection.Open();

				using(var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
				{
					populator = this.GetPopulator(type, reader);
					list = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

					while(reader.Read())
					{
						var item = populator.Populate(reader);

						if(item != null)
							list.Add(item);
					}

					slaveResults = new Dictionary<string, IList>();

					var slaves = statement.Slaves.GetEnumerator();

					while(reader.NextResult() && slaves.MoveNext())
					{
						var slave = slaves.Current;
						var member = EntityMemberProvider.Default.GetMember(type, slave.Slaver.Name);
						var elementType = TypeExtension.GetElementType(member.Type);
						var elements = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

						populator = this.GetPopulator(elementType, reader);

						while(reader.Read())
						{
							var element = populator.Populate(reader);

							if(element != null)
								elements.Add(element);
						}

						slaveResults.Add(slave.Slaver.Name, elements);
					}
				}
			}
			finally
			{
				if(command.Connection != null)
					command.Connection.Dispose();
			}

			if(list != null && slaveResults != null)
			{
				foreach(var slave in slaveResults)
				{
					entity.Properties.Find(slave.Key, (path, e, p) =>
					{
					});
				}

				foreach(var item in list)
				{

				}
			}

			return list ?? Zongsoft.Collections.Enumerable.Empty(type);
		}
		#endregion

		#region 嵌套子类
		public class LazyCollection : IEnumerable
		{
			private Type _type;
			private IDbCommand _command;
			private Func<Type, IDataReader, IDataPopulator> _populatorThunk;

			public LazyCollection(IDbCommand command, Type type, Func<Type, IDataReader, IDataPopulator> populatorThunk)
			{
				_type = type;
				_command = command;
				_populatorThunk = populatorThunk;
			}

			public IEnumerator GetEnumerator()
			{
				_command.Connection.Open();
				var reader = _command.ExecuteReader(CommandBehavior.CloseConnection);
				var populator = _populatorThunk.Invoke(_type, reader);

				return new LazyIterator(reader, populator);
			}

			private class LazyIterator : IEnumerator, IDisposable
			{
				private IDataReader _reader;
				private IDataPopulator _populator;

				public LazyIterator(IDataReader reader, IDataPopulator populator)
				{
					_reader = reader;
					_populator = populator;
				}

				public object Current
				{
					get
					{
						return _populator.Populate(_reader);
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

				public void Dispose()
				{
					var reader = System.Threading.Interlocked.Exchange(ref _reader, null);

					if(reader != null)
					{
						_reader.Dispose();

						if(_populator != null && _populator is IDisposable disposable)
							disposable.Dispose();
					}
				}
			}
		}
		#endregion
	}
}
