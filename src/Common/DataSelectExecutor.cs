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
			switch(context)
			{
				case DataSelectContext selection:
					this.OnExecute(selection, statement);
					break;
				case DataInsertContext insertion:
					this.OnExecute(insertion, statement);
					break;
				case DataIncrementContext increment:
					this.OnExecute(increment, statement);
					break;
			}
		}

		protected virtual void OnExecute(DataSelectContext context, SelectStatement statement)
		{
			//根据生成的脚本创建对应的数据命令
			var command = context.Build(statement);

			context.Result = CreateResults(context.EntityType, context, statement, command, !context.Transaction.InAmbient, null);
		}

		protected virtual void OnExecute(DataInsertContext context, SelectStatement statement)
		{
			//根据生成的脚本创建对应的数据命令
			var command = context.Build(statement);

			//确保数据命令的连接被打开（注意：不用关闭数据连接，因为它可能关联了其他子事务）
			if(command.Connection.State == System.Data.ConnectionState.Closed)
				command.Connection.Open();

			using(var reader = command.ExecuteReader())
			{
				if(reader.Read())
				{
					for(int i = 0; i < reader.FieldCount; i++)
					{
						var schema = string.IsNullOrEmpty(statement.Alias) ? context.Schema.Find(reader.GetName(i)) : context.Schema.Find(statement.Alias);

						if(schema != null)
							schema.Token.SetValue(context.Data, reader.GetValue(i));
					}
				}
			}
		}

		protected virtual void OnExecute(DataIncrementContext context, SelectStatement statement)
		{
			//根据生成的脚本创建对应的数据命令
			var command = context.Build(statement);

			//确保数据命令的连接被打开（注意：不用关闭数据连接，因为它可能关联了其他子事务）
			if(command.Connection.State == System.Data.ConnectionState.Closed)
				command.Connection.Open();

			//执行命令
			context.Result = Zongsoft.Common.Convert.ConvertValue<long>(command.ExecuteScalar());
		}
		#endregion

		#region 私有方法
		private static IEnumerable CreateResults(Type elementType, DataSelectContext context, SelectStatement statement, DbCommand command, bool closeConnection, Action<string, Paging> paginator)
		{
			return (IEnumerable)System.Activator.CreateInstance(typeof(LazyCollection<>).MakeGenericType(elementType), new object[] { context, statement, command, closeConnection, paginator });
		}
		#endregion

		#region 嵌套子类
		public class LazyCollection<T> : IEnumerable<T>, IEnumerable, IPaginator
		{
			#region 公共事件
			public event EventHandler<PagingEventArgs> Paginated;
			#endregion

			#region 成员变量
			private readonly CommandBehavior _behavior;
			private readonly DbCommand _command;
			private readonly DataSelectContext _context;
			private readonly SelectStatement _statement;
			private readonly Action<string, Paging> _paginate;
			#endregion

			#region 构造函数
			public LazyCollection(DataSelectContext context, SelectStatement statement, DbCommand command, bool closeConnection, Action<string, Paging> paginate)
			{
				_context = context ?? throw new ArgumentNullException(nameof(context));
				_statement = statement ?? throw new ArgumentNullException(nameof(statement));
				_command = command ?? throw new ArgumentNullException(nameof(command));
				_behavior = closeConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default;

				if(paginate != null)
					_paginate = paginate;
				else
					_paginate = (key, paging) => this.OnPaginated(key, paging);
			}
			#endregion

			#region 遍历迭代
			public IEnumerator<T> GetEnumerator()
			{
				if(_command.Connection.State == ConnectionState.Closed)
					_command.Connection.Open();

				return new LazyIterator(_context, _statement, _command.ExecuteReader(_behavior), _paginate);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion

			#region 激发事件
			protected virtual void OnPaginated(string key, Paging paging)
			{
				this.Paginated?.Invoke(this, new PagingEventArgs(key, paging));
			}
			#endregion

			#region 数据迭代
			private class LazyIterator : IEnumerator<T>, IDisposable
			{
				#region 成员变量
				private IDataReader _reader;
				private Action<string, Paging> _paginate;
				private readonly IDataPopulator _populator;
				private readonly DataSelectContext _context;
				private readonly SelectStatement _statement;
				private readonly IDictionary<string, SlaveToken> _slaves;
				#endregion

				#region 构造函数
				public LazyIterator(DataSelectContext context, SelectStatement statement, IDataReader reader, Action<string, Paging> paginate)
				{
					var entity = context.Entity;

					if(!string.IsNullOrEmpty(statement.Alias))
						entity = ((IEntityComplexPropertyMetadata)context.Entity.Find(statement.Alias)).Foreign;

					_context = context;
					_statement = statement;
					_reader = reader;
					_paginate = paginate;
					_slaves = GetSlaves(_statement, _reader);
					_populator = DataEnvironment.Populators.GetProvider(typeof(T)).GetPopulator(entity, typeof(T), _reader);
				}
				#endregion

				#region 公共成员
				public T Current
				{
					get
					{
						var entity = _populator.Populate(_reader);

						if(entity == null)
							return default(T);

						if(_statement.HasSlaves)
						{
							foreach(var slave in _statement.Slaves)
							{
								if(slave is SelectStatement selection && _slaves.TryGetValue(selection.Alias, out var token))
								{
									if(token.Schema.Token.MemberType == null)
										continue;

									//生成子查询语句对应的命令
									var command = _context.Build(slave);

									foreach(var parameter in token.Parameters)
									{
										command.Parameters[parameter.Name].Value = _reader.GetValue(parameter.Ordinal);
									}

									//创建一个新的查询结果集
									var results = CreateResults(Zongsoft.Common.TypeExtension.GetElementType(token.Schema.Token.MemberType), _context, selection, command, true, _paginate);

									//如果要设置的目标成员类型是一个数组或者集合，则需要将动态的查询结果集转换为固定的列表
									if(Zongsoft.Common.TypeExtension.IsCollection(token.Schema.Token.MemberType))
									{
										var list = Activator.CreateInstance(
											typeof(List<>).MakeGenericType(Zongsoft.Common.TypeExtension.GetElementType(token.Schema.Token.MemberType)),
											new object[] { results });

										if(token.Schema.Token.MemberType.IsArray)
											results = (IEnumerable)list.GetType().GetMethod("ToArray").Invoke(list, new object[0]);
										else
											results = (IEnumerable)list;
									}

									token.Schema.Token.SetValue(entity, results);
								}
							}
						}

						return (T)entity;
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

				#region 私有方法
				private IDictionary<string, SlaveToken> GetSlaves(IStatementBase statement, IDataReader reader)
				{
					IEnumerable<ParameterToken> GetParameters(string path)
					{
						if(string.IsNullOrEmpty(path))
							yield break;

						for(int i = 0; i < reader.FieldCount; i++)
						{
							var name = reader.GetName(i);

							if(name.StartsWith("$" + path + ":"))
								yield return new ParameterToken(name.Substring(path.Length + 2), i);
						}
					}

					if(statement.HasSlaves)
					{
						var tokens = new Dictionary<string, SlaveToken>(statement.Slaves.Count);

						foreach(var slave in statement.Slaves)
						{
							if(slave is SelectStatementBase selection && !string.IsNullOrEmpty(selection.Alias))
							{
								var schema = _context.Schema.Find(selection.Alias);

								if(schema != null)
									tokens.Add(selection.Alias, new SlaveToken(schema, GetParameters(selection.Alias)));
							}
						}

						if(tokens.Count > 0)
							return tokens;
					}

					return null;
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
					{
						//处理分页的总记录数
						if(_statement.Paging != null && _statement.Paging.PageSize > 0)
						{
							if(reader.NextResult() && reader.Read())
							{
								_statement.Paging.TotalCount = reader.GetInt64(0);
								_paginate?.Invoke(_statement.Alias, _statement.Paging);
							}
						}

						reader.Dispose();
					}
				}
				#endregion
			}
			#endregion
		}

		private struct SlaveToken
		{
			public SchemaMember Schema;
			public IEnumerable<ParameterToken> Parameters;

			public SlaveToken(SchemaMember schema, IEnumerable<ParameterToken> parameters)
			{
				this.Schema = schema;
				this.Parameters = parameters;
			}
		}

		private struct ParameterToken
		{
			public readonly string Name;
			public readonly int Ordinal;

			public ParameterToken(string name, int ordinal)
			{
				this.Name = name;
				this.Ordinal = ordinal;
			}
		}
		#endregion
	}
}
