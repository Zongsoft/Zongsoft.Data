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
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class SelectStatementVisitor : ExpressionVisitor
	{
		#region 构造函数
		public SelectStatementVisitor(StringBuilder text) : base(text)
		{
		}
		#endregion

		#region 公共方法
		public override IExpression Visit(IExpression expression)
		{
			if(expression is SelectStatement statement)
			{
				if(statement.Select != null && statement.Select.Members.Count > 0)
					this.VisitSelect(statement.Select);

				if(statement.Into != null)
					this.VisitInto(statement.Into);

				if(statement.From != null && statement.From.Count > 0)
					this.VisitFrom(statement.From);

				if(statement.Where != null)
					this.VisitWhere(statement.Where);

				if(statement.GroupBy != null && statement.GroupBy.Keys.Count > 0)
					this.VisitGroupBy(statement.GroupBy);

				if(statement.OrderBy != null && statement.OrderBy.Members.Count > 0)
					this.VisitOrderBy(statement.OrderBy);

				if(statement.HasSlaves)
				{
					this.Text.AppendLine();

					foreach(var slave in statement.Slaves)
					{
						this.Text.AppendLine($"/* {slave.Slaver.Name} */");
						this.Visit(slave);
					}
				}

				return statement;
			}

			//调用基类同名方法
			return base.Visit(expression);
		}
		#endregion

		#region 虚拟方法
		protected virtual void VisitSelect(SelectClause clause)
		{
			this.Text.Append("SELECT ");

			if(clause.IsDistinct)
				this.Text.Append("DISTINCT ");

			int index = 0;

			foreach(var member in clause.Members)
			{
				if(index++ > 0)
					this.Text.AppendLine(",");

				this.Visit(member);
			}

			this.Text.AppendLine();
		}

		protected virtual void VisitInto(IIdentifier into)
		{
			this.Text.Append("INTO ");
			this.Visit(into);
			this.Text.AppendLine();
		}

		protected virtual void VisitFrom(ICollection<ISource> sources)
		{
			this.Text.Append("FROM ");

			foreach(var source in sources)
			{
				switch(source)
				{
					case TableIdentifier table:
						this.Visit(table);
						this.Text.AppendLine();

						break;
					case SelectStatement subquery:
						this.Text.Append("(");

						//递归生成子查询语句
						this.Visit(subquery);

						if(string.IsNullOrEmpty(subquery.Alias))
							this.Text.AppendLine(")");
						else
							this.Text.AppendLine(") AS " + this.GetAlias(subquery.Alias));

						break;
					case JoinClause joining:
						this.VisitJoin(joining);

						break;
				}
			}
		}

		protected virtual void VisitJoin(JoinClause joining)
		{
			switch(joining.Type)
			{
				case JoinType.Inner:
					this.Text.Append("INNER JOIN ");
					break;
				case JoinType.Left:
					this.Text.Append("LEFT JOIN ");
					break;
				case JoinType.Right:
					this.Text.Append("RIGHT JOIN ");
					break;
				case JoinType.Full:
					this.Text.Append("FULL JOIN ");
					break;
			}

			switch(joining.Target)
			{
				case TableIdentifier table:
					this.Visit(table);
					this.Text.AppendLine(" ON /* " + joining.Name + " */");

					break;
				case SelectStatement subquery:
					this.Text.Append("(");

					//递归生成子查询语句
					this.Visit(subquery);

					if(string.IsNullOrEmpty(subquery.Alias))
						this.Text.AppendLine(") ON");
					else
						this.Text.AppendLine(") AS " + this.GetAlias(subquery.Alias) + " ON");

					break;
			}

			this.Visit(joining.Condition);
			this.Text.AppendLine();
		}

		protected virtual void VisitWhere(IExpression where)
		{
			this.Text.Append("WHERE ");
			this.Visit(where);
			this.Text.AppendLine();
		}

		protected virtual void VisitGroupBy(GroupByClause clause)
		{
			this.Text.Append("GROUP BY ");

			int index = 0;

			foreach(var key in clause.Keys)
			{
				if(index++ > 0)
					this.Text.Append(",");

				this.Visit(key);
			}

			this.Text.AppendLine();

			if(clause.Having != null)
			{
				this.Text.Append("HAVING ");
				this.Visit(clause.Having);
				this.Text.AppendLine();
			}
		}

		protected virtual void VisitOrderBy(OrderByClause clause)
		{
			this.Text.Append("ORDER BY ");

			int index = 0;

			foreach(var member in clause.Members)
			{
				if(index++ > 0)
					this.Text.Append(",");

				this.Visit(member.Field);

				if(member.Mode == SortingMode.Descending)
					this.Text.Append(" DESC");
			}

			this.Text.AppendLine();
		}
		#endregion
	}
}
