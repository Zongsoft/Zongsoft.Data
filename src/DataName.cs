using System;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class DataName
	{
		#region 成员字段
		private string _elementName;
		private string _containerName;
		private string _namespace;
		#endregion

		#region 构造函数
		private DataName(string elementName, string containerName, string @namespace)
		{
			if(string.IsNullOrWhiteSpace(elementName))
				throw new ArgumentNullException("elementName");

			_elementName = elementName.Trim();
			_containerName = containerName.Trim();
			_namespace = @namespace.Trim();
		}
		#endregion

		#region 公共属性
		public string QualifiedName
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_namespace))
				{
					if(string.IsNullOrWhiteSpace(_containerName))
						return _elementName;
					else
						return _containerName + "." + _elementName;
				}
				else
				{
					if(string.IsNullOrWhiteSpace(_containerName))
						return _elementName + "@" + _namespace;
					else
						return _containerName + "." + _elementName + "@" + _namespace;
				}
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

		#region 静态方法
		public static DataName Parse(string qualifiedName)
		{
			if(string.IsNullOrWhiteSpace(qualifiedName))
				throw new ArgumentNullException("qualifiedName");

			string elementName, containerName, @namespace;

			Parse(qualifiedName, out elementName, out containerName, out @namespace);

			return new DataName(elementName, containerName, @namespace);
		}

		public static void Parse(string qualifiedName, out string elementName, out string containerName, out string @namespace)
		{
			if(string.IsNullOrWhiteSpace(qualifiedName))
				throw new ArgumentNullException("qualifiedName");

			//初始化输出参数值
			elementName = string.Empty;
			containerName = string.Empty;
			@namespace = string.Empty;

			var parts = qualifiedName.Split('@');

			if(parts.Length > 2)
				throw new ArgumentException(string.Format("The '{0}' parameter is invalid.", qualifiedName));

			if(string.IsNullOrWhiteSpace(parts[0]))
				throw new ArgumentException("The path part of qualifiedName is empty.");

			if(parts.Length > 1)
				@namespace = parts[1].Trim();

			//拆解路径部分(即容器名和元素名)
			var index = parts[0].LastIndexOf('.');

			if(index < 0)
			{
				elementName = parts[0].Trim();
				containerName = string.Empty;
			}
			else
			{
				elementName = parts[0].Substring(index + 1).Trim();

				if(string.IsNullOrWhiteSpace(elementName))
					throw new ArithmeticException("The name part of qualifiedName is empty.");

				containerName = parts[0].Substring(0, index).Trim();
			}
		}
		#endregion
	}
}
