using System;
using System.Text;

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
		protected override IExpressionVisitor GetVisitor(StringBuilder output)
		{
			return new DummyExpressionVisitor(output);
		}
		#endregion
	}
}
