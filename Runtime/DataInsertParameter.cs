using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataInsertParameter : DataParameter
	{
		#region 成员字段
		private object _value;
		private string _scope;
		#endregion

		#region 构造函数
		public DataInsertParameter(string qualifiedName, object value, string scope) : base(qualifiedName)
		{
			_value = value;
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
