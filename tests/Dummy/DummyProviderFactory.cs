using System;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Common;

namespace Zongsoft.Data.Dummy
{
	public class DummyProviderFactory : DataProviderFactory
	{
		protected override IDataProvider CreateProvider(string name)
		{
			return new DummyProvider(name);
		}
	}
}
