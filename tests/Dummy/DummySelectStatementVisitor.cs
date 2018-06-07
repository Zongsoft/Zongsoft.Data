using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummySelectStatementVisitor : SelectStatementVisitor
	{
		public DummySelectStatementVisitor(StringBuilder text) : base(text)
		{
		}
	}
}
