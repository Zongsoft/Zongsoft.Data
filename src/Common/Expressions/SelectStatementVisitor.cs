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
	public class SelectStatementVisitor : IStatementVisitor<SelectStatement>
	{
		#region 构造函数
		protected SelectStatementVisitor()
		{
		}
		#endregion

		#region 公共方法
		public void Visit(SelectStatement statement, IExpressionVisitor visitor)
		{
			//通知当前语句开始访问
			this.OnVisiting(statement, visitor);

			if(statement.Select != null && statement.Select.Members.Count > 0)
				this.VisitSelect(statement.Select, visitor);

			if(statement.Into != null)
				this.VisitInto(statement.Into, visitor);

			if(statement.From != null && statement.From.Count > 0)
				this.VisitFrom(statement.From, visitor);

			if(statement.Where != null)
				this.VisitWhere(statement.Where, visitor);

			if(statement.GroupBy != null && statement.GroupBy.Keys.Count > 0)
				this.VisitGroupBy(statement.GroupBy, visitor);

			if(statement.OrderBy != null && statement.OrderBy.Members.Count > 0)
				this.VisitOrderBy(statement.OrderBy, visitor);

			//通知当前语句访问完成
			this.OnVisited(statement, visitor);

			if(statement.HasSlaves)
			{
				foreach(var slave in statement.Slaves)
				{
					visitor.Output.AppendLine();
					visitor.Output.AppendLine($"/* {slave.Slaver.Name} */");

					this.Visit(slave, visitor);
				}
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual void VisitSelect(SelectClause clause, IExpressionVisitor visitor)
		{
			if(visitor.Output.Length > 0)
				visitor.Output.AppendLine();

			visitor.Output.Append("SELECT ");

			if(clause.IsDistinct)
				visitor.Output.Append("DISTINCT ");

			int index = 0;

			foreach(var member in clause.Members)
			{
				if(index++ > 0)
					visitor.Output.AppendLine(",");

				visitor.Visit(member);
			}
		}

		protected virtual void VisitInto(IIdentifier into, IExpressionVisitor visitor)
		{
			visitor.Output.Append(" INTO ");
			visitor.Visit(into);
		}

		protected virtual void VisitFrom(ICollection<ISource> sources, IExpressionVisitor visitor)
		{
			if(visitor.Output.Length > 0)
				visitor.Output.AppendLine();

			visitor.Output.Append("FROM ");

			foreach(var source in sources)
			{
				switch(source)
				{
					case TableIdentifier table:
						visitor.Visit(table);

						break;
					case SelectStatement subquery:
						visitor.Output.Append("(");

						//递归生成子查询语句
						visitor.Visit(subquery);

						if(string.IsNullOrEmpty(subquery.Alias))
							visitor.Output.Append(")");
						else
							visitor.Output.Append(") AS " + subquery.Alias);

						break;
					case JoinClause joining:
						this.VisitJoin(joining, visitor);

						break;
				}
			}
		}

		protected virtual void VisitJoin(JoinClause joining, IExpressionVisitor visitor)
		{
			visitor.Output.AppendLine();

			switch(joining.Type)
			{
				case JoinType.Inner:
					visitor.Output.Append("INNER JOIN ");
					break;
				case JoinType.Left:
					visitor.Output.Append("LEFT JOIN ");
					break;
				case JoinType.Right:
					visitor.Output.Append("RIGHT JOIN ");
					break;
				case JoinType.Full:
					visitor.Output.Append("FULL JOIN ");
					break;
			}

			switch(joining.Target)
			{
				case TableIdentifier table:
					visitor.Visit(table);
					visitor.Output.AppendLine(" ON /* " + joining.Name + " */");

					break;
				case SelectStatement subquery:
					visitor.Output.Append("(");

					//递归生成子查询语句
					visitor.Visit(subquery);

					if(string.IsNullOrEmpty(subquery.Alias))
						visitor.Output.AppendLine(") ON");
					else
						visitor.Output.AppendLine(") AS " + subquery.Alias + " ON");

					break;
			}

			visitor.Visit(joining.Condition);
		}

		protected virtual void VisitWhere(IExpression where, IExpressionVisitor visitor)
		{
			if(visitor.Output.Length > 0)
				visitor.Output.AppendLine();

			visitor.Output.Append("WHERE ");
			visitor.Visit(where);
		}

		protected virtual void VisitGroupBy(GroupByClause clause, IExpressionVisitor visitor)
		{
			if(visitor.Output.Length > 0)
				visitor.Output.AppendLine();

			visitor.Output.Append("GROUP BY ");

			int index = 0;

			foreach(var key in clause.Keys)
			{
				if(index++ > 0)
					visitor.Output.Append(",");

				visitor.Visit(key);
			}

			if(clause.Having != null)
			{
				visitor.Output.AppendLine();
				visitor.Output.Append("HAVING ");
				visitor.Visit(clause.Having);
			}
		}

		protected virtual void VisitOrderBy(OrderByClause clause, IExpressionVisitor visitor)
		{
			if(visitor.Output.Length > 0)
				visitor.Output.AppendLine();

			visitor.Output.Append("ORDER BY ");

			int index = 0;

			foreach(var member in clause.Members)
			{
				if(index++ > 0)
					visitor.Output.Append(",");

				visitor.Visit(member.Field);

				if(member.Mode == SortingMode.Descending)
					visitor.Output.Append(" DESC");
			}
		}

		protected virtual void OnVisiting(SelectStatement statement, IExpressionVisitor visitor)
		{
		}

		protected virtual void OnVisited(SelectStatement statement, IExpressionVisitor visitor)
		{
			visitor.Output.AppendLine(";");
		}
		#endregion
	}
}
