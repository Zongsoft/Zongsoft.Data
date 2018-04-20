using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class OrderByClause
	{
		#region 构造函数
		public OrderByClause()
		{
			this.Members = new HashSet<OrderByMember>();
		}
		#endregion

		#region 公共属性
		public ICollection<OrderByMember> Members
		{
			get;
		}
		#endregion

		#region 公共方法
		public OrderByMember Add(FieldIdentifier field, SortingMode mode = SortingMode.Ascending)
		{
			var member = new OrderByMember(field, mode);
			this.Members.Add(member);
			return member;
		}
		#endregion

		#region 嵌套结构
		public struct OrderByMember
		{
			public OrderByMember(FieldIdentifier field, SortingMode mode = SortingMode.Ascending)
			{
				this.Field = field;
				this.Mode = mode;
			}

			public FieldIdentifier Field;
			public SortingMode Mode;
		}
		#endregion
	}
}
