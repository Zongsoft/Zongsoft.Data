/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata.Schema
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
					_conceptEntity = MetadataManager.Default.GetConceptElement<MetadataEntity>(this.ConceptQualifiedName);

				return _conceptEntity;
			}
		}

		public MetadataEntity StorageEntity
		{
			get
			{
				if(_storageEntity == null)
					_storageEntity = MetadataManager.Default.GetStorageElement<MetadataEntity>(this.StorageQualifiedName);

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

			return new CommandElement(this, qualifiedName);
		}
		#endregion

		#region 嵌套子类
		public class CommandElement : MetadataElementBase
		{
			#region 成员字段
			private string _qualifiedName;

			private MetadataMappingEntity _mapping;
			private MetadataCommand _command;
			private CommandParameterElementCollection _parameters;
			#endregion

			#region 构造函数
			internal CommandElement(MetadataMappingEntity mapping, string qualifiedName)
			{
				_mapping = mapping;
				_qualifiedName = qualifiedName;
				_parameters = new CommandParameterElementCollection(this);
			}
			#endregion

			#region 公共属性
			public string QualifiedName
			{
				get
				{
					return _qualifiedName;
				}
			}

			public MetadataCommand Command
			{
				get
				{
					if(_command == null)
						_command = MetadataManager.Default.GetStorageElement<MetadataCommand>(_qualifiedName);

					return _command;
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
