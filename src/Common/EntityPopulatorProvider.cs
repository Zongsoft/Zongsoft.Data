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
			var mapping = new EntityMember[reader.FieldCount];

			for(int i = 0; i < reader.FieldCount; i++)
			{
				//获取字段名对应的属性名（注意：由查询引擎确保返回的记录列名就是属性名）
				var name = reader.GetName(i);

				if(IsLetterOrUnderscore(name[0]) && members.TryGet(name, out var member))
					mapping[i] = (EntityMember)member;
				else
					mapping[i] = null;
			}

			return new EntityPopulator(type, mapping);
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool IsLetterOrUnderscore(char chr)
		{
			return (chr >= 'A' && chr <= 'Z') ||
			       (chr >= 'a' && chr <= 'z') || chr == '_';
		}
		#endregion
	}
}
