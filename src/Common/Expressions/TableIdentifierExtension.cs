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

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public static class TableIdentifierExtension
	{
		#region 公共方法
		/// <summary>
		///		<para>从指定的表标识对应的实体开始进行路径展开。</para>
		///		<para>注：展开过程包括对父实体的属性集的搜索。</para>
		/// </summary>
		/// <param name="table">指定的进行展开的起点。</param>
		/// <param name="path">指定要展开的成员路径，支持多级导航属性路径。</param>
		/// <param name="step">指定路径中每个属性的展开回调函数。</param>
		/// <returns>返回找到的结果。</returns>
		public static SpreadResult Spread(this TableIdentifier table, string path, Func<SpreadContext, ISource> step = null)
		{
			if(table == null)
				throw new ArgumentNullException(nameof(table));

			if(table.Entity == null)
				throw new DataException($"The '{table}' table cannot be expanded.");

			if(string.IsNullOrEmpty(path))
				return SpreadResult.Failure(table);

			Queue<IEntityMetadata> ancestors = null;
			IEntityPropertyMetadata property = null;
			ISource token = table;
			var parts = path.Split('.');
			var properties = table.Entity.Properties;

			for(int i = 0; i < parts.Length; i++)
			{
				if(properties == null)
					return SpreadResult.Failure(token);

				//如果当前属性集合中不包含指定的属性，则尝试从父实体中查找
				if(!properties.TryGet(parts[i], out property))
				{
					//尝试从父实体中查找指定的属性
					property = FindBaseProperty(ref properties, parts[i], ref ancestors);

					//如果父实体中也不含指定的属性则返回失败
					if(property == null)
						return SpreadResult.Failure(token);
				}

				//如果回调函数不为空，则调用匹配回调函数
				//注意：将回调函数返回的结果作为下一次的用户数据保存起来
				if(step != null)
					token = step(new SpreadContext(string.Join(".", parts, 0, i), token, property, ancestors));

				//清空继承实体链
				if(ancestors != null)
					ancestors.Clear();

				if(property.IsSimplex)
					break;
				else
					properties = GetAssociatedProperties((IEntityComplexPropertyMetadata)property, ref ancestors);
			}

			//返回查找到的结果
			return new SpreadResult(token, property);
		}
		#endregion

		#region 私有方法
		private static IEntityPropertyMetadataCollection GetAssociatedProperties(IEntityComplexPropertyMetadata property, ref Queue<IEntityMetadata> ancestors)
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
					found = FindBaseProperty(ref properties, part, ref ancestors);

					if(found == null)
						throw new DataException($"The '{part}' property of '{properties.Entity.Name}' entity does not existed.");
				}

				if(found.IsSimplex)
					return null;

				properties = GetAssociatedProperties((IEntityComplexPropertyMetadata)found, ref ancestors);
			}

			return properties;
		}

		private static IEntityPropertyMetadata FindBaseProperty(ref IEntityPropertyMetadataCollection properties, string name, ref Queue<IEntityMetadata> ancestors)
		{
			if(properties == null)
				return null;

			var baseEntity = properties.Entity.GetBaseEntity();

			if(baseEntity != null)
			{
				ancestors = new Queue<IEntityMetadata>();
				ancestors.Enqueue(baseEntity);
			}

			while(baseEntity != null)
			{
				if(baseEntity.Properties.TryGet(name, out var property))
					return property;

				baseEntity = baseEntity.GetBaseEntity();

				if(baseEntity != null)
					ancestors.Enqueue(baseEntity);
			}

			return null;
		}
		#endregion

		#region 嵌套子类
		/// <summary>
		/// 表示路径展开操作结果的结构。
		/// </summary>
		public struct SpreadResult
		{
			#region 公共字段
			public readonly ISource Source;
			public readonly IEntityPropertyMetadata Property;
			#endregion

			#region 构造函数
			public SpreadResult(ISource source, IEntityPropertyMetadata property)
			{
				this.Source = source;
				this.Property = property;
			}
			#endregion

			#region 公共属性
			public bool IsFailed
			{
				get
				{
					return this.Property == null;
				}
			}
			#endregion

			#region 静态方法
			internal static SpreadResult Failure(ISource token)
			{
				return new SpreadResult(token, null);
			}
			#endregion
		}

		/// <summary>
		/// 表示路径展开操作上下文的结构。
		/// </summary>
		public struct SpreadContext
		{
			#region 公共字段
			/// <summary>
			/// 获取当前匹配属性的成员路径（注意：不含当前属性名，即不是全路径）。
			/// </summary>
			public readonly string Path;

			/// <summary>
			/// 获取当前匹配阶段的源。
			/// </summary>
			public readonly ISource Source;

			/// <summary>
			/// 获取当前匹配到的属性。
			/// </summary>
			public readonly IEntityPropertyMetadata Property;

			/// <summary>
			/// 获取当前匹配属性的祖先（不含当前匹配属性所在的实体）实体集，如果为空集合，则表明当前匹配到的属性位于查找的实体。
			/// </summary>
			public readonly IEnumerable<IEntityMetadata> Ancestors;
			#endregion

			#region 构造函数
			public SpreadContext(string path, ISource source, IEntityPropertyMetadata property, IEnumerable<IEntityMetadata> ancestors)
			{
				this.Path = path;
				this.Source = source;
				this.Property = property;
				this.Ancestors = ancestors ?? System.Linq.Enumerable.Empty<IEntityMetadata>();
			}
			#endregion

			#region 公共属性
			public string FullPath
			{
				get
				{
					if(string.IsNullOrEmpty(this.Path))
						return this.Property.Name;
					else
						return this.Path + "." + this.Property.Name;
				}
			}
			#endregion
		}
		#endregion
	}
}
