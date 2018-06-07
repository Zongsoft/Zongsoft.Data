using System;
using System.Text;

namespace Zongsoft.Data.Common.Expressions
{
	public abstract class StatementScriptorBase : IStatementScriptor
	{
		#region 构造函数
		protected StatementScriptorBase(IDataProvider provider)
		{
			this.Provider = provider ?? throw new ArgumentNullException(nameof(provider));
		}
		#endregion

		#region 公共属性
		public IDataProvider Provider
		{
			get;
		}
		#endregion

		#region 公共方法
		public virtual Script Script(IStatement statement)
		{
			if(statement == null)
				throw new ArgumentNullException(nameof(statement));

			IExpressionVisitor visitor = null;
			var text = new StringBuilder(1024);

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
				case UpsertStatement upsert:
					visitor = this.GetUpsertVisitor(upsert, text);
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

			if(statement.HasParameters)
				return new Script(text.ToString(), statement.Parameters);
			else
				return new Script(text.ToString());
		}
		#endregion

		#region 虚拟方法
		protected abstract IExpressionVisitor GetVisitor(IExpression expression, StringBuilder text);

		protected abstract SelectStatementVisitor GetSelectVisitor(SelectStatement statement, StringBuilder text);

		protected abstract DeleteStatementVisitor GetDeleteVisitor(DeleteStatement statement, StringBuilder text);

		protected abstract InsertStatementVisitor GetInsertVisitor(InsertStatement statement, StringBuilder text);

		protected abstract UpsertStatementVisitor GetUpsertVisitor(UpsertStatement statement, StringBuilder text);

		protected abstract UpdateStatementVisitor GetUpdateVisitor(UpdateStatement statement, StringBuilder text);
		#endregion
	}
}
