using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyExpressionVisitor : ExpressionVisitor
	{
		#region 构造函数
		public DummyExpressionVisitor(StringBuilder output) : base(output)
		{
		}
		#endregion

		#region 公共属性
		public override IExpressionDialect Dialect => DummyDialect.Instance;
		#endregion

		#region 重写方法
		protected override IExpression VisitStatement(IStatement statement)
		{
			switch(statement)
			{
				case SelectStatement select:
					DummySelectStatementVisitor.Instance.Visit(this, select);
					break;
				case DeleteStatement delete:
					DummyDeleteStatementVisitor.Instance.Visit(this, delete);
					break;
				case InsertStatement insert:
					DummyInsertStatementVisitor.Instance.Visit(this, insert);
					break;
				case UpsertStatement upsert:
					DummyUpsertStatementVisitor.Instance.Visit(this, upsert);
					break;
				case UpdateStatement update:
					DummyUpdateStatementVisitor.Instance.Visit(this, update);
					break;
			}

			return statement;
		}
		#endregion

		#region 嵌套子类
		private class DummyDialect : IExpressionDialect
		{
			public static readonly DummyDialect Instance = new DummyDialect();

			public string GetAggregateMethodName(Grouping.AggregateMethod method)
			{
				return method.ToString();
			}

			public string GetAlias(string alias)
			{
				return "'" + alias + "'";
			}

			public string GetIdentifier(string name)
			{
				return name;
			}

			public string GetSymbol(Operator @operator)
			{
				return null;
			}
		}
		#endregion
	}
}
