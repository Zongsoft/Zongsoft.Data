using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class EntityComplexPropertyMetadata : EntityPropertyMetadata
	{
		#region 成员字段
		private bool _isMultiple;
		private AssociationMetadata _relationship;
		#endregion

		#region 构造函数
		public EntityComplexPropertyMetadata(EntityMetadata entity, string name, Type type) : base(entity, name, type)
		{
			_isMultiple = false;
		}

		public EntityComplexPropertyMetadata(EntityMetadata entity, string name, Type type, bool isMultiple) : base(entity, name, type)
		{
			_isMultiple = isMultiple;
		}
		#endregion

		#region 公共属性
		public bool IsMultiple
		{
			get
			{
				return _isMultiple;
			}
		}

		public AssociationMetadata Relationship
		{
			get
			{
				return _relationship;
			}
		}
		#endregion

		#region 重写属性
		public override bool IsComplex
		{
			get
			{
				return false;
			}
		}

		public override bool IsSimplex
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}
