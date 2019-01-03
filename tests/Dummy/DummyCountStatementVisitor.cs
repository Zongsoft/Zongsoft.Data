using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyCountStatementVisitor : CountStatementVisitor
	{
		public static readonly DummyCountStatementVisitor Instance = new DummyCountStatementVisitor();

		private DummyCountStatementVisitor()
		{
		}
	}
}
