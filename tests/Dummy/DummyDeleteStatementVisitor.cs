using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyDeleteStatementVisitor : DeleteStatementVisitor
	{
		public static readonly DummyDeleteStatementVisitor Instance = new DummyDeleteStatementVisitor();

		private DummyDeleteStatementVisitor()
		{
		}
	}
}
