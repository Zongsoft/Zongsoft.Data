using System;
using System.ComponentModel;

namespace Zongsoft.Data.Metadata
{
	public enum AssociationMultiplicity
	{
		[Zongsoft.ComponentModel.Alias("?")]
		ZeroOrOne,

		[Zongsoft.ComponentModel.Alias("1")]
		One,

		[Zongsoft.ComponentModel.Alias("*")]
		Many,
	}
}
