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

		private IStatement BuildStatement(DataInsertContext context, IDataSource source, object data = null, string path = null)
		{
			if(data == null)
				data = context.Data;

			var statement = new InsertStatement(context.Entity);
			var properties = this.GetProperties(data, context.Scope, context.Entity);

			foreach(var property in properties)
			{
				if(property.IsSimplex)
					statement.Fields.Add(statement.Table.CreateField((IEntitySimplexPropertyMetadata)property));
				else
				{
					if(((IEntityComplexPropertyMetadata)property).Multiplicity == AssociationMultiplicity.Many)
						statement.Slaves.Add(this.BuildStatements());
					else
						statement.Slaves.Add(this.BuildStatement(context, source, property.GetValue(data, path), path + "." + property.Name));
				}
			}

			return statement;
		}

		private ICollection<IEntityPropertyMetadata> GetProperties(object data, string scope, IEntityMetadata metadata)
		{
		}

		private IDictionary<string, object> GetData(object data, string scope, IEntityMetadata metadata)
		{
			if(data is IEntity entity)
				return entity.GetChanges();
		}

		private object GetData(object owner, string path, Type type)
		{
		}

		public class StatementToken
		{
			public string Name;

		}
	}
}
