using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class SelectClause
	{
		#region 成员字段
		private IList<IExpression> _members;
		#endregion

		#region 构造函数
		public SelectClause()
		{
			_members = new List<IExpression>();
		}

		public SelectClause(bool isDistinct)
		{
			this.IsDistinct = isDistinct;
			_members = new List<IExpression>();
		}
		#endregion

		#region 公共属性
		public bool IsDistinct
		{
			get;
			set;
		}

		public IList<IExpression> Members
		{
			get
			{
				return _members;
			}
		}
		#endregion
	}
}
