using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyStatementScriptor : StatementScriptorBase
	{
		#region 构造函数
		public DummyStatementScriptor()
		{
		}
		#endregion

		#region 重写方法
		protected override IExpressionVisitor GetVisitor(IExpression expression, StringBuilder text)
		{
			return new DummyExpressionVisitor(text);
		}

		protected override SelectStatementVisitor GetSelectVisitor(SelectStatement statement, StringBuilder text)
		{
			return new DummySelectStatementVisitor(text);
		}

		protected override DeleteStatementVisitor GetDeleteVisitor(DeleteStatement statement, StringBuilder text)
		{
			return new DummyDeleteStatementVisitor(text);
		}

		protected override InsertStatementVisitor GetInsertVisitor(InsertStatement statement, StringBuilder text)
		{
			return new DummyInsertStatementVisitor(text);
		}

		protected override UpsertStatementVisitor GetUpsertVisitor(UpsertStatement statement, StringBuilder text)
		{
			return new DummyUpsertStatementVisitor(text);
		}

		protected override UpdateStatementVisitor GetUpdateVisitor(UpdateStatement statement, StringBuilder text)
		{
			return new DummyUpdateStatementVisitor(text);
		}
		#endregion
	}
}
