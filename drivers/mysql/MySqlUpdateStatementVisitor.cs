﻿/*
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
 * This file is part of Zongsoft.Data.MySql.
 *
 * Zongsoft.Data.MySql is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data.MySql is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data.MySql; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.MySql
{
	public class MySqlUpdateStatementVisitor : UpdateStatementVisitor
	{
		#region 单例字段
		public static readonly MySqlUpdateStatementVisitor Instance = new MySqlUpdateStatementVisitor();
		#endregion

		#region 构造函数
		private MySqlUpdateStatementVisitor()
		{
		}
		#endregion

		#region 重写方法
		protected override void VisitTables(IExpressionVisitor visitor, IList<TableIdentifier> tables, UpdateStatement statement)
		{
			//调用基类同名方法
			base.VisitTables(visitor, tables, statement);

			/*
			 * 注意：由于 MySQL 的 UPDATE 语句不支持 FROM 子句，因此必须将其改写为多表修改的语法。
			 */

			if(statement.HasFrom)
			{
				foreach(var source in statement.From)
				{
					switch(source)
					{
						case TableIdentifier table:
							if(!tables.Contains(table))
							{
								visitor.Output.Append(",");
								visitor.Visit(table);
							}

							break;
						case JoinClause join:
							if(join.Target is TableIdentifier target)
							{
								visitor.Output.Append(",");
								visitor.Visit(target);
							}
							else
							{
								throw new DataException($"The {MySqlDriver.Key} driver does not support the FROM clause of the UPDATE statement contain an expression of type '{join.Target.GetType().Name}'.");
							}

							break;
						default:
							throw new NotSupportedException($"The {MySqlDriver.Key} driver does not support the FROM clause of the UPDATE statement contain an expression of type '{source.GetType().Name}'.");
					}
				}
			}
		}

		protected override void VisitWhere(IExpressionVisitor visitor, IExpression where, UpdateStatement statement)
		{
			/*
			 * 注意：由于 MySQL 的 UPDATE 语句不支持 FROM 子句，因此必须将其改写为多表修改的语法。
			 * 由于 FROM 子句中可能包含 JOIN 类型语句，所以必须将 JOIN 子句中的条件式添加到 UPDATE 语句的 WHERE 子句中。
			 */

			if(statement.HasFrom)
			{
				var expressions = Expression.Block(BlockExpressionDelimiter.Space);

				foreach(var source in statement.From)
				{
					if(source is JoinClause join)
					{
						expressions.Add(join.Type == JoinType.Inner ? Expression.Literal("AND") : Expression.Literal("OR"));
						expressions.Add(join.Condition);
					}
				}

				if(expressions.Count > 0)
				{
					expressions.Insert(0, where);
					where = expressions;
				}
			}

			//调用基类同名方法
			base.VisitWhere(visitor, where, statement);
		}

		protected override void VisitFrom(IExpressionVisitor visitor, ICollection<ISource> sources, UpdateStatement statement)
		{
			/*
			 * 由于 MySQL 的 UPDATE 语句不支持 FROM 子句，故不输出任何内容，且不调用基类同名方法以避免生成错误的语句。
			 */
		}

		protected override void VisitReturning(IExpressionVisitor visitor, ReturningClause returning, UpdateStatement statement)
		{
			/*
			 * 由于 MySQL 的 UPDATE 语句不支持 RETURN|RETURNING 子句，故不输出任何内容，且不调用基类同名方法以避免生成错误的语句。
			 */
		}
		#endregion
	}
}
