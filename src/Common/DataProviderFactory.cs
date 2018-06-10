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
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Data.Common
{
	public class DataProviderFactory : IDataProviderFactory
	{
		#region 成员字段
		private IDictionary<string, IDataProvider> _providers;
		#endregion

		#region 构造函数
		protected DataProviderFactory()
		{
			_providers = new Dictionary<string, IDataProvider>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _providers.Count;
			}
		}
		#endregion

		#region 公共方法
		public IDataProvider GetProvider(string name)
		{
			if(_providers.TryGetValue(name, out var provider))
				return provider;

			lock(_providers)
			{
				if(_providers.TryGetValue(name, out provider))
					return provider;

				_providers.Add(name, provider = this.CreateProvider(name));
			}

			return provider;
		}
		#endregion

		#region 虚拟方法
		protected virtual IDataProvider CreateProvider(string name)
		{
			var loaders = DataEnvironment.Loaders;
			var provider = new DataProvider(name);

			foreach(var loader in loaders)
			{
				var metadatas = loader.Load(name);

				if(metadatas != null)
				{
					foreach(var metadata in metadatas)
						provider.Metadata.Providers.Add(metadata);
				}
			}

			var connectionStrings = OptionManager.Default.GetOptionValue("/Data/ConnectionStrings") as ConnectionStringElementCollection;

			if(connectionStrings != null)
			{
				foreach(ConnectionStringElement connectionString in connectionStrings)
				{
					if(string.Equals(connectionString.Name, name, StringComparison.OrdinalIgnoreCase) ||
					   connectionString.Name.StartsWith(name + ":", StringComparison.OrdinalIgnoreCase))
						provider.Sources.Add(new DataSource(connectionString.Name, connectionString.Value, connectionString.Provider));
				}
			}

			return provider;
		}
		#endregion

		#region 枚举遍历
		public IEnumerator<IDataProvider> GetEnumerator()
		{
			foreach(var value in _providers.Values)
			{
				yield return value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach(var value in _providers.Values)
			{
				yield return value;
			}
		}
		#endregion
	}
}
