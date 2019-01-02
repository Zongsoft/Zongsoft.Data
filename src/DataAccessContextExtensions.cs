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
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;

using Zongsoft.Reflection;
using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data
{
	public static class DataAccessContextExtension
	{
		/// <summary>
		/// 创建语句对应的 <see cref="DbCommand"/> 数据命令。
		/// </summary>
		/// <param name="context">指定的数据访问上下文。</param>
		/// <param name="statement">指定要创建命令的语句。</param>
		/// <param name="connectionOpend">是否自动打开数据命令关联的数据连接。</param>
		/// <returns>返回创建的数据命令。</returns>
		public static DbCommand Build(this IDataAccessContext context, IStatement statement, bool connectionOpend = false)
		{
			///创建指定语句的数据命令对象
			var command = context.Source.Driver.CreateCommand(statement);

			//设置数据命令关联的数据库连接对象
			command.Connection = context.Source.ConnectionManager.Acquire(context);

			//确保数据命名的连接被打开
			if(connectionOpend && command.Connection.State == System.Data.ConnectionState.Closed)
				command.Connection.Open();

			return command;
		}
	}

	[Obsolete]
	public static class DataSelectContextExtension
	{
		#region 公共方法
		[Obsolete]
		public static IEnumerable<string> ResolveScope(this DataSelectContext context)
		{
			var provider = context.Provider;
			var entity = context.Entity;
			var members = context.GetEntityMembers();

			IEnumerable<string> Resolve(string wildcard)
			{
				foreach(var property in entity.Properties.Where(p => p.IsSimplex && (members == null || members.Contains(p.Name))))
				{
					yield return property.Name;
				}

				var baseName = entity.BaseName;

				while(baseName != null && baseName.Length > 0)
				{
					if(!entity.Metadata.Entities.TryGet(entity.BaseName, out var baseEntity))
						baseEntity = provider.Metadata.Entities.Get(entity.BaseName);

					foreach(var property in baseEntity.Properties.Where(p => p.IsSimplex && (members == null || members.Contains(p.Name))))
					{
						//忽略父表中的主键
						if(!property.IsPrimaryKey)
							yield return property.Name;
					}

					baseName = baseEntity.BaseName;
				}
			}

			//return Scoping.Parse(context.Scope).Map(Resolve);
			return Scoping.Parse(context.Schema).Map(Resolve);
		}

		public static MemberTokenCollection GetEntityMembers(this DataSelectContextBase context, string path = null)
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
