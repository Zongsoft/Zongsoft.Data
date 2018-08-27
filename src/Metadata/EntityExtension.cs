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

using Zongsoft.Collections;

namespace Zongsoft.Data.Metadata
{
	public static class EntityExtension
	{
		private static readonly ConcurrentDictionary<IEntityMetadata, EntityTokenCache> _cache = new ConcurrentDictionary<IEntityMetadata, EntityTokenCache>();

		public static IEntityMetadata GetBaseEntity(this IEntityMetadata entity)
		{
			if(entity == null && string.IsNullOrEmpty(entity.BaseName))
				return null;

			if(entity.Metadata.Entities.TryGet(entity.BaseName, out var baseEntity))
				return baseEntity;

			if(entity.Metadata.Manager.Entities.TryGet(entity.BaseName, out baseEntity))
				return baseEntity;

			throw new DataException($"The '{entity.BaseName}' base of '{entity.Name}' entity does not exist.");
		}

		public static IEntityMetadata[] GetInherits(this IEntityMetadata entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			if(string.IsNullOrEmpty(entity.BaseName))
				return new[] { entity };

			var super = entity;
			var stack = new Stack<IEntityMetadata>();

			while(super != null)
			{
				stack.Push(super);
				super = GetBaseEntity(super);
			}

			return stack.ToArray();
		}

		public static string GetTableName(this IEntityMetadata entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			return string.IsNullOrEmpty(entity.Alias) ? entity.Name : entity.Alias;
		}

		public static IReadOnlyNamedCollection<EntityPropertyToken> GetTokens(this IEntityMetadata entity, Type type)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			if(type == null)
				throw new ArgumentNullException(nameof(type));

			return _cache.GetOrAdd(entity, e => new EntityTokenCache(e)).GetTokens(type);
		}

		private class EntityTokenCache
		{
			#region 成员字段
			private readonly IEntityMetadata _entity;
			private readonly ConcurrentDictionary<Type, IReadOnlyNamedCollection<EntityPropertyToken>> _cache = new ConcurrentDictionary<Type, IReadOnlyNamedCollection<EntityPropertyToken>>();
			#endregion

			#region 构造函数
			internal EntityTokenCache(IEntityMetadata entity)
			{
				_entity = entity;
			}
			#endregion

			#region 公共方法
			public IReadOnlyNamedCollection<EntityPropertyToken> GetTokens(Type type)
			{
				if(type == null)
					throw new ArgumentNullException(nameof(type));

				return _cache.GetOrAdd(type, t => CreateTokens(t));
			}
			#endregion

			#region 私有方法
			private IReadOnlyNamedCollection<EntityPropertyToken> CreateTokens(Type type)
			{
				var collection = new NamedCollection<EntityPropertyToken>(m => m.Property.Name);

				if(type == null || Zongsoft.Common.TypeExtension.IsDictionary(type))
				{
					foreach(var property in _entity.Properties)
					{
						collection.Add(new EntityPropertyToken(property, null));
					}
				}
				else
				{
					foreach(var property in _entity.Properties)
					{
						var member = (MemberInfo)type.GetField(property.Name, BindingFlags.Public | BindingFlags.Instance) ??
												 type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);

						if(member != null)
							collection.Add(new EntityPropertyToken(property, member));
					}
				}

				return collection;
			}
			#endregion
		}
	}
}
