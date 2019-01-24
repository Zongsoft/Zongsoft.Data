﻿/*
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
using System.Data;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common
{
	public class EntityPopulatorProvider : IDataPopulatorProvider
	{
		#region 单例模式
		public static readonly EntityPopulatorProvider Instance = new EntityPopulatorProvider();
		#endregion

		#region 构造函数
		private EntityPopulatorProvider()
		{
		}
		#endregion

		#region 公共方法
		public bool CanPopulate(Type type)
		{
			return !(type.IsPrimitive || type.IsArray || type.IsEnum ||
			         Zongsoft.Common.TypeExtension.IsScalarType(type) ||
			         Zongsoft.Common.TypeExtension.IsEnumerable(type));
		}

		public IDataPopulator GetPopulator(IEntityMetadata entity, Type type, IDataReader reader)
		{
			var members = EntityMemberProvider.Instance.GetMembers(type);
			var tokens = new List<EntityPopulator.PopulateToken>(reader.FieldCount);

			for(int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
			{
				//获取当前列对应的属性名（注意：由查询引擎确保返回的列名就是属性名）
				var name = reader.GetName(ordinal);

				//如果属性名的首字符不是字母或下划线则忽略当前列
				if(!IsLetterOrUnderscore(name[0]))
					continue;

				//构建当前属性的层级结构
				this.FillTokens(entity, members, tokens, name, ordinal);
			}

			return new EntityPopulator(type, tokens);
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool IsLetterOrUnderscore(char chr)
		{
			return (chr >= 'A' && chr <= 'Z') ||
			       (chr >= 'a' && chr <= 'z') || chr == '_';
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private void FillTokens(IEntityMetadata entity, Collections.INamedCollection<EntityMember> members, ICollection<EntityPopulator.PopulateToken> tokens, string name, int ordinal)
		{
			EntityMember member;
			EntityPopulator.PopulateToken? token = null;

			int index, last = 0;

			while((index = name.IndexOf('.', last + 1)) > 0)
			{
				token = FillToken(entity, members, tokens, name.Substring(GetLast(last), index - GetLast(last)));
				last = index;

				if(token == null)
					return;

				entity = token.Value.Entity;
				members = EntityMemberProvider.Instance.GetMembers(token.Value.Member.Type);
				tokens = token.Value.Tokens;
			}

			if(members.TryGet(name.Substring(GetLast(last)), out member))
			{
				if(token.HasValue && entity.Properties.Get(member.Name).IsPrimaryKey)
				{
					for(int i = 0; i < entity.Key.Length; i++)
					{
						if(string.Equals(entity.Key[i].Name, member.Name))
						{
							token.Value.Keys[i] = ordinal;
							break;
						}
					}
				}

				tokens.Add(new EntityPopulator.PopulateToken(entity, member, ordinal));
			}
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private EntityPopulator.PopulateToken? FillToken(IEntityMetadata entity, Collections.INamedCollection<EntityMember> members, ICollection<EntityPopulator.PopulateToken> tokens, string name)
		{
			foreach(var token in tokens)
			{
				if(string.Equals(token.Member.Name, name))
					return token;
			}

			if(members.TryGet(name, out var member))
			{
				if(entity.Properties[name].IsSimplex)
					throw new InvalidOperationException($"The '{name}' property of '{entity}' entity is not a complex(navigation) property.");

				var token = new EntityPopulator.PopulateToken(((IEntityComplexPropertyMetadata)entity.Properties[name]).Foreign, (EntityMember)member);
				tokens.Add(token);
				return token;
			}

			return null;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private int GetLast(int last)
		{
			return last > 0 ? last + 1 : last;
		}
		#endregion
	}
}
