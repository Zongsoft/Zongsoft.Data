using System;
using System.Data;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyProvider : DataProviderBase
	{
		public DummyProvider() : base("DummyProvider", "DummyDriver")
		{
			this.Builder = new DummyStatementBuilder();
			this.Scriptor = new DummyStatementScriptor(this);
		}

		public override IDbCommand CreateCommand(string text = null, CommandType commandType = CommandType.Text)
		{
			throw new NotImplementedException();
		}

		public override IDbConnection CreateConnection()
		{
			throw new NotImplementedException();
		}
	}
}
