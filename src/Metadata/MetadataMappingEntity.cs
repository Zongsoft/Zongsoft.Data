using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示实体映射元素的类。
	/// </summary>
	public class MetadataMappingEntity : MetadataMapping
	{
		#region 成员字段
		private MetadataEntity _conceptEntity;
		private MetadataEntity _storageEntity;
		private MetadataMappingEntityPropertyCollection _properties;
		private CommandElement _deleteCommand;
		private CommandElement _insertCommand;
		private CommandElement _updateCommand;
		private CommandElement _selectCommand;
		#endregion

		#region 构造函数
		public MetadataMappingEntity(MetadataFile file, string conceptEntityName, string storageEntityName) : base(file, conceptEntityName, storageEntityName)
		{
			_properties = new MetadataMappingEntityPropertyCollection(this);
		}
		#endregion

		#region 公共属性
		public MetadataEntity ConceptEntity
		{
			get
			{
				if(_conceptEntity == null)
				{
					_conceptEntity = (MetadataEntity)this.GetMappedElement(this.ConceptElementPath,
					                 (name, @namespace) => MetadataManager.Default.GetConceptContainer(name, @namespace),
					                 (container, name) => container.Entities[name]);
				}

				return _conceptEntity;
			}
		}

		public MetadataEntity StorageEntity
		{
			get
			{
				if(_storageEntity == null)
				{
					_storageEntity = (MetadataEntity)this.GetMappedElement(this.StorageElementPath,
					                 (name, @namespace) => MetadataManager.Default.GetStorageContainer(name, @namespace),
					                 (container, name) => container.Entities[name]);
				}

				return _storageEntity;
			}
		}

		public MetadataMappingEntityPropertyCollection Properties
		{
			get
			{
				return _properties;
			}
		}

		public CommandElement DeleteCommand
		{
			get
			{
				return _deleteCommand;
			}
			set
			{
				_deleteCommand = value;
			}
		}

		public CommandElement InsertCommand
		{
			get
			{
				return _insertCommand;
			}
			set
			{
				_insertCommand = value;
			}
		}

		public CommandElement UpdateCommand
		{
			get
			{
				return _updateCommand;
			}
			set
			{
				_updateCommand = value;
			}
		}

		public CommandElement SelectCommand
		{
			get
			{
				return _selectCommand;
			}
			set
			{
				_selectCommand = value;
			}
		}
		#endregion

		#region 内部方法
		internal CommandElement CreateCommandElement(string qualifiedName)
		{
			if(string.IsNullOrWhiteSpace(qualifiedName))
				throw new ArgumentNullException("qualifiedName");

			return new CommandElement(this, DataName.Parse(qualifiedName),
				   new Lazy<MetadataCommand>(() =>
				   {
					   return (MetadataCommand)this.GetMappedElement(qualifiedName,
						      (name, @namespace) => MetadataManager.Default.GetStorageContainer(name, @namespace),
						      (container, name) => container.Commands[name]);
				   }));
		}
		#endregion

		#region 嵌套子类
		public class CommandElement : MetadataElementBase
		{
			#region 成员字段
			private MetadataMappingEntity _mapping;
			private DataName _name;
			private Lazy<MetadataCommand> _command;
			private CommandParameterElementCollection _parameters;
			#endregion

			#region 构造函数
			internal CommandElement(MetadataMappingEntity mapping, DataName name, Lazy<MetadataCommand> command)
			{
				_mapping = mapping;
				_name = name;
				_command = command;
				_parameters = new CommandParameterElementCollection(this);
			}
			#endregion

			#region 公共属性
			public DataName Name
			{
				get
				{
					return _name;
				}
			}

			public MetadataCommand Command
			{
				get
				{
					if(_command == null)
						return null;

					return _command.Value;
				}
			}

			public CommandParameterElementCollection Parameters
			{
				get
				{
					return _parameters;
				}
			}
			#endregion
		}

		public class CommandParameterElement : MetadataElementBase
		{
			#region 成员字段
			private string _name;
			private string _mappedTo;
			#endregion

			#region 构造函数
			internal CommandParameterElement(string name, string mappedTo)
			{
				if(string.IsNullOrWhiteSpace(name))
					throw new ArgumentNullException("name");

				if(string.IsNullOrWhiteSpace(mappedTo))
					throw new ArgumentNullException("mappedTo");

				_name = name.Trim();
				_mappedTo = mappedTo.Trim();
			}
			#endregion

			#region 公共属性
			public string Name
			{
				get
				{
					return _name;
				}
			}

			public string MappedTo
			{
				get
				{
					return _mappedTo;
				}
			}
			#endregion
		}

		public class CommandParameterElementCollection : MetadataElementCollectionBase<CommandParameterElement>
		{
			public CommandParameterElementCollection(CommandElement mapping) : base(mapping)
			{
			}

			protected override string GetKeyForItem(CommandParameterElement item)
			{
				return item.Name;
			}
		}
		#endregion
	}
}
