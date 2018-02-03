using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class FieldIdentifier
	{
		public TableIdentifier Table
		{
			get;
		}

		public string Name
		{
			get;
		}

		public string Alias
		{
			get;
			set;
		}
	}
}
