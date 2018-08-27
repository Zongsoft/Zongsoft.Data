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
using System.Reflection;

using Zongsoft.Collections;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public class Scope : ScopeBase
	{
		#region 构造函数
		private Scope(EntityPropertyToken token)
		{
			this.Token = token;
		}
		#endregion

		#region 公共属性
		public override string Name
		{
			get
			{
				return this.Property.Name;
			}
		}

		public Scope Parent
		{
			get
			{
				return (Scope)base.GetParent();
			}
		}

		public EntityPropertyToken Token
		{
			get;
		}
		#endregion

		#region 解析方法
		public static IReadOnlyNamedCollection<Scope> Parse(string text, IEntityMetadata entity, Type elementType)
		{
			return ScopeBase.Parse(text, token =>
			{
				var owner = entity;

				if(token.Parent != null)
				{
					var parent = (Scope)token.Parent;

					if(parent.Token.Property.IsSimplex)
						throw new DataException("");

					owner = ((IEntityComplexPropertyMetadata)parent.Token.Property).GetForeignEntity();
				}

				if(token.Name == "*")
					return owner.GetTokens(elementType)
								.Where(p => p.Property.IsSimplex)
								.Select(p => new Scope(p));

				return new Scope[] { new Scope(owner.GetTokens(elementType).Get(token.Name)) };
			});
		}
		#endregion
	}
}
