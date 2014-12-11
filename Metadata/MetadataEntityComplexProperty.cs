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
						return MetadataManager.Default.GetConceptElement<MetadataAssociation>(qualifiedName);
					else
						return MetadataManager.Default.GetStorageElement<MetadataAssociation>(qualifiedName);
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
		}
		#endregion
	}
}
