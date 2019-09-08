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
			return this.BuildStatements(context.Entity, null, context.Schema.Members);
		}
		#endregion

		#region 私有方法
		private IEnumerable<UpsertStatement> BuildStatements(IDataEntity entity, SchemaMember owner, IEnumerable<SchemaMember> schemas)
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
							var field = statement.Table.CreateField(schema.Token);
							statement.Fields.Add(field);

							var parameter = this.IsLinked(owner, simplex) ?
							                Expression.Parameter(schema.Token.Property.Name, simplex.Type) :
							                Expression.Parameter(ParameterExpression.Anonymous, schema, field);

							statement.Values.Add(parameter);
							statement.Parameters.Add(parameter);
						}
					}
					else
					{
						if(!schema.HasChildren)
							throw new DataException($"Missing members that does not specify '{schema.FullPath}' complex property.");

						var complex = (IDataEntityComplexProperty)schema.Token.Property;
						var slaves = this.BuildStatements(complex.Foreign, schema, schema.Children);

						foreach(var slave in slaves)
						{
							slave.Schema = schema;
							statement.Slaves.Add(slave);
						}
					}
				}

				yield return statement;
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
		#endregion
	}
}
