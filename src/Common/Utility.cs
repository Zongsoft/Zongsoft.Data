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
	internal static class Utility
	{
		public static DbType GetDbType(object value)
		{
			if(value == null)
				return DbType.Object;

			var type = value.GetType();

			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = Nullable.GetUnderlyingType(type);

			if(type.IsEnum)
				type = Enum.GetUnderlyingType(type);

			switch(Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return DbType.Boolean;
				case TypeCode.Byte:
					return DbType.Byte;
				case TypeCode.SByte:
					return DbType.SByte;
				case TypeCode.Char:
					return DbType.StringFixedLength;
				case TypeCode.DateTime:
					return ((DateTime)value).Kind == DateTimeKind.Utc ? DbType.DateTimeOffset : DbType.DateTime;
				case TypeCode.Decimal:
					return DbType.Decimal;
				case TypeCode.Double:
					return DbType.Double;
				case TypeCode.Int16:
					return DbType.Int16;
				case TypeCode.Int32:
					return DbType.Int32;
				case TypeCode.Int64:
					return DbType.Int64;
				case TypeCode.Single:
					return DbType.Single;
				case TypeCode.String:
					return DbType.String;
				case TypeCode.UInt16:
					return DbType.UInt16;
				case TypeCode.UInt32:
					return DbType.UInt32;
				case TypeCode.UInt64:
					return DbType.UInt64;
			}

			if(type == typeof(DateTimeOffset))
				return DbType.DateTimeOffset;
			else if(type == typeof(Guid))
				return DbType.Guid;
			else if(type == typeof(byte[]) || typeof(System.IO.Stream).IsAssignableFrom(type))
				return DbType.Binary;

			return DbType.Object;
		}
	}
}
