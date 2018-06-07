using System;
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class DeleteStatementVisitor : ExpressionVisitor
	{
		#region 构造函数
		public DeleteStatementVisitor(StringBuilder text) : base(text)
		{
		}
		#endregion

		#region 公共方法
		public override IExpression Visit(IExpression expression)
		{
			if(expression is DeleteStatement statement)
			{
				this.Text.Append("DELETE FROM ");
				this.VisitTable(statement.Table);

				if(statement.Where != null)
					this.VisitWhere(statement.Where);

				return statement;
			}

			//调用基类同名方法
			return base.Visit(expression);
		}
		#endregion

		#region 虚拟方法
		protected virtual void VisitWhere(IExpression where)
		{
			this.Text.Append(" WHERE ");
			this.Visit(where);
			this.Text.AppendLine();
		}
		#endregion
	}
}
