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
		public DummyExpressionVisitor()
		{
		}
		#endregion

		#region 公共属性
		//public override IExpressionDialect Dialect => DummyDialect.Instance;
		#endregion

		#region 重写方法
		protected override IExpression VisitStatement(IStatementBase statement)
		{
			switch(statement)
			{
				case TableDefinition table:
					DummyTableDefinitionVisitor.Instance.Visit(this, table);
					break;
				case CountStatement count:
					DummyCountStatementVisitor.Instance.Visit(this, count);
					break;
				case ExistStatement exist:
					DummyExistStatementVisitor.Instance.Visit(this, exist);
					break;
				case ExecutionStatement execution:
					DummyExecutionStatementVisitor.Instance.Visit(this, execution);
					break;
				case SelectStatement select:
					DummySelectStatementVisitor.Instance.Visit(this, select);
					break;
				case DeleteStatement delete:
					DummyDeleteStatementVisitor.Instance.Visit(this, delete);
					break;
				case InsertStatement insert:
					DummyInsertStatementVisitor.Instance.Visit(this, insert);
					break;
				case UpdateStatement update:
					DummyUpdateStatementVisitor.Instance.Visit(this, update);
					break;
				case UpsertStatement upsert:
					DummyUpsertStatementVisitor.Instance.Visit(this, upsert);
					break;
			}

			return statement;
		}
		#endregion

		#region 嵌套子类
		private class DummyDialect : IExpressionDialect
		{
			public static readonly DummyDialect Instance = new DummyDialect();

			public string GetAlias(string alias)
			{
				return "'" + alias + "'";
			}

			public string GetIdentifier(string name)
			{
				return name;
			}

			public string GetIdentifier(IIdentifier identifier)
			{
				return identifier.Name;
			}

			public string GetSymbol(Operator @operator)
			{
				return null;
			}

			public string GetDbType(System.Data.DbType dbType, int length, byte precision, byte scale)
			{
				return dbType.ToString();
			}

			public string GetMethodName(MethodExpression method)
			{
				switch(method)
				{
					case AggregateExpression aggregate:
						return aggregate.Method.ToString();
					case SequenceExpression sequence:
						return sequence.Method.ToString();
					default:
						return method.Name;
				}
			}
		}

		private class DummyTableDefinitionVisitor : TableDefinitionVisitor
		{
			public static readonly DummyTableDefinitionVisitor Instance = new DummyTableDefinitionVisitor();

			private DummyTableDefinitionVisitor()
			{
			}
		}
		#endregion
	}
}
