using System;
using System.ComponentModel;

namespace Zongsoft.Data.Metadata
{
	public enum MetadataAssociationMultiplicity
	{
		[Zongsoft.ComponentModel.Alias("0..1")]
		ZeroOrOne,

		[Zongsoft.ComponentModel.Alias("1")]
		One,

		[Zongsoft.ComponentModel.Alias("*")]
		Many,
	}
}
