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

		#region 公共属性
		IDataEntityCreator IDataPopulator.EntityCreator
		{
			get
			{
				return null;
			}
		}
		#endregion

		#region 公共方法
		public System.Collections.IEnumerable Populate(IDataReader reader, DataSelectContext context)
		{
			if(reader.FieldCount != 1)
				yield break;

			switch(Type.GetTypeCode(context.EntityType))
			{
				case TypeCode.Boolean:
					yield return reader.GetBoolean(0);
					break;
				case TypeCode.Byte:
					yield return reader.GetByte(0);
					break;
				case TypeCode.SByte:
					yield return (sbyte)reader.GetByte(0);
					break;
				case TypeCode.Char:
					yield return reader.GetChar(0);
					break;
				case TypeCode.DateTime:
					yield return reader.GetDateTime(0);
					break;
				case TypeCode.Decimal:
					yield return reader.GetDecimal(0);
					break;
				case TypeCode.Double:
					yield return reader.GetDouble(0);
					break;
				case TypeCode.Int16:
					yield return reader.GetInt16(0);
					break;
				case TypeCode.Int32:
					yield return reader.GetInt32(0);
					break;
				case TypeCode.Int64:
					yield return reader.GetInt64(0);
					break;
				case TypeCode.Single:
					yield return reader.GetFloat(0);
					break;
				case TypeCode.String:
					yield return reader.GetString(0);
					break;
				case TypeCode.UInt16:
					yield return (ushort)reader.GetInt16(0);
					break;
				case TypeCode.UInt32:
					yield return (uint)reader.GetInt32(0);
					break;
				case TypeCode.UInt64:
					yield return (ulong)reader.GetInt64(0);
					break;
				default:
					if(context.EntityType == typeof(Guid))
						yield return reader.GetGuid(0);
					else if(context.EntityType == typeof(DateTimeOffset))
						yield return (DateTimeOffset)reader.GetDateTime(0);
					else if(context.EntityType == typeof(byte[]))
					{
						var buffer = new byte[1024 * 8];
						var length = reader.GetBytes(0, 0, buffer, 0, buffer.Length);

						if(length > 0)
							yield return new ArraySegment<byte>(buffer, 0, (int)length);
						else
							yield return new byte[0];
					}
					else
						yield return reader.GetValue(0);

					break;
			}
		}
		#endregion
	}
}
