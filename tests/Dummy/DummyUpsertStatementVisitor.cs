using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyUpsertStatementVisitor : UpsertStatementVisitor
	{
		public static readonly DummyUpsertStatementVisitor Instance = new DummyUpsertStatementVisitor();

		private DummyUpsertStatementVisitor()
		{
		}
	}
}
