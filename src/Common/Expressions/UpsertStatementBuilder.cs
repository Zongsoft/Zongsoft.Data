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
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpsertStatementBuilder : IStatementBuilder<DataUpsertContext>
	{
		#region 构建方法
		public IEnumerable<IStatementBase> Build(DataUpsertContext context)
		{
			return this.BuildStatements(context, context.Entity, context.Data, null, context.Schema.Members);
		}
		#endregion

		#region 私有方法
		private IEnumerable<IMutateStatement> BuildStatements(DataUpsertContext context, IDataEntity entity, object data, SchemaMember owner, IEnumerable<SchemaMember> schemas)
		{
			var inherits = entity.GetInherits();

			foreach(var inherit in inherits)
			{
				var statement = new UpsertStatement(inherit, owner);

				foreach(var schema in schemas)
				{
					if(!inherit.Properties.Contains(schema.Name))
						continue;

					if(schema.Token.Property.IsSimplex)
					{
						var simplex = (IDataEntitySimplexProperty)schema.Token.Property;

						if(simplex.Sequence != null && simplex.Sequence.IsBuiltin)
						{
							statement.Sequence = new SelectStatement(owner?.FullPath);
							statement.Sequence.Select.Members.Add(SequenceExpression.Current(simplex.Sequence.Name, simplex.Name));
						}
						else
						{
							//确认当前成员是否有提供的写入值
							var provided = context.TryGetProvidedValue(DataAccessMethod.Insert, simplex, out var value);

							var field = statement.Table.CreateField(schema.Token);
							statement.Fields.Add(field);

							var parameter = this.IsLinked(owner, simplex) ?
							                Expression.Parameter(schema.Token.Property.Name, simplex.Type) :
											(
												provided ?
												Expression.Parameter(ParameterExpression.Anonymous, field, schema, value):
												Expression.Parameter(ParameterExpression.Anonymous, field, schema)
											);

							statement.Values.Add(parameter);
							statement.Parameters.Add(parameter);

							//处理完新增子句部分，接着再处理修改子句部分
							if(!simplex.IsPrimaryKey && !simplex.Immutable && this.HasChanges(data, schema.Name))
							{
								//确认当前成员是否有提供的写入值
								if(context.TryGetProvidedValue(DataAccessMethod.Update, simplex, out value))
								{
									parameter = Expression.Parameter(ParameterExpression.Anonymous, field, schema, value);
									statement.Parameters.Add(parameter);
								}

								statement.Updation.Add(new FieldValue(field, parameter));
							}
						}
					}
					else
					{
						//不可变复合属性不支持任何写操作，即在修改操作中不能包含不可变复合属性
						if(schema.Token.Property.Immutable)
							throw new DataException($"The '{schema.FullPath}' is an immutable complex(navigation) property and does not support the upsert operation.");

						if(!schema.HasChildren)
							throw new DataException($"Missing members that does not specify '{schema.FullPath}' complex property.");

						var complex = (IDataEntityComplexProperty)schema.Token.Property;
						var slaves = this.BuildStatements(
							context,
							complex.Foreign,
							context.IsMultiple ? null : schema.Token.GetValue(data),
							schema,
							schema.Children);

						foreach(var slave in slaves)
						{
							slave.Schema = schema;
							statement.Slaves.Add(slave);
						}
					}
				}

				if(statement.Fields.Count > 0)
					yield return statement;
				else if(statement.HasSlaves)
				{
					foreach(var slave in statement.Slaves)
						yield return (IMutateStatement)slave;
				}
			}
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool IsLinked(SchemaMember owner, IDataEntitySimplexProperty property)
		{
			if(owner == null || owner.Token.Property.IsSimplex)
				return false;

			var links = ((IDataEntityComplexProperty)owner.Token.Property).Links;

			for(int i = 0; i < links.Length; i++)
			{
				if(object.Equals(links[i].Foreign, property))
					return true;
			}

			return false;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool HasChanges(object data, string name)
		{
			switch(data)
			{
				case IModel model:
					return model.HasChanges(name);
				case IDataDictionary dictionary:
					return dictionary.HasChanges(name);
				case IDictionary<string, object> generic:
					return generic.ContainsKey(name);
				case System.Collections.IDictionary classic:
					return classic.Contains(name);
			}

			return true;
		}
		#endregion
	}
}
