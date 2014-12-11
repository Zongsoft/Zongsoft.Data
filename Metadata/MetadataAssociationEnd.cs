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
