using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyExecutionStatementVisitor : ExecutionStatementVisitor
	{
		public static readonly DummyExecutionStatementVisitor Instance = new DummyExecutionStatementVisitor();

		private DummyExecutionStatementVisitor()
		{
		}
	}
}
