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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public struct EntityPropertyToken
	{
		public readonly IEntityPropertyMetadata Property;
		public readonly MemberInfo Member;

		public EntityPropertyToken(IEntityPropertyMetadata property, MemberInfo member)
		{
			this.Property = property;
			this.Member = member;
		}

		public object GetValue(object target)
		{
			if(this.Member != null)
				return Reflection.Reflector.GetValue(this.Member, target);

			if(target is IDictionary dict1)
				return dict1[this.Property.Name];

			if(target is IDictionary<string, object> dict2)
				return dict2[this.Property.Name];

			throw new InvalidOperationException("");
		}

		public void SetValue(object target, object value)
		{
			if(this.Member != null)
				Reflection.Reflector.SetValue(this.Member, target, value);
			else
			{
				if(target is IDictionary dict1)
					dict1[this.Property.Name] = value;
				else if(target is IDictionary<string, object> dict2)
					dict2[this.Property.Name] = value;
				else
					throw new InvalidOperationException("");
			}
		}
	}
}
