using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataUpdateParameter : DataParameter
	{
		#region 成员字段
		private ICondition _condition;
		private string _scope;
		private object _value;
		#endregion

		#region 构造函数
		public DataUpdateParameter(string qualifiedName, object value, ICondition condition, string scope) : base(qualifiedName)
		{
			_value = value;
			_condition = condition;
			_scope = scope;
		}
		#endregion

		#region 公共属性
		public object Value
		{
			get
			{
				return _value;
			}
		}

		public IEnumerable<object> Values
		{
			get
			{
				if(_value != null && Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), _value.GetType()))
					return _value as IEnumerable<object>;

				return System.Linq.Enumerable.Empty<object>();
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
		#endregion
	}
}
