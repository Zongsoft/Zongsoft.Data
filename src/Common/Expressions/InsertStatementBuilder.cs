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
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class InsertStatementBuilder : IStatementBuilder
	{
		IStatement IStatementBuilder.Build(DataAccessContextBase context, IDataSource source)
		{
			if(context.Method == DataAccessMethod.Insert)
				return this.Build((DataInsertContext)context, source);

			//抛出数据异常
			throw new DataException($"The {this.GetType().Name} builder does not support the {context.Method} operation.");
		}

		public IStatement Build(DataInsertContext context, IDataSource source)
		{
			if(context.Data == null)
				return null;

			if(context.IsMultiple)
			{
				if(Zongsoft.Common.TypeExtension.IsDictionary(context.Data.GetType()))
					return this.BuildStatement(context, source);

				if(context.Data is IEnumerable)
				{
					var statements = new StatementCollection();

					foreach(var item in (IEnumerable)context.Data)
					{
						statements.Add(this.BuildStatement(context, source, item));
					}

					return statements;
				}
			}

			return this.BuildStatement(context, source);
		}

		private IStatement BuildStatement(IEntityMetadata entity, object data, IEnumerable<Scope> scopes, bool isMultiple)
		{
			var inherits = entity.GetInherits();
			var statements = inherits.Length > 1 ? new StatementCollection() : null;
			InsertStatement statement = null;

			foreach(var inherit in inherits)
			{
				statement = new InsertStatement(inherit);

				foreach(var scope in scopes)
				{
					if(scope.Token.Property.IsSimplex)
					{
						statement.Fields.Add(statement.Table.CreateField(scope.Token));

						if(!isMultiple)
							statement.Values.Add(Expression.Constant(scope.Token.GetValue(data)));
					}
					else
					{
						var complex = (IEntityComplexPropertyMetadata)scope.Token.Property;
						var slave = this.BuildStatement(complex.GetForeignEntity(), scope.Token.GetValue(data), (IEnumerable<Scope>)scope.Children, complex.Multiplicity == AssociationMultiplicity.Many);

						if(complex.Multiplicity == AssociationMultiplicity.Many)
						{
						}

						statement.Slaves.Add(slave);
					}
				}

				if(isMultiple)
				{
					var items = (data as IEnumerable) ?? throw new DataException("");

					foreach(var item in items)
					{
						foreach(var field in statement.Fields)
						{
							statement.Values.Add(Expression.Constant(field.Token.GetValue(item)));
						}
					}
				}

				if(statements != null && statement.HasValues)
					statements.Add(statement);
			}

			return (IStatement)statements ?? statement;
		}

		private IStatement BuildStatement(DataInsertContext context, IDataSource source, object data = null, string path = null)
		{
			if(data == null)
				data = context.Data;

			var entities = context.Entity.GetInherits();
			var statements = entities.Length > 1 ? new StatementCollection() : null;
			InsertStatement statement = null;

			foreach(var inherit in context.Entity.GetInherits())
			{
				statement = new InsertStatement(inherit);
				var segments = context.Scopes;
				var values = new List<IExpression>();

				foreach(var segment in segments)
				{
					var token = ((Scope.Segment<EntityPropertyToken>)segment).Token;

					if(token.Property.IsSimplex)
					{
						statement.Fields.Add(statement.Table.CreateField((IEntitySimplexPropertyMetadata)token.Property));
						values.Add(Expression.Constant(token.GetValue(data)));
					}
					else
					{
						this.BuildChild((Scope.Segment<EntityPropertyToken>)segment, token.GetValue(data));
					}
				}

				if(statements != null)
					statements.Add(statement);
			}

			return (IStatement)statements ?? statement;
		}

		private void BuildChild(Scope.Segment<EntityPropertyToken> segment, object data)
		{
			if(!segment.HasChildren)
				return;

			var complex = (IEntityComplexPropertyMetadata)segment.Token.Property;
			var entity = complex.GetForeignEntity();
			var statement = new InsertStatement(entity);
			var values = new List<Expression>();

			foreach(var child in segment.Children)
			{
				var token = ((Scope.Segment<EntityPropertyToken>)child).Token;

				if(token.Property.IsSimplex)
				{
					statement.Fields.Add(statement.Table.CreateField((IEntitySimplexPropertyMetadata)token.Property));
					values.Add(Expression.Constant(token.GetValue(data)));
				}
				else
				{
					this.BuildChild((Scope.Segment<EntityPropertyToken>)child, token.GetValue(data));
				}
			}
		}
	}
}
