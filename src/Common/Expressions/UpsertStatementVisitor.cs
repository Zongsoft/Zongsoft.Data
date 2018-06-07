using System;
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpsertStatementVisitor : InsertStatementVisitor
	{
		#region 构造函数
		public UpsertStatementVisitor(StringBuilder text) : base(text)
		{
		}
		#endregion

	}
}
