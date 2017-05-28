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

namespace Zongsoft.Data.Metadata
{
	public class MetadataAssociationEnd : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _entityName;
		private MetadataAssociationMultiplicity _multiplicity;
		private string[] _properties;
		private MetadataAssociationEndConstraintCollection _constraints;
		#endregion

		#region 构造函数
		public MetadataAssociationEnd(string name, string entityName, MetadataAssociationMultiplicity multiplicity = MetadataAssociationMultiplicity.One)
		{
			if(string.IsNullOrWhiteSpace(entityName))
				throw new ArgumentNullException("entityName");

			_entityName = entityName.Trim();
			_multiplicity = multiplicity;
			_name = string.IsNullOrWhiteSpace(name) ? _entityName : name.Trim();
			_constraints = new MetadataAssociationEndConstraintCollection(this);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关联顶点的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取关联顶点的实体元素。
		/// </summary>
		public MetadataEntity Entity
		{
			get
			{
				var qualifiedName = _entityName;

				if(!qualifiedName.Contains(".") && !string.IsNullOrWhiteSpace(this.Association.Container.Name))
					qualifiedName = this.Association.Container.Name + "." + qualifiedName;

				if(!qualifiedName.Contains("@"))
					qualifiedName += "@" + this.Association.Container.File.Namespace;

				if(this.Association.Container.Kind == MetadataElementKind.Concept)
					return MetadataManager.Default.GetConceptElement<MetadataEntity>(qualifiedName);
				else
					return MetadataManager.Default.GetStorageElement<MetadataEntity>(qualifiedName);
			}
		}

		/// <summary>
		/// 获取关联顶点的实体名称。
		/// </summary>
		public string EntityName
		{
			get
			{
				return _entityName;
			}
		}

		/// <summary>
		/// 获取或设置关联顶点的复合度。
		/// </summary>
		public MetadataAssociationMultiplicity Multiplicity
		{
			get
			{
				return _multiplicity;
			}
			set
			{
				_multiplicity = value;
			}
		}

		/// <summary>
		/// 获取关联顶点所属的关系对象。
		/// </summary>
		public MetadataAssociation Association
		{
			get
			{
				return (MetadataAssociation)base.Owner;
			}
		}

		/// <summary>
		/// 获取或设置关联顶点的关联属性名集合。
		/// </summary>
		public string[] Properties
		{
			get
			{
				return _properties;
			}
			set
			{
				if(value == null || value.Length < 1)
					throw new ArgumentNullException();

				_properties = value;
			}
		}

		/// <summary>
		/// 获取关联顶点的约束集合。
		/// </summary>
		public MetadataAssociationEndConstraintCollection Constraints
		{
			get
			{
				return _constraints;
			}
		}
		#endregion
	}
}
