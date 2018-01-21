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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Zongsoft.Data.Metadata
{
	public class MetadataDirector : IMetadataProvider
	{
		#region 单例字段
		public static readonly MetadataDirector Instance = new MetadataDirector();
		#endregion

		#region 成员字段
		private ObservableCollection<IMetadataProvider> _providers;
		#endregion

		#region 构造函数
		public MetadataDirector()
		{
			_providers = new ObservableCollection<IMetadataProvider>();
			_providers.CollectionChanged += Providers_CollectionChanged;
		}
		#endregion

		#region 公共属性
		public ICollection<IMetadataProvider> Providers
		{
			get
			{
				return _providers;
			}
		}

		public ICollection<IEntity> Entities
		{
			get
			{
				return null;
			}
		}

		public ICollection<ICommand> Commands
		{
			get
			{
				return null;
			}
		}
		#endregion

		#region 公共方法
		public IEntity GetEntity(string name)
		{
			throw new NotImplementedException();
		}

		public ICommand GetCommand(string name)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 集合事件
		private void Providers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach(IMetadataProvider provider in e.NewItems)
			{
			}
		}
		#endregion
	}
}
