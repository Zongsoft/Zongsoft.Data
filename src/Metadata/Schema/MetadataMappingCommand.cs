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
	public class MetadataMappingCommand : MetadataMapping
	{
		#region 成员字段
		private MetadataCommand _conceptCommand;
		private MetadataCommand _storageCommand;
		private MetadataMappingCommandParameterCollection _parameters;
		#endregion

		#region 构造函数
		public MetadataMappingCommand(MetadataFile file, string conceptQualifiedName, string storageQualifiedName) : base(file, conceptQualifiedName, storageQualifiedName)
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
					_conceptCommand = MetadataManager.Default.GetConceptElement<MetadataCommand>(this.ConceptQualifiedName);

				return _conceptCommand;
			}
		}

		public MetadataCommand StorageCommand
		{
			get
			{
				if(_storageCommand == null)
					_storageCommand = MetadataManager.Default.GetStorageElement<MetadataCommand>(this.StorageQualifiedName);

				return _storageCommand;
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
