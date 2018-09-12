using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Tests
{
	public static class Utility
	{
		public static Zongsoft.Data.Common.IDataProvider GetProvider(string applicationName)
		{
			if(string.IsNullOrEmpty(applicationName))
				throw new ArgumentNullException(nameof(applicationName));

			DataEnvironment.Drivers.Add(new Dummy.DummyDriver());

			var provider = DataEnvironment.Providers.GetProvider(applicationName);
			provider.Connector = Dummy.DummyConnector.Instance;

			if(provider.Metadata.Loader is Metadata.Profiles.MetadataFileLoader loader)
				loader.Path = @"/Zongsoft/Zongsoft.Community/src/|/Zongsoft/Zongsoft.Security/src/|/Zongsoft/Zongsoft.Data/src/";

			return provider;
		}
	}
}
