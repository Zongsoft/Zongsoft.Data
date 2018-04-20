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

using Zongsoft.Reflection;
using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public static class DataAccessContextExtension
	{
		#region 常量定义
		private const string KEY_ENTITY_STATE = "$Entity";
		#endregion

		#region 公共方法
		public static IEntity GetEntity(this DataAccessContextBase context)
		{
			if(context.States.TryGetValue(KEY_ENTITY_STATE, out var state) && state is IEntity entity)
				return entity;

			return (IEntity)(context.States[KEY_ENTITY_STATE] = DataEnvironment.Metadata.Entities.Get(context.Name));
		}

		public static MemberTokenCollection GetEntityMembers(this DataSelectionContext context, Type type, string path = null)
		{
			if(string.IsNullOrEmpty(path))
				return EntityMemberProvider.Default.GetMembers(type);

			var member = EntityMemberProvider.Default.GetMember(type, path);

			if(member != null)
				return EntityMemberProvider.Default.GetMembers(member.Type);

			return null;
		}
		#endregion
	}

	public static class DataSelectContextExtension
	{
		#region 公共方法
		public static MemberToken GetEntityMember(this DataSelectionContext context, string path)
		{
			return EntityMemberProvider.Default.GetMember(context.EntityType, path);
		}

		public static MemberTokenCollection GetEntityMembers(this DataSelectionContext context, string path = null)
		{
			if(string.IsNullOrEmpty(path))
				return EntityMemberProvider.Default.GetMembers(context.EntityType);

			var member = EntityMemberProvider.Default.GetMember(context.EntityType, path);

			if(member != null)
				return EntityMemberProvider.Default.GetMembers(member.Type);

			return null;
		}
		#endregion
	}
}
