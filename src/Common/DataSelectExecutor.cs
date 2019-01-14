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

			context.Result = CreateResults(context.EntityType, context, statement, command);
		}
		#endregion

		#region 私有方法
		private static IEnumerable CreateResults(Type elementType, DataSelectContext context, SelectStatement statement, DbCommand command)
		{
			return (IEnumerable)System.Activator.CreateInstance(typeof(LazyCollection<>).MakeGenericType(elementType), new object[] { context, statement, command });
		}
		#endregion

		#region 嵌套子类
		public class LazyCollection<T> : IEnumerable<T>, IEnumerable
		{
			#region 成员变量
			private readonly DbCommand _command;
			private readonly DataSelectContext _context;
			private readonly SelectStatement _statement;
			#endregion

			#region 构造函数
			public LazyCollection(DataSelectContext context, SelectStatement statement, DbCommand command)
			{
				_context = context ?? throw new ArgumentNullException(nameof(context));
				_statement = statement ?? throw new ArgumentNullException(nameof(statement));
				_command = command ?? throw new ArgumentNullException(nameof(command));
			}
			#endregion

			#region 遍历迭代
			public IEnumerator<T> GetEnumerator()
			{
				if(_command.Connection.State == ConnectionState.Closed)
					_command.Connection.Open();

				return new LazyIterator(_context, _statement, _command.ExecuteReader(CommandBehavior.CloseConnection));
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
				private readonly IDataPopulator _populator;
				private readonly DataSelectContext _context;
				private readonly SelectStatement _statement;
				private readonly IDictionary<string, SlaveToken> _slaves;
				#endregion

				#region 构造函数
				public LazyIterator(DataSelectContext context, SelectStatement statement, IDataReader reader)
				{
					var entity = context.Entity;

					if(!string.IsNullOrEmpty(statement.Alias))
						entity = context.Entity.Find(statement.Alias).Entity;

					_context = context;
					_statement = statement;
					_reader = reader;
					_slaves = GetSlaves(_statement, _reader);
					_populator = DataEnvironment.Populators.GetProvider(typeof(T)).GetPopulator(entity, typeof(T), _reader);
				}
				#endregion

				#region 公共成员
				public T Current
				{
					get
					{
						var entity = (T)_populator.Populate(_reader);

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

									token.Schema.Token.SetValue(entity, CreateResults(Zongsoft.Common.TypeExtension.GetElementType(token.Schema.Token.MemberType), _context, selection, command));
								}
							}
						}

						return entity;
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
				private IDictionary<string, SlaveToken> GetSlaves(IStatement statement, IDataReader reader)
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
