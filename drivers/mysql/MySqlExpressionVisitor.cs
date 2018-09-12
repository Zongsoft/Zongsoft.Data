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
using System.Data;
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
				case TableDefinition table:
					MySqlTableDefinitionVisitor.Instance.Visit(this, table);
					break;
				case SelectStatement select:
					MySqlSelectStatementVisitor.Instance.Visit(this, select);
					break;
				case DeleteStatement delete:
					MySqlDeleteStatementVisitor.Instance.Visit(this, delete);
					break;
				case InsertStatement insert:
					MySqlInsertStatementVisitor.Instance.Visit(this, insert);
					break;
				case UpsertStatement upsert:
					MySqlUpsertStatementVisitor.Instance.Visit(this, upsert);
					break;
				case UpdateStatement update:
					MySqlUpdateStatementVisitor.Instance.Visit(this, update);
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

			public string GetDbType(DbType dbType)
			{
				switch(dbType)
				{
					case DbType.AnsiString:
						return "varchar";
					case DbType.AnsiStringFixedLength:
						return "char";
					case DbType.Binary:
						return "varbinary";
					case DbType.Boolean:
						return "tinyint(1)";
					case DbType.Byte:
						return "unsigned tinyint";
					case DbType.Currency:
						return "decimal(12,2)";
					case DbType.Date:
						return "date";
					case DbType.DateTime:
						return "datetime";
					case DbType.DateTime2:
						return "datetime2";
					case DbType.DateTimeOffset:
						return "datetime";
					case DbType.Decimal:
						return "decimal";
					case DbType.Double:
						return "double";
					case DbType.Guid:
						return "binary(16)";
					case DbType.Int16:
						return "smallint";
					case DbType.Int32:
						return "int";
					case DbType.Int64:
						return "bigint";
					case DbType.Object:
						return "json";
					case DbType.SByte:
						return "tinyint";
					case DbType.Single:
						return "float";
					case DbType.String:
						return "nvarchar";
					case DbType.StringFixedLength:
						return "nchar";
					case DbType.Time:
						return "time";
					case DbType.UInt16:
						return "unsigned smallint";
					case DbType.UInt32:
						return "unsigned int";
					case DbType.UInt64:
						return "unsigned bigint";
					case DbType.Xml:
						return "nvarchar(4000)";
				}

				throw new DataException($"Unsupported '{dbType.ToString()}' data type.");
			}
			#endregion
		}

		private class MySqlTableDefinitionVisitor : TableDefinitionVisitor
		{
			#region 单例字段
			public static readonly MySqlTableDefinitionVisitor Instance = new MySqlTableDefinitionVisitor();
			#endregion

			#region 私有构造
			private MySqlTableDefinitionVisitor()
			{
			}
			#endregion
		}
		#endregion
	}
}
