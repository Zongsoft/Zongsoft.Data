using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyExistStatementVisitor : ExistStatementVisitor
	{
		public static readonly DummyExistStatementVisitor Instance = new DummyExistStatementVisitor();

		private DummyExistStatementVisitor()
		{
		}
	}
}
