/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public class DataAccessEnvironment
	{
		#region 单例字段
		public static readonly DataAccessEnvironment Instance = new DataAccessEnvironment();
		#endregion

		#region 成员字段
		private MetadataManager _metadataManager;
		private IList<IDataMapperProvider> _mappings;
		private IDataPopulatorProvider _populatorProvider;
		#endregion

		#region 私有构造
		private DataAccessEnvironment()
		{
			_metadataManager = MetadataManager.Default;
			_mappings = new List<IDataMapperProvider>();
			_populatorProvider = new DataPopulatorProvider();
		}
		#endregion

		#region 公共属性
		public MetadataManager MetadataManager
		{
			get
			{
				return _metadataManager;
			}
		}

		public ICollection<IDataMapperProvider> Mappings
		{
			get
			{
				return _mappings;
			}
		}

		public IDataPopulatorProvider PopulatorProvider
		{
			get
			{
				return _populatorProvider;
			}
		}
		#endregion
	}
}
