using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataAssociationEndConstraint : MetadataElementBase
	{
		#region 成员字段
		private string _propertyName;
		private string _value;
		private ConditionOperator _operator;
		#endregion

		#region 构造函数
		public MetadataAssociationEndConstraint(string propertyName, string value, ConditionOperator @operator = ConditionOperator.Equal)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName");

			if(string.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException("value");

			_propertyName = propertyName.Trim();
			_value = value.Trim();
			_operator = @operator;
		}
		#endregion

		#region 公共属性
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
		}

		public ConditionOperator Operator
		{
			get
			{
				return _operator;
			}
		}
		#endregion
	}
}
