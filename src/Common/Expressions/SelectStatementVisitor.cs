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
	public class SelectStatementVisitor : StatementVisitorBase<SelectStatement>
	{
		#region 构造函数
		protected SelectStatementVisitor()
		{
		}
		#endregion

		#region 重写方法
		protected override void OnVisit(IExpressionVisitor visitor, SelectStatement statement)
		{
			if(statement.Select != null && statement.Select.Members.Count > 0)
				this.VisitSelect(visitor, statement.Select);

			if(statement.Into != null)
				this.VisitInto(visitor, statement.Into);

			if(statement.From != null && statement.From.Count > 0)
				this.VisitFrom(visitor, statement.From);

			if(statement.Where != null)
				this.VisitWhere(visitor, statement.Where);

			if(statement.GroupBy != null && statement.GroupBy.Keys.Count > 0)
				this.VisitGroupBy(visitor, statement.GroupBy);

			if(statement.OrderBy != null && statement.OrderBy.Members.Count > 0)
				this.VisitOrderBy(visitor, statement.OrderBy);
		}

		protected override void OnVisiting(IExpressionVisitor visitor, SelectStatement statement)
		{
			//if(statement.Master != null)
			//	visitor.Output.AppendLine($"/* {statement.Master.Name} */");

			//调用基类同名方法
			base.OnVisiting(visitor, statement);
		}

		protected override void OnVisited(IExpressionVisitor visitor, SelectStatement statement)
		{
			if(visitor.Depth == 0)
				visitor.Output.AppendLine(";");

			//调用基类同名方法
			base.OnVisited(visitor, statement);
		}
		#endregion

		#region 虚拟方法
		protected virtual void VisitSelect(IExpressionVisitor visitor, SelectClause clause)
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

		protected virtual void VisitInto(IExpressionVisitor visitor, IIdentifier into)
		{
			visitor.Output.Append(" INTO ");
			visitor.Visit(into);
		}

		protected virtual void VisitFrom(IExpressionVisitor visitor, ICollection<ISource> sources)
		{
			visitor.VisitFrom(sources, (v, j) => this.VisitJoin(v, j));
		}

		protected virtual void VisitJoin(IExpressionVisitor visitor, JoinClause joining)
		{
			visitor.VisitJoin(joining);
		}

		protected virtual void VisitWhere(IExpressionVisitor visitor, IExpression where)
		{
			visitor.VisitWhere(where);
		}

		protected virtual void VisitGroupBy(IExpressionVisitor visitor, GroupByClause clause)
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

		protected virtual void VisitOrderBy(IExpressionVisitor visitor, OrderByClause clause)
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
		#endregion
	}
}
