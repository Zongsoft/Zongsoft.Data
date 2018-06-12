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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Collections;

namespace Zongsoft.Data.Metadata.Profiles
{
	public class MetadataFileManager : IMetadataProviderManager, IDisposable
	{
		#region 成员字段
		private readonly string _name;
		private IMetadataLoader _loader;
		private ICollection<IMetadataProvider> _providers;
		private IReadOnlyNamedCollection<IEntity> _entities;
		private IReadOnlyNamedCollection<ICommand> _commands;
		#endregion

		#region 构造函数
		public MetadataFileManager(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name;
			_loader = new MetadataFileLoader();
			_providers = new List<IMetadataProvider>();
			_entities = new EntityCollection(_providers);
			_commands = new CommandCollection(_providers);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取元数据管理器所属的应用名。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取当前应用的元数据文件加载器。
		/// </summary>
		public IMetadataLoader Loader
		{
			get
			{
				return _loader;
			}
		}

		/// <summary>
		/// 获取元数据提供程序集（即数据映射文件集合）。
		/// </summary>
		public ICollection<IMetadataProvider> Providers
		{
			get
			{
				return _providers;
			}
		}

		/// <summary>
		/// 获取全局的实体元数据集。
		/// </summary>
		public IReadOnlyNamedCollection<IEntity> Entities
		{
			get
			{
				return _entities;
			}
		}

		/// <summary>
		/// 获取全局的命令元数据集。
		/// </summary>
		public IReadOnlyNamedCollection<ICommand> Commands
		{
			get
			{
				return _commands;
			}
		}
		#endregion

		#region 处置方法
		void IDisposable.Dispose()
		{
			var providers = System.Threading.Interlocked.Exchange(ref _providers, null);

			if(providers != null)
			{
				providers.Clear();
			}
		}
		#endregion

		private class EntityCollection : IReadOnlyNamedCollection<IEntity>
		{
			#region 成员字段
			private readonly ICollection<IMetadataProvider> _providers;
			#endregion

			#region 构造函数
			public EntityCollection(ICollection<IMetadataProvider> providers)
			{
				_providers = providers ?? throw new ArgumentNullException(nameof(providers));
			}
			#endregion

			#region 公共成员
			public int Count
			{
				get
				{
					return _providers.Sum(p => p.Entities.Count);
				}
			}

			public bool Contains(string name)
			{
				return _providers.Any(p => p.Entities.Contains(name));
			}

			public IEntity Get(string name)
			{
				IEntity entity;

				foreach(var provider in _providers)
				{
					if(provider.Entities.TryGet(name, out entity))
						return entity;
				}

				throw new KeyNotFoundException($"The specified '{name}' entity does not exist.");
			}

			public bool TryGet(string name, out IEntity value)
			{
				value = null;

				foreach(var provider in _providers)
				{
					if(provider.Entities.TryGet(name, out value))
						return true;
				}

				return false;
			}

			public IEnumerator<IEntity> GetEnumerator()
			{
				foreach(var provider in _providers)
				{
					foreach(var entity in provider.Entities)
						yield return entity;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}

		private class CommandCollection : IReadOnlyNamedCollection<ICommand>
		{
			#region 成员字段
			private readonly ICollection<IMetadataProvider> _providers;
			#endregion

			#region 构造函数
			public CommandCollection(ICollection<IMetadataProvider> providers)
			{
				_providers = providers ?? throw new ArgumentNullException(nameof(providers));
			}
			#endregion

			#region 公共成员
			public int Count
			{
				get
				{
					return _providers.Sum(p => p.Commands.Count);
				}
			}

			public bool Contains(string name)
			{
				return _providers.Any(p => p.Commands.Contains(name));
			}

			public ICommand Get(string name)
			{
				ICommand command;

				foreach(var provider in _providers)
				{
					if(provider.Commands.TryGet(name, out command))
						return command;
				}

				throw new KeyNotFoundException($"The specified '{name}' command does not exist.");
			}

			public bool TryGet(string name, out ICommand value)
			{
				value = null;

				foreach(var provider in _providers)
				{
					if(provider.Commands.TryGet(name, out value))
						return true;
				}

				return false;
			}

			public IEnumerator<ICommand> GetEnumerator()
			{
				foreach(var provider in _providers)
				{
					foreach(var command in provider.Commands)
						yield return command;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}
	}
}
