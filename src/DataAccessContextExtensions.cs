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
using System.Collections.Generic;

using Zongsoft.Reflection;
using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public static class DataAccessContextExtension
	{
		#region 公共方法
		public static IDataProvider GetProvider(this DataAccessContextBase context)
		{
			return DataEnvironment.Providers.GetProvider(context.DataAccess.Name);
		}

		public static IEntity GetEntity(this DataAccessContextBase context)
		{
			if(DataEnvironment.Metadatas.Get(context.DataAccess.Name).Entities.TryGet(context.Name, out var entity))
				return entity;

			throw new DataException($"The specified '{context.Name}' entity mapping does not exist.");
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
