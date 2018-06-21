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
using System.Text;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.MySql
{
	public class MySqlExpressionVisitor : ExpressionVisitor
	{
		#region 构造函数
		public MySqlExpressionVisitor(StringBuilder output) : base(output)
		{
		}
		#endregion

		#region 公共属性
		public override IExpressionDialect Dialect
		{
			get
			{
				return MySqlExpressionDialect.Instance;
			}
		}
		#endregion

		#region 重写方法
		protected override IExpression VisitStatement(IStatement statement)
		{
			switch(statement)
			{
				case SelectStatement select:
					MySqlSelectStatementVisitor.Instance.Visit(select, this);
					break;
				case DeleteStatement delete:
					MySqlDeleteStatementVisitor.Instance.Visit(delete, this);
					break;
				case InsertStatement insert:
					MySqlInsertStatementVisitor.Instance.Visit(insert, this);
					break;
				case UpsertStatement upsert:
					MySqlUpsertStatementVisitor.Instance.Visit(upsert, this);
					break;
				case UpdateStatement update:
					MySqlUpdateStatementVisitor.Instance.Visit(update, this);
					break;
			}

			return statement;
		}
		#endregion

		#region 嵌套子类
		private class MySqlExpressionDialect : IExpressionDialect
		{
			#region 单例字段
			public static readonly MySqlExpressionDialect Instance = new MySqlExpressionDialect();
			#endregion

			#region 私有构造
			private MySqlExpressionDialect()
			{
			}
			#endregion

			#region 公共方法
			public string GetAggregateMethodName(Grouping.AggregateMethod method)
			{
				switch(method)
				{
					case Grouping.AggregateMethod.Count:
						return "COUNT";
					case Grouping.AggregateMethod.Sum:
						return "SUM";
					case Grouping.AggregateMethod.Average:
						return "AVG";
					case Grouping.AggregateMethod.Maximum:
						return "MAX";
					case Grouping.AggregateMethod.Minimum:
						return "MIN";
					case Grouping.AggregateMethod.Deviation:
						return "STDEV";
					case Grouping.AggregateMethod.DeviationPopulation:
						return "STDEV_POP";
					case Grouping.AggregateMethod.Variance:
						return "VARIANCE";
					case Grouping.AggregateMethod.VariancePopulation:
						return "VAR_POP";
					default:
						throw new NotSupportedException($"Invalid '{method}' aggregate method.");
				}
			}

			public string GetAlias(string alias)
			{
				return $"'{alias}'";
			}

			public string GetIdentifier(string name)
			{
				return $"`{name}`";
			}

			public string GetSymbol(Operator @operator)
			{
				return null;
			}
			#endregion
		}
		#endregion
	}
}
