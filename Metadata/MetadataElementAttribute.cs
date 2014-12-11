using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataElementAttribute
	{
		#region 成员字段
		private string _localName;
		private string _prefix;
		private string _name;
		private string _namespaceUri;
		private string _value;
		#endregion

		#region 构造函数
		public MetadataElementAttribute(string prefix, string localName, string value, string namespaceUri)
		{
			if(string.IsNullOrWhiteSpace(localName))
				throw new ArgumentNullException("localName");

			_prefix = prefix;
			_localName = localName;
			_name = string.IsNullOrWhiteSpace(prefix) ? localName : (prefix + ":" + localName);
			_value = value;
			_namespaceUri = namespaceUri;
		}
		#endregion

		#region 公共属性
		public string LocalName
		{
			get
			{
				return _localName;
			}
		}

		public string Prefix
		{
			get
			{
				return _prefix;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return _namespaceUri;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
		}
		#endregion
	}
}
