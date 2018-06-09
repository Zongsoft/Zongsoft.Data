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

using Zongsoft.Collections;
using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	/// <summary>
	/// 提供数据访问操作的环境信息。
	/// </summary>
	public static class DataEnvironment
	{
		#region 成员字段
		private static IDataDriverFactory _drivers;
		private static IDataSourceProvider _sources;
		private static IDataPopulatorProvider _populators;
		private static IDataProviderFactory _providers;
		private static INamedCollection<IMetadataProviderManager> _metadatas;
		#endregion

		#region 静态构造
		static DataEnvironment()
		{
			_populators = DataPopulatorProvider.Default;
			_metadatas = new NamedCollection<IMetadataProviderManager>(p => p.Name, StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public static INamedCollection<IMetadataProviderManager> Metadatas
		{
			get
			{
				return _metadatas;
			}
		}

		public static IDataDriverFactory Drivers
		{
			get
			{
				return _drivers;
			}
			set
			{
				_drivers = value ?? throw new ArgumentNullException();
			}
		}

		public static IDataSourceProvider Sources
		{
			get
			{
				return _sources;
			}
			set
			{
				_sources = value ?? throw new ArgumentNullException();
			}
		}

		public static IDataProviderFactory Providers
		{
			get
			{
				return _providers;
			}
			set
			{
				_providers = value ?? throw new ArgumentNullException();
			}
		}

		public static IDataPopulatorProvider Populators
		{
			get
			{
				return _populators;
			}
			set
			{
				_populators = value ?? throw new ArgumentNullException();
			}
		}
		#endregion
	}
}
