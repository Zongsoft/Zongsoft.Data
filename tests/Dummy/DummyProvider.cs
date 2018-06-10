using System;
using System.Data;
using System.Collections.Generic;

using Zongsoft.Data.Common;

namespace Zongsoft.Data.Dummy
{
	public class DummyProvider : DataProvider
	{
		public DummyProvider(string name) : base(name)
		{
			this.Builder = new DummyStatementBuilder();
		}

		protected override void OnExecute(DataAccessContextBase context)
		{
		}
	}
}
