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
using System.Collections.Generic;

namespace Zongsoft.Data.Common
{
	public class DictionaryPopulator : IDataPopulator
	{
		#region 单例模式
		public static readonly DictionaryPopulator Instance = new DictionaryPopulator();
		#endregion

		#region 构造函数
		private DictionaryPopulator()
		{
		}
		#endregion

		#region 公共方法
		public object Populate(Type type, IDataRecord record)
		{
			var keys = new string[record.FieldCount];

			for(int i = 0; i < record.FieldCount; i++)
			{
				//获取字段名对应的属性名（注意：由查询引擎确保返回的记录列名就是属性名）
				keys[i] = record.GetName(i);
			}

			//创建一个对应的实体字典
			var dictionary = this.CreateDictionary(record.FieldCount);

			for(var i = 0; i < record.FieldCount; i++)
			{
				dictionary[keys[i]] = record.GetValue(i);
			}

			return dictionary;
		}
		#endregion

		#region 虚拟方法
		protected virtual IDictionary<string, object> CreateDictionary(int capacity)
		{
			return new Dictionary<string, object>(capacity, StringComparer.OrdinalIgnoreCase);
		}
		#endregion
	}
}
