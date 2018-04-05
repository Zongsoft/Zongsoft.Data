/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

		#region 成员字段
		private DictionaryCreator _creator;
		#endregion

		#region 构造函数
		private DictionaryPopulator()
		{
			_creator = DictionaryCreator.Instance;
		}

		private DictionaryPopulator(DictionaryCreator dictionaryCreator)
		{
			_creator = dictionaryCreator ?? DictionaryCreator.Instance;
		}
		#endregion

		#region 公共属性
		IDataEntityCreator IDataPopulator.EntityCreator
		{
			get
			{
				return _creator;
			}
		}
		#endregion

		#region 公共方法
		public System.Collections.IEnumerable Populate(IDataReader reader, DataSelectionContext context)
		{
			var metadata = DataEnvironment.Providers.GetProvider(context).Metadata.Entities.Get(context.Name);
			var keys = new string[reader.FieldCount];

			for(int i = 0; i < reader.FieldCount; i++)
			{
				//获取字段名对应的属性名
				keys[i] = metadata.Properties.GetProperty(reader.GetName(i)).Name;
			}

			while(reader.Read())
			{
				//通过字典创建器来构建一个对应的实体字典
				var dictionary = _creator.Create(context.EntityType, reader);

				for(var i = 0; i < reader.FieldCount; i++)
				{
					dictionary[keys[i]] = reader.GetValue(i);
				}

				yield return dictionary;
			};
		}
		#endregion
	}
}
