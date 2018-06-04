using System;
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpdateStatementVisitor : ExpressionVisitor
	{
		#region 构造函数
		public UpdateStatementVisitor()
		{
		}

		public UpdateStatementVisitor(StringBuilder text) : base(text)
		{
		}
		#endregion

		#region 公共方法
		public override IExpression Visit(IExpression expression)
		{
			if(expression is UpdateStatement statement)
			{
				if(statement.Fields == null || statement.Fields.Count == 0)
					throw new DataException("Missing required fields in the update statment.");

				this.Text.Append("UPDATE ");
				this.VisitTable(statement.Table);
				this.Text.Append(" SET ");

				foreach(var field in statement.Fields)
				{
					this.VisitField(field.Field);
					this.Text.Append("=");
					this.Visit(field.Value);
				}

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
