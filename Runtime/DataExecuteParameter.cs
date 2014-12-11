using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataExecuteParameter : DataParameter
	{
		#region 成员字段
		private IDictionary<string, object> _inParameters;
		private IDictionary<string, object> _outParameters;
		#endregion

		#region 构造函数
		public DataExecuteParameter(string qualifiedName, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters = null) : base(qualifiedName)
		{
			_inParameters = inParameters;
			_outParameters = outParameters;
		}
		#endregion

		#region 公共属性
		public IDictionary<string, object> InParameters
		{
			get
			{
				return _inParameters;
			}
		}

		public IDictionary<string, object> OutParameters
		{
			get
			{
				return _outParameters;
			}
		}
		#endregion
	}
}
