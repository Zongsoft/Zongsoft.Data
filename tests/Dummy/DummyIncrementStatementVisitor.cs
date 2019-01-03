using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyIncrementStatementVisitor : IncrementStatementVisitor
	{
		public static readonly DummyIncrementStatementVisitor Instance = new DummyIncrementStatementVisitor();

		private DummyIncrementStatementVisitor()
		{
		}
	}
}
