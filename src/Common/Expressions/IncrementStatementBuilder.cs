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

namespace Zongsoft.Data.Common.Expressions
{
	public class IncrementStatementBuilder : IStatementBuilder<DataIncrementContext>
	{
		public IEnumerable<IStatementBase> Build(DataIncrementContext context)
		{
			var statement = new UpdateStatement(context.Entity);

			var source = statement.From(context.Member, null, out var property);
			var field = source.CreateField(property);
			var value = context.Interval > 0 ?
			            Expression.Add(field, Expression.Constant(context.Interval)) :
			            Expression.Subtract(field, Expression.Constant(-context.Interval));

			//添加修改字段
			statement.Fields.Add(new FieldValue(field, value));

			//构建WHERE子句
			statement.Where = statement.Where(context.Condition);

			if(context.Source.Features.Support(Feature.Updation.Outputting))
			{
				statement.Returning = new ReturningClause();

				statement.Returning.Table.Field((Metadata.IDataEntitySimplexProperty)property);
				statement.Returning.Append(field, ReturningClause.ReturningMode.Inserted);
			}
			else
			{
				var slave = new SelectStatement();

				foreach(var from in statement.From)
					slave.From.Add(from);

				slave.Where = statement.Where;
				slave.Select.Members.Add(field);

				//注：由于从属语句的WHERE子句只是简单的指向父语句的WHERE子句，
				//因此必须手动将父语句的参数依次添加到从属语句中。
				foreach(var parameter in statement.Parameters)
				{
					slave.Parameters.Add(parameter);
				}

				statement.Slaves.Add(slave);
			}

			yield return statement;
		}
	}
}
