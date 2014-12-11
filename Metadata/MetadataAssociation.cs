using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataAssociation : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private MetadataAssociationEndCollection _members;
		#endregion

		#region 构造函数
		public MetadataAssociation(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_members = new MetadataAssociationEndCollection(this);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置关联元素的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		/// <summary>
		/// 获取关系元素的全称，即为“容器名.元素名”。
		/// </summary>
		public string FullName
		{
			get
			{
				var container = this.Container;

				if(container == null || string.IsNullOrWhiteSpace(container.Name))
					return _name;

				return container.Name + "." + _name;
			}
		}

		/// <summary>
		/// 获取关联元素的成员集。
		/// </summary>
		public MetadataAssociationEndCollection Members
		{
			get
			{
				return _members;
			}
		}

		/// <summary>
		/// 获取关联元素所属的容器元素。
		/// </summary>
		public MetadataContainer Container
		{
			get
			{
				return (MetadataContainer)base.Owner;
			}
		}
		#endregion

		#region 公共方法
		public bool IsOneToMany(string from, string to)
		{
			if(string.IsNullOrWhiteSpace(from))
				throw new ArgumentNullException("from");

			if(string.IsNullOrWhiteSpace(to))
				throw new ArgumentNullException("to");

			var fromMember = this.Members[from];
			var toMember = this.Members[to];

			if(fromMember == null || toMember == null)
				return false;

			return ((fromMember.Multiplicity == MetadataAssociationMultiplicity.One || fromMember.Multiplicity == MetadataAssociationMultiplicity.ZeroOrOne) && toMember.Multiplicity == MetadataAssociationMultiplicity.Many) ||
			       ((toMember.Multiplicity == MetadataAssociationMultiplicity.One || toMember.Multiplicity == MetadataAssociationMultiplicity.ZeroOrOne) && fromMember.Multiplicity == MetadataAssociationMultiplicity.Many);
		}

		public bool IsOneToOne(string from, string to)
		{
			if(string.IsNullOrWhiteSpace(from))
				throw new ArgumentNullException("from");

			if(string.IsNullOrWhiteSpace(to))
				throw new ArgumentNullException("to");

			var fromMember = this.Members[from];
			var toMember = this.Members[to];

			if(fromMember == null || toMember == null)
				return false;

			return (fromMember.Multiplicity == MetadataAssociationMultiplicity.One || fromMember.Multiplicity == MetadataAssociationMultiplicity.ZeroOrOne) &&
				   (toMember.Multiplicity == MetadataAssociationMultiplicity.One || toMember.Multiplicity == MetadataAssociationMultiplicity.ZeroOrOne);
		}
		#endregion
	}
}
