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
	public class ScalarPopulator : IDataPopulator
	{
		#region 单例模式
		public static readonly ScalarPopulator Instance = new ScalarPopulator();
		#endregion

		#region 构造函数
		private ScalarPopulator()
		{
		}
		#endregion

		#region 公共方法
		public object Populate(Type type, IDataRecord record)
		{
			switch(Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return record.GetBoolean(0);
				case TypeCode.Byte:
					return record.GetByte(0);
				case TypeCode.SByte:
					return (sbyte)record.GetByte(0);
				case TypeCode.Char:
					return record.GetChar(0);
				case TypeCode.DateTime:
					return record.GetDateTime(0);
				case TypeCode.Decimal:
					return record.GetDecimal(0);
				case TypeCode.Double:
					return record.GetDouble(0);
				case TypeCode.Int16:
					return record.GetInt16(0);
				case TypeCode.Int32:
					return record.GetInt32(0);
				case TypeCode.Int64:
					return record.GetInt64(0);
				case TypeCode.Single:
					return record.GetFloat(0);
				case TypeCode.String:
					return record.GetString(0);
				case TypeCode.UInt16:
					return (ushort)record.GetInt16(0);
				case TypeCode.UInt32:
					return (uint)record.GetInt32(0);
				case TypeCode.UInt64:
					return (ulong)record.GetInt64(0);
				default:
					if(type == typeof(Guid))
						return record.GetGuid(0);
					else if(type == typeof(DateTimeOffset))
						return (DateTimeOffset)record.GetDateTime(0);
					else if(type == typeof(byte[]))
					{
						var buffer = new byte[1024];
						var length = record.GetBytes(0, 0, buffer, 0, buffer.Length);

						if(length > 0)
							return new ArraySegment<byte>(buffer, 0, (int)length);
						else
							return new byte[0];
					}

					return record.GetValue(0);
			}
		}
		#endregion
	}
}
