using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data.Metadata.Tests
{
	public class MetadataFileResolverTest
	{
		[Fact]
		public void Test()
		{
			var filePath = @"/Zongsoft/Zongsoft.Security/src/Zongsoft.Security(Official).mapping";
			var metadata = Profiles.MetadataFileResolver.Default.Resolve(filePath);

			Assert.NotNull(metadata);

			using(var manager = new Profiles.MetadataFileManager(@"/temp/"))
			{
				Assert.True(manager.Providers.Count > 0);
			}
		}
	}
}
