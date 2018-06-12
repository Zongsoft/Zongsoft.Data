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
	public class ScalarPopulatorProvider : IDataPopulatorProvider
	{
		#region 单例模式
		public static readonly ScalarPopulatorProvider Instance = new ScalarPopulatorProvider();
		#endregion

		#region 构造函数
		private ScalarPopulatorProvider()
		{
		}
		#endregion

		#region 公共方法
		public bool CanPopulate(Type type)
		{
			return Zongsoft.Common.TypeExtension.IsScalarType(type);
		}

		public IDataPopulator GetPopulator(Type type, IDataReader reader)
		{
			//获取指定类型对应的装配器
			var populator = this.GetPopulator(type);

			if(populator == null)
			{
				//如果是可空类型，则获取可空类型的定义元类型
				if(type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					populator = this.GetPopulator(type.GetGenericArguments()[0]);
				}
			}

			return populator;
		}
		#endregion

		#region 私有方法
		private IDataPopulator GetPopulator(Type type)
		{
			switch(Type.GetTypeCode(type))
			{
				case TypeCode.Char:
					return ScalarPopulator.Char;
				case TypeCode.String:
					return ScalarPopulator.String;
				case TypeCode.Boolean:
					return ScalarPopulator.Boolean;
				case TypeCode.DateTime:
					return ScalarPopulator.DateTime;
				case TypeCode.Byte:
					return ScalarPopulator.Byte;
				case TypeCode.SByte:
					return ScalarPopulator.SByte;
				case TypeCode.Int16:
					return ScalarPopulator.Int16;
				case TypeCode.Int32:
					return ScalarPopulator.Int32;
				case TypeCode.Int64:
					return ScalarPopulator.Int64;
				case TypeCode.UInt16:
					return ScalarPopulator.UInt16;
				case TypeCode.UInt32:
					return ScalarPopulator.UInt32;
				case TypeCode.UInt64:
					return ScalarPopulator.UInt64;
				case TypeCode.Single:
					return ScalarPopulator.Single;
				case TypeCode.Double:
					return ScalarPopulator.Double;
				case TypeCode.Decimal:
					return ScalarPopulator.Decimal;
			}

			if(type == typeof(Guid))
				return ScalarPopulator.Guid;
			else if(type == typeof(DateTimeOffset))
				return ScalarPopulator.DateTimeOffset;

			return null;
		}
		#endregion
	}
}
