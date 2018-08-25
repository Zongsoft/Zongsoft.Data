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

namespace Zongsoft.Data.Metadata
{
	public static class EntityPorpertyCollectionExtension
	{
		/// <summary>
		///		<para>查找属性集合中指定成员路径对应的属性。</para>
		///		<para>注：查找范围包括父实体的属性集。</para>
		/// </summary>
		/// <param name="properties">指定要查找的属性集合。</param>
		/// <param name="path">指定要查找的成员路径，支持多级导航属性路径。</param>
		/// <param name="match">属性匹配成功后的回调函数，其各参数表示：
		///		<list type="number">
		///			<item>第一个参数，表示当前匹配属性的成员路径（注意：不含当前属性名，即不是全路径）；</item>
		///			<item>第二个参数，表示当前匹配属性的源实体，如果该参数值与匹配属性所在的实体不同，则说明匹配属性位于源实体的父实体或导航属性的父实体中。</item>
		///			<item>第三个参数，表示当前匹配到的属性。</item>
		///		</list>
		///		<para>
		///		回调函数的返回值为空(null)，表示查找方法继续后续的匹配；<br/>
		///		如果为真(true)则当前查找方法立即退出，并返回当前匹配到的属性；<br/>
		///		如果为假(False)则当前查找方法立即退出，并返回空(null)，即查找失败。
		///		</para>
		/// </param>
		/// <returns>返回找到的属性。</returns>
		public static IEntityPropertyMetadata Find(this IEntityPropertyMetadataCollection properties, string path, Action<string, IEntityMetadata, IEntityPropertyMetadata> match = null)
		{
			if(string.IsNullOrEmpty(path))
				return null;

			IEntityPropertyMetadata property = null;
			var parts = path.Split('.');

			for(int i = 0; i < parts.Length; i++)
			{
				if(properties == null)
					return null;

				var parent = properties.Entity;

				//如果当前属性集合中不包含指定的属性，则尝试从父实体中查找
				if(!properties.TryGet(parts[i], out property))
				{
					//尝试从父实体中查找指定的属性
					property = FindBaseProperty(ref properties, parts[i]);

					//如果父实体中也不含指定的属性则返回失败
					if(property == null)
						return null;
				}

				//调用匹配回调函数
				match?.Invoke(string.Join(".", parts, 0, i), parent, property);

				if(property.IsSimplex)
					properties = null;
				else
					properties = GetAssociatedProperties((IEntityComplexPropertyMetadata)property);
			}

			//返回查找到的属性
			return property;
		}

		#region 私有方法
		private static IEntityPropertyMetadataCollection GetAssociatedProperties(IEntityComplexPropertyMetadata property)
		{
			var index = property.Role.IndexOf(':');
			var entityName = index < 0 ? property.Role : property.Role.Substring(0, index);

			if(!property.Entity.Metadata.Manager.Entities.TryGet(entityName, out var entity))
				throw new DataException($"The '{entityName}' target entity associated with the Role in the '{property.Entity.Name}:{property.Name}' complex property does not exist.");

			if(index < 0)
				return entity.Properties;

			var parts = property.Role.Substring(index + 1).Split('.');
			var properties = entity.Properties;

			foreach(var part in parts)
			{
				if(properties == null)
					return null;

				if(!properties.TryGet(part, out var found))
				{
					found = FindBaseProperty(ref properties, part);

					if(found == null)
						throw new DataException($"The '{part}' property of '{properties.Entity.Name}' entity does not existed.");
				}

				if(found.IsSimplex)
					return null;

				properties = GetAssociatedProperties((IEntityComplexPropertyMetadata)found);
			}

			return properties;
		}

		private static IEntityPropertyMetadata FindBaseProperty(ref IEntityPropertyMetadataCollection properties, string name)
		{
			if(properties == null)
				return null;

			var metadata = properties.Entity.Metadata.Manager;

			while(!string.IsNullOrEmpty(properties.Entity.BaseName) &&
				  metadata.Entities.TryGet(properties.Entity.BaseName, out var baseEntity))
			{
				properties = baseEntity.Properties;

				if(properties.TryGet(name, out var property))
					return property;
			}

			return null;
		}
		#endregion
	}
}
