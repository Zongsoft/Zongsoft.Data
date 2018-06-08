/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

			if(Zongsoft.Common.TypeExtension.IsEnumerable(type))
				type = Zongsoft.Common.TypeExtension.GetElementType(type);

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
