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
using System.Reflection;

namespace Zongsoft.Data.Common
{
	public struct EntityMember
	{
		#region 私有变量
		private readonly Action<object, object> _setter;
		private readonly Action<object, IDataRecord, int> _populate;
		#endregion

		#region 公共字段
		public readonly string Name;
		public readonly Type Type;
		#endregion

		#region 构造函数
		public EntityMember(FieldInfo field, Action<object, IDataRecord, int> populate)
		{
			this.Name = field.Name;
			this.Type = field.FieldType;

			_setter = (entity, value) => field.SetValue(entity, value);
			_populate = populate ?? throw new ArgumentNullException(nameof(populate));
		}

		public EntityMember(PropertyInfo property, Action<object, IDataRecord, int> populate)
		{
			this.Name = property.Name;
			this.Type = property.PropertyType;

			_setter = (entity, value) => property.SetValue(entity, value);
			_populate = populate ?? throw new ArgumentNullException(nameof(populate));
		}
		#endregion

		#region 公共方法
		public void Populate(object entity, IDataRecord record, int ordinal)
		{
			_populate.Invoke(entity, record, ordinal);
		}

		public void SetValue(object entity, object value)
		{
			_setter.Invoke(entity, value);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return this.Name + " : " + this.Type.Name;
		}
		#endregion
	}
}
