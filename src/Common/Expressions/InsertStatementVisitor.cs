using System;
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class InsertStatementVisitor : ExpressionVisitor
	{
		#region 构造函数
		public InsertStatementVisitor(StringBuilder text) : base(text)
		{
		}
		#endregion

		#region 公共方法
		public override IExpression Visit(IExpression expression)
		{
			if(expression is InsertStatement statement)
			{
				if(statement.Fields == null || statement.Fields.Count == 0)
					throw new DataException("Missing required fields in the insert statment.");

				this.Text.Append("INSERT INTO ");
				this.VisitTable(statement.Table);

				this.Text.Append(" (");

				var index = 0;

				foreach(var field in statement.Fields)
				{
					if(index++ > 0)
						this.Text.Append(",");

					this.VisitField(field);
				}

				this.Text.Append(") VALUES ");
				index = 0;

				foreach(var value in statement.Values)
				{
					if(index++ > 0)
						this.Text.Append(",");

					if(index % statement.Fields.Count == 0)
						this.Text.Append("(");

					this.Visit(value);

					if(index % statement.Fields.Count == 0)
						this.Text.Append(")");
				}

				this.Text.Append(")");

				return statement;
			}

			//调用基类同名方法
			return base.Visit(expression);
		}
		#endregion
	}
}
