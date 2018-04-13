using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data.Tests.Metadata
{
	public class MetadataResolverTest
	{
		[Fact]
		public void Test()
		{
			var filePath = @"/Zongsoft/Zongsoft.Security/src/Zongsoft.Security.mapping";
			var metadata = Zongsoft.Data.Metadata.Schema.MetadataResolver.Default.Resolve(filePath);

			Assert.NotNull(metadata);
		}
	}
}
