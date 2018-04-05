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
using System.Collections.Generic;

using Zongsoft.Reflection;
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
		private static IDataBuilderFactory _builders;
		private static IDataProviderSelector _providers;
		private static IDataPopulatorProvider _populators;
		#endregion

		#region 静态构造
		static DataEnvironment()
		{
			_populators = DataPopulatorProvider.Default;
		}
		#endregion

		#region 公共属性
		public static IDataBuilderFactory Builders
		{
			get
			{
				return _builders;
			}
			set
			{
				_builders = value ?? throw new ArgumentNullException();
			}
		}

		public static IDataProviderSelector Providers
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

		#region 嵌套子类
		public static class Metadata
		{
			#region 成员字段
			private static readonly Collections.INamedCollection<IEntity> _entities = new Collections.NamedCollectionEx<IEntity>(item => item.Name);
			private static readonly Collections.INamedCollection<ICommand> _commands = new Collections.NamedCollectionEx<ICommand>(item => item.Name);
			#endregion

			#region 公共属性
			public static Collections.INamedCollection<IEntity> Entities
			{
				get
				{
					return _entities;
				}
			}

			public static Collections.INamedCollection<ICommand> Commands
			{
				get
				{
					return _commands;
				}
			}
			#endregion
		}
		#endregion
	}
}
