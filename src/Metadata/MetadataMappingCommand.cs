using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataMappingCommand : MetadataMapping
	{
		#region 成员字段
		private MetadataCommand _conceptCommand;
		private MetadataCommand _storageCommand;
		private MetadataMappingCommandParameterCollection _parameters;
		#endregion

		#region 构造函数
		public MetadataMappingCommand(MetadataFile file, string conceptEntityName, string storageEntityName) : base(file, conceptEntityName, storageEntityName)
		{
			_parameters = new MetadataMappingCommandParameterCollection(this);
		}
		#endregion

		#region 公共属性
		public MetadataCommand ConceptCommand
		{
			get
			{
				if(_conceptCommand == null)
				{
					_conceptCommand = (MetadataCommand)this.GetMappedElement(this.ConceptElementPath,
					                  (name, @namespace) => MetadataManager.Default.GetConceptContainer(name, @namespace),
					                  (container, name) => container.Commands[name]);
				}

				return _conceptCommand;
			}
		}

		public MetadataCommand StorageCommand
		{
			get
			{
				if(_storageCommand == null)
				{
					_storageCommand = (MetadataCommand)this.GetMappedElement(this.StorageElementPath,
					                  (name, @namespace) => MetadataManager.Default.GetStorageContainer(name, @namespace),
					                  (container, name) => container.Commands[name]);
				}

				return _conceptCommand;
			}
		}

		public MetadataMappingCommandParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}
		#endregion
	}
}
