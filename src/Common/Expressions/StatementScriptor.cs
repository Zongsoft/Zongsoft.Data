using System;
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class StatementScriptor : IStatementScriptor
	{
		#region 公共方法
		public string Script(IExpression statement)
		{
			if(statement == null)
				throw new ArgumentNullException(nameof(statement));

			IExpressionVisitor visitor = null;
			var text = new StringBuilder(1024 * 4);

			switch(statement)
			{
				case SelectStatement select:
					visitor = this.GetSelectVisitor(select, text);
					break;
				case DeleteStatement delete:
					visitor = this.GetDeleteVisitor(delete, text);
					break;
				case InsertStatement insert:
					visitor = this.GetInsertVisitor(insert, text);
					break;
				case UpdateStatement update:
					visitor = this.GetUpdateVisitor(update, text);
					break;
				default:
					visitor = this.GetVisitor(statement, text);
					break;
			}

			if(visitor == null)
				return null;

			visitor.Visit(statement);
			return text.ToString();
		}
		#endregion

		#region 虚拟方法
		protected virtual IExpressionVisitor GetVisitor(IExpression expression, StringBuilder text)
		{
			return new ExpressionVisitor(text);
		}

		protected virtual SelectStatementVisitor GetSelectVisitor(SelectStatement statement, StringBuilder text)
		{
			return new SelectStatementVisitor(text);
		}

		protected virtual DeleteStatementVisitor GetDeleteVisitor(DeleteStatement statement, StringBuilder text)
		{
			return new DeleteStatementVisitor(text);
		}

		protected virtual InsertStatementVisitor GetInsertVisitor(InsertStatement statement, StringBuilder text)
		{
			return new InsertStatementVisitor(text);
		}

		protected virtual UpdateStatementVisitor GetUpdateVisitor(UpdateStatement statement, StringBuilder text)
		{
			return new UpdateStatementVisitor(text);
		}
		#endregion
	}
}
