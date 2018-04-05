using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class FieldIdentifier : IIdentifier
	{
		#region 构造函数
		public FieldIdentifier(ISource table, string name, string alias = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Table = table ?? throw new ArgumentNullException(nameof(table));
			this.Name = name.Trim();
			this.Alias = alias;
		}
		#endregion

		#region 公共属性
		public ISource Table
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
		#endregion
	}
}
