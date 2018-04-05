using System;
using System.Reflection;

namespace Zongsoft.Data.Common
{
	public class EntityMemberProvider : Zongsoft.Reflection.MemberTokenProvider
	{
		#region 单例模式
		public static readonly new EntityMemberProvider Default = new EntityMemberProvider();
		#endregion

		#region 构造函数
		private EntityMemberProvider() : base(type => Zongsoft.Common.TypeExtension.IsScalarType(type))
		{
		}
		#endregion

		#region 重写方法
		protected override Reflection.MemberTokenCollection CreateMembers(Type type, Reflection.MemberKind kinds)
		{
			//如果是集合或者字典则返回空
			if(Zongsoft.Common.TypeExtension.IsCollection(type) ||
			   Zongsoft.Common.TypeExtension.IsDictionary(type))
				return null;

			//调用基类同名方法
			return base.CreateMembers(type, kinds);
		}

		protected override Reflection.MemberToken CreateMember(MemberInfo member)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					var field = (FieldInfo)member;

					if(!field.IsInitOnly)
						return new EntityMember(field, EntityEmitter.GenerateFieldSetter(field));

					break;
				case MemberTypes.Property:
					var property = (PropertyInfo)member;

					if(property.CanRead && property.CanWrite)
						return new EntityMember(property, EntityEmitter.GeneratePropertySetter(property));

					break;
			}

			return null;
		}
		#endregion
	}
}
