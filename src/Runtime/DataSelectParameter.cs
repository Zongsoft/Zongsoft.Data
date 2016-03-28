using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataSelectParameter : DataParameter
	{
		#region 成员字段
		private ICondition _condition;
		private string _scope;
		private Paging _paging;
		private Sorting[] _sortings;
		private Type _entityType;
		#endregion

		#region 构造函数
		public DataSelectParameter(string fullName, ICondition condition, string scope, Paging paging, Sorting[] sortings) : base(fullName)
		{
			_condition = condition;
			_scope = scope;
			_paging = paging;
			_sortings = sortings;
		}
		#endregion

		#region 公共属性
		public Type EntityType
		{
			get
			{
				return _entityType;
			}
			set
			{
				_entityType = value;
			}
		}

		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		public string Scope
		{
			get
			{
				return _scope;
			}
		}

		public Paging Paging
		{
			get
			{
				return _paging;
			}
			set
			{
				_paging = value;
			}
		}

		public Sorting[] Sortings
		{
			get
			{
				return _sortings;
			}
		}
		#endregion
	}
}
