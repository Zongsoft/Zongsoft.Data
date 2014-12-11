using System;
using System.IO;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataFile : MarshalByRefObject
	{
		#region 成员字段
		private string _url;
		private string _namespace;
		private MetadataContainerCollection _storages;
		private MetadataContainerCollection _concepts;
		private MetadataMappingCollection _mappings;
		#endregion

		#region 构造函数
		public MetadataFile(string @namespace) : this(@namespace, string.Empty)
		{
		}

		public MetadataFile(string @namespace, string url)
		{
			if(string.IsNullOrWhiteSpace(@namespace))
				throw new ArgumentNullException("namespace");

			_url = url;
			_namespace = @namespace.Trim();
			_storages = new MetadataContainerCollection(this);
			_concepts = new MetadataContainerCollection(this);
			_mappings = new MetadataMappingCollection(this);
		}
		#endregion

		#region 公共属性
		public string Url
		{
			get
			{
				return _url;
			}
		}

		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}

		public MetadataContainerCollection Storages
		{
			get
			{
				return _storages;
			}
		}

		public MetadataContainerCollection Concepts
		{
			get
			{
				return _concepts;
			}
		}

		public MetadataMappingCollection Mappings
		{
			get
			{
				return _mappings;
			}
		}
		#endregion

		#region 静态方法
		public static MetadataFile Load(Stream stream)
		{
			return MetadataResolver.Default.Resolve(stream);
		}

		public static MetadataFile Load(TextReader reader)
		{
			return MetadataResolver.Default.Resolve(reader);
		}

		public static MetadataFile Load(System.Xml.XmlReader reader)
		{
			return MetadataResolver.Default.Resolve(reader);
		}

		public static MetadataFile Load(string filePath)
		{
			return MetadataResolver.Default.Resolve(filePath);
		}
		#endregion
	}
}
