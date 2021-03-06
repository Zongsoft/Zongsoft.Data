﻿using System;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Metadata.Profiles;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class MetadataFileResolverTest
	{
		[Fact]
		public void Test()
		{
			var filePath = @"/Zongsoft/Zongsoft.Security/src/Zongsoft.Security(Official).mapping";
			var metadata = MetadataFileResolver.Default.Resolve(filePath, null);

			Assert.NotNull(metadata);
		}
	}
}
