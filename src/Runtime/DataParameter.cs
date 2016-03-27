using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataParameter : MarshalByRefObject
	{
		#region 成员字段
		private string _elementName;
		private string _containerName;
		private string _namespace;
		#endregion

		#region 构造函数
		protected DataParameter(string qualifiedName)
		{
			if(string.IsNullOrWhiteSpace(qualifiedName))
				throw new ArgumentNullException("qualifiedName");

			DataName.Parse(qualifiedName, out _elementName, out _containerName, out _namespace);
		}
		#endregion

		#region 公共属性
		public string QualifiedName
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_namespace))
					return _containerName + "." + _elementName;
				else
					return _containerName + "." + _elementName + "@" + _namespace;
			}
		}

		public string ElementName
		{
			get
			{
				return _elementName;
			}
		}

		public string ContainerName
		{
			get
			{
				return _containerName;
			}
		}

		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				_namespace = value == null ? string.Empty : value.Trim();
			}
		}
		#endregion
	}
}
