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
						var name = DataName.Parse(_associationName);
						var qualifiedName = _associationName;

						if(string.IsNullOrWhiteSpace(name.ContainerName))
						{
							if(!string.IsNullOrWhiteSpace(_owner.Entity.Container.Name))
								qualifiedName = _owner.Entity.Container.Name + "." + name;
						}

						if(!qualifiedName.Contains("@"))
							qualifiedName += "@" + _owner.Entity.Container.File.Namespace;

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
