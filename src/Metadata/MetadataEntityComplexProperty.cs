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
	/// <summary>
	/// 表示复合属性元素的类。
	/// </summary>
	public class MetadataEntityComplexProperty : MetadataEntityProperty
	{
		#region 成员字段
		private RelationshipElement _relationship;
		#endregion

		#region 构造函数
		public MetadataEntityComplexProperty(string name, string associationName, string from, string to) : base(name)
		{
			if(string.IsNullOrWhiteSpace(associationName))
				throw new ArgumentNullException("associationName");

			_relationship = new RelationshipElement(this, associationName, from, to);
		}
		#endregion

		#region 公共属性
		public RelationshipElement Relationship
		{
			get
			{
				return _relationship;
			}
		}
		#endregion

		#region 嵌套子类
		public class RelationshipElement : MarshalByRefObject
		{
			#region 成员字段
			private MetadataEntityComplexProperty _owner;
			private MetadataAssociation _association;
			private string _associationName;
			private string _from;
			private string _to;
			#endregion

			#region 构造函数
			public RelationshipElement(MetadataEntityComplexProperty owner, string associationName, string principal, string dependent)
			{
				if(owner == null)
					throw new ArgumentNullException("owner");

				if(string.IsNullOrWhiteSpace(associationName))
					throw new ArgumentNullException("associationName");

				_owner = owner;
				_associationName = associationName.Trim();
				_from = principal;
				_to = dependent;
			}
			#endregion

			#region 公共属性
			public MetadataEntityComplexProperty Owner
			{
				get
				{
					return _owner;
				}
			}

			public MetadataAssociation Association
			{
				get
				{
					if(_association == null)
					{
						var qualifiedName = _associationName;

						if(!qualifiedName.Contains("."))
							qualifiedName = _owner.Entity.Container.Name + _associationName;

						if(_owner.Entity.Container.Kind == MetadataElementKind.Concept)
							_association = MetadataManager.Default.GetConceptElement<MetadataAssociation>(qualifiedName);
						else
							_association = MetadataManager.Default.GetStorageElement<MetadataAssociation>(qualifiedName);
					}

					return _association;
				}
			}

			public string AssociationName
			{
				get
				{
					return _associationName;
				}
			}

			public string From
			{
				get
				{
					return _from;
				}
			}

			public string To
			{
				get
				{
					return _to;
				}
			}
			#endregion

			#region 公共方法
			public MetadataEntity GetFromEntity()
			{
				var association = this.Association;

				if(association == null)
					return null;

				return association.Members[_from].Entity;
			}

			public MetadataEntity GetToEntity()
			{
				var association = this.Association;

				if(association == null)
					return null;

				return association.Members[_to].Entity;
			}

			public IEnumerable<MetadataEntityProperty> GetFromEntityReferences()
			{
				return this.GetEntityReferences(_from);
			}

			public IEnumerable<MetadataEntityProperty> GetToEntityReferences()
			{
				return this.GetEntityReferences(_to);
			}

			public bool IsOneToOne()
			{
				return this.Association.IsOneToOne(_from, _to);
			}

			public bool IsOneToMany()
			{
				return this.Association.IsOneToMany(_from, _to);
			}

			public bool IsManyToOne()
			{
				return this.Association.IsManyToOne(_from, _to);
			}

			public bool IsManyToMany()
			{
				return this.Association.IsManyToMany(_from, _to);
			}
			#endregion

			#region 私有方法
			private MetadataEntityProperty[] GetEntityReferences(string memberName)
			{
				var association = this.Association;

				if(association == null)
					return null;

				var entity = association.Members[memberName].Entity;
				var refNames = association.Members[memberName].Properties;
				var refs = new MetadataEntityProperty[refNames.Length];

				for(int i = 0; i < refs.Length; i++)
				{
					refs[i] = entity.Properties[refNames[i]];
				}

				return refs;
			}
			#endregion
		}
		#endregion
	}
}
