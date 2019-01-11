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
using System.Data;
using System.Collections.Generic;

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

		public IDataPopulator GetPopulator(Type type, IDataReader reader)
		{
			var members = EntityMemberProvider.Default.GetMembers(type);
			var tokens = new List<EntityPopulator.PopulateToken>(reader.FieldCount);

			for(int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
			{
				//获取当前列对应的属性名（注意：由查询引擎确保返回的列名就是属性名）
				var name = reader.GetName(ordinal);

				//如果属性名的首字符不是字母或下划线则忽略当前列
				if(IsLetterOrUnderscore(name[0]))
					continue;

				//构建当前属性的层级结构
				this.FillTokens(members, tokens, name, ordinal);
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
		private void FillTokens(Reflection.MemberTokenCollection members, ICollection<EntityPopulator.PopulateToken> tokens, string name, int ordinal)
		{
			Reflection.MemberToken member;
			EntityPopulator.PopulateToken? token = null;

			int index, last = 0;

			while((index = name.IndexOf('.', last + 1)) > 0)
			{
				token = FillToken(members, tokens, name.Substring(last, index - last));
				last = index;

				if(token == null)
					return;

				members = EntityMemberProvider.Default.GetMembers(token.Value.Member.Type);
				tokens = token.Value.Tokens;
			}

			if(members.TryGet(name.Substring(last), out member) && token.HasValue)
				token.Value.Tokens.Add(new EntityPopulator.PopulateToken((EntityMember)member, ordinal));
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private EntityPopulator.PopulateToken? FillToken(Reflection.MemberTokenCollection members, ICollection<EntityPopulator.PopulateToken> tokens, string name)
		{
			EntityPopulator.PopulateToken? found = null;

			foreach(var token in tokens)
			{
				if(string.Equals(token.Member.Name, name))
				{
					found = token;
					break;
				}
			}

			if(found == null && members.TryGet(name, out var member))
			{
				found = new EntityPopulator.PopulateToken((EntityMember)member);
				tokens.Add(found.Value);
			}

			return found;
		}
		#endregion
	}
}
