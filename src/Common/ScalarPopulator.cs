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

namespace Zongsoft.Data.Common
{
	public static class ScalarPopulator
	{
		#region 单例字段
		public static readonly IDataPopulator Char = new ValuePopulator<char>();
		public static readonly IDataPopulator Guid = new ValuePopulator<Guid>();
		public static readonly IDataPopulator String = new ValuePopulator<string>();
		public static readonly IDataPopulator Boolean = new ValuePopulator<bool>();
		public static readonly IDataPopulator DateTime = new ValuePopulator<DateTime>();
		public static readonly IDataPopulator DateTimeOffset = new ValuePopulator<DateTimeOffset>();

		public static readonly IDataPopulator Byte = new ValuePopulator<byte>();
		public static readonly IDataPopulator SByte = new ValuePopulator<sbyte>();
		public static readonly IDataPopulator Int16 = new ValuePopulator<Int16>();
		public static readonly IDataPopulator Int32 = new ValuePopulator<Int32>();
		public static readonly IDataPopulator Int64 = new ValuePopulator<Int64>();
		public static readonly IDataPopulator UInt16 = new ValuePopulator<UInt16>();
		public static readonly IDataPopulator UInt32 = new ValuePopulator<UInt32>();
		public static readonly IDataPopulator UInt64 = new ValuePopulator<UInt64>();

		public static readonly IDataPopulator Single = new ValuePopulator<float>();
		public static readonly IDataPopulator Double = new ValuePopulator<double>();
		public static readonly IDataPopulator Decimal = new ValuePopulator<decimal>();
		#endregion

		#region 嵌套子类
		private class ValuePopulator<T> : IDataPopulator
		{
			public object Populate(IDataRecord record)
			{
				return record.GetValue<T>(0);
			}
		}
		#endregion
	}
}
