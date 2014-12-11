using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataCountParameter : DataParameter
	{
		#region 成员字段
		private ICondition _condition;
		private string _scope;
		#endregion

		#region 构造函数
		public DataCountParameter(string qualifiedName, ICondition condition, string scope = null) : base(qualifiedName)
		{
			_condition = condition;
			_scope = scope;
		}
		#endregion

		#region 公共属性
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
		#endregion
	}
}
