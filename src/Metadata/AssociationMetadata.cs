using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class AssociationMetadata
	{
		#region 成员字段
		private EntityMetadata _principal;
		private EntityMetadata _foreign;
		private AssociationMemberMetadata[] _members;
		#endregion

		#region 构造函数
		public AssociationMetadata(EntityMetadata principal, string[] principalKeys, EntityMetadata foreign, string[] foreignKeys)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_foreign = foreign ?? throw new ArgumentNullException(nameof(foreign));

			if(principalKeys == null || principalKeys.Length == 0)
				throw new ArgumentNullException(nameof(principalKeys));
			if(foreignKeys == null || foreignKeys.Length == 0)
				throw new ArgumentNullException(nameof(foreignKeys));
			if(principalKeys.Length != foreignKeys.Length)
				throw new ArgumentException();

			_members = new AssociationMemberMetadata[principalKeys.Length];

			for(int i = 0; i < principalKeys.Length; i++)
			{
				var principalKey = principal.Properties[principalKeys[i]];

				if(principalKey == null)
					throw new ArgumentException();
				if(!principalKey.IsSimplex)
					throw new ArgumentException();

				var foreignKey = foreign.Properties[foreignKeys[i]];

				if(foreignKey == null)
					throw new ArgumentException();
				if(!foreignKey.IsSimplex)
					throw new ArgumentException();

				_members[i] = new AssociationMemberMetadata((EntitySimplexPropertyMetadata)principalKey, (EntitySimplexPropertyMetadata)foreignKey);
			}
		}
		#endregion

		#region 公共属性
		public EntityMetadata Principal
		{
			get
			{
				return _principal;
			}
		}

		public EntityMetadata Foreign
		{
			get
			{
				return _foreign;
			}
		}

		public AssociationMemberMetadata[] Members
		{
			get
			{
				return _members;
			}
		}
		#endregion

		#region 嵌套子类
		public class AssociationMemberMetadata
		{
			#region 成员字段
			private EntitySimplexPropertyMetadata _principalKey;
			private EntitySimplexPropertyMetadata _foreignKey;
			#endregion

			#region 构造函数
			public AssociationMemberMetadata(EntitySimplexPropertyMetadata principalKey, EntitySimplexPropertyMetadata foreignKey)
			{
				_principalKey = principalKey ?? throw new ArgumentNullException(nameof(principalKey));
				_foreignKey = foreignKey ?? throw new ArgumentNullException(nameof(foreignKey));
			}
			#endregion

			#region 公共属性
			public EntitySimplexPropertyMetadata PrincipalKey
			{
				get
				{
					return _principalKey;
				}
			}

			public EntitySimplexPropertyMetadata ForeignKey
			{
				get
				{
					return _foreignKey;
				}
			}
			#endregion
		}
		#endregion
	}
}
