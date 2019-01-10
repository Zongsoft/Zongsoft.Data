using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Tests
{
	internal static class Utility
	{
		public static Zongsoft.Data.Common.IDataProvider GetProvider(string applicationName)
		{
			if(string.IsNullOrEmpty(applicationName))
				throw new ArgumentNullException(nameof(applicationName));

			DataEnvironment.Drivers.Add(new Dummy.DummyDriver());

			var provider = DataEnvironment.Providers.GetProvider(applicationName);
			provider.Multiplexer = Dummy.DummyMultiplexer.Instance;

			if(provider.Metadata.Loader is Metadata.Profiles.MetadataFileLoader loader)
				loader.Path = @"/Zongsoft/Zongsoft.Data/src/|/Zongsoft/Zongsoft.Security/src/|/Zongsoft/Zongsoft.Community/src/|";

			return provider;
		}

		public static IEnumerable<Common.Expressions.IStatement> Build(this IDataAccessContext context)
		{
			return context.Source.Driver.Builder.Build(context);
		}
	}
}
