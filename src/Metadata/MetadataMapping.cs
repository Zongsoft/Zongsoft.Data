using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示实体映射元素的类。
	/// </summary>
	public class MetadataMapping : MetadataElementBase
	{
		#region 成员字段
		private string _conceptElementName;
		private string _storageElementName;
		#endregion

		#region 构造函数
		protected MetadataMapping(MetadataFile file, string conceptElementName, string storageElementName) : base(MetadataElementKind.Mapping, file)
		{
			if(string.IsNullOrWhiteSpace(conceptElementName))
				throw new ArgumentNullException("conceptElementName");

			if(string.IsNullOrWhiteSpace(storageElementName))
				throw new ArgumentNullException("storageElementName");

			_conceptElementName = conceptElementName.Trim();
			_storageElementName = storageElementName.Trim();
		}
		#endregion

		#region 公共属性
		public string ConceptElementPath
		{
			get
			{
				return _conceptElementName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_conceptElementName = value.Trim();
			}
		}

		public string StorageElementPath
		{
			get
			{
				return _storageElementName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_storageElementName = value.Trim();
			}
		}

		public MetadataFile File
		{
			get
			{
				return (MetadataFile)base.Owner;
			}
		}
		#endregion

		#region 保护方法
		protected MetadataElementBase GetMappedElement(string qualifiedName, Func<string, string, MetadataContainer> getContainer, Func<MetadataContainer, string, MetadataElementBase> getElement)
		{
			var name = DataName.Parse(qualifiedName);
			var @namespace = string.IsNullOrWhiteSpace(name.Namespace) ? this.File.Namespace : name.Namespace;

			var container = getContainer(name.ContainerName, @namespace);

			if(container != null)
				return getElement(container, name.ElementName);

			return null;
		}
		#endregion
	}
}
