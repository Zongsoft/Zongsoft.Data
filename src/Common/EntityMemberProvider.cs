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
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Data.Common
{
	public class EntityMemberProvider
	{
		#region 单例模式
		public static readonly EntityMemberProvider Instance = new EntityMemberProvider();
		#endregion

		#region 成员字段
		private readonly ConcurrentDictionary<Type, Collections.INamedCollection<EntityMember>> _cache;
		#endregion

		#region 构造函数
		private EntityMemberProvider()
		{
			_cache = new ConcurrentDictionary<Type, Collections.INamedCollection<EntityMember>>();
		}
		#endregion

		#region 公共方法
		public Collections.INamedCollection<EntityMember> GetMembers(Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			//如果指定的类型是单值类型则返回空
			if(Zongsoft.Common.TypeExtension.IsScalarType(type))
				return null;

			return _cache.GetOrAdd(type, key => this.CreateMembers(key));
		}
		#endregion

		#region 私有方法
		private Collections.INamedCollection<EntityMember> CreateMembers(Type type)
		{
			//如果是字典则返回空
			if(Zongsoft.Common.TypeExtension.IsDictionary(type))
				return null;

			if(Zongsoft.Common.TypeExtension.IsEnumerable(type))
				type = Zongsoft.Common.TypeExtension.GetElementType(type);

			var members = this.FindMembers(type);
			var tokens = new Collections.NamedCollection<EntityMember>(item => item.Name);

			foreach(var member in members)
			{
				var token = this.CreateMemberToken(member);

				if(token != null)
					tokens.Add(token.Value);
			}

			return tokens;
		}

		private EntityMember? CreateMemberToken(MemberInfo member)
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

		private IEnumerable<MemberInfo> FindMembers(Type type)
		{
			foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
				yield return field;

			foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				yield return property;

			if(type.IsInterface)
			{
				var contracts = type.GetInterfaces();

				foreach(var contract in contracts)
				{
					foreach(var property in contract.GetProperties(BindingFlags.Public | BindingFlags.Instance))
						yield return property;
				}
			}
		}
		#endregion
	}
}
