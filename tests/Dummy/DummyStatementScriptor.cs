using System;
using System.Text;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyStatementScriptor : StatementScriptorBase
	{
		#region 单例字段
		public static readonly DummyStatementScriptor Default = new DummyStatementScriptor();
		#endregion

		#region 构造函数
		private DummyStatementScriptor()
		{
		}
		#endregion

		#region 重写方法
		protected override IExpressionVisitor GetVisitor(StringBuilder output)
		{
			return new DummyExpressionVisitor(output);
		}
		#endregion
	}
}
