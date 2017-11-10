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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Data.Metadata.Schema
{
	public class MetadataManager
	{
		#region 单例字段
		public static readonly MetadataManager Default = new MetadataManager();
		#endregion

		#region 成员字段
		private ICollection<MetadataFile> _files;
		private IMetadataResolver _resolver;

		private IDictionary<string, MetadataElementBase> _concepts;
		private IDictionary<string, MetadataElementBase> _storages;
		#endregion

		#region 私有构造
		private MetadataManager()
		{
			_files = new List<MetadataFile>();
			_resolver = MetadataResolver.Default;

			_concepts = new Dictionary<string, MetadataElementBase>(StringComparer.OrdinalIgnoreCase);
			_storages = new Dictionary<string, MetadataElementBase>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public IReadOnlyCollection<MetadataFile> Files
		{
			get
			{
				return (IReadOnlyCollection<MetadataFile>)_files;
			}
		}

		public IMetadataResolver Resolver
		{
			get
			{
				return _resolver;
			}
			set
			{
				_resolver = value ?? throw new ArgumentNullException();
			}
		}
		#endregion

		#region 公共方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Load(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			if(System.IO.File.Exists(path))
			{
				if(_files.Any(p => string.Equals(p.Url, path, StringComparison.OrdinalIgnoreCase)))
					return;

				var metadata = _resolver.Resolve(path);

				if(metadata != null)
				{
					this.LoadElements(metadata);
					_files.Add(metadata);

				}
			}
			else if(System.IO.Directory.Exists(path))
			{
				var files = System.IO.Directory.EnumerateFiles(path, "*.mapping", System.IO.SearchOption.AllDirectories);

				foreach(var file in files)
				{
					if(_files.Any(p => string.Equals(p.Url, file, StringComparison.OrdinalIgnoreCase)))
						continue;

					var metadata = _resolver.Resolve(file);

					if(metadata != null)
					{
						this.LoadElements(metadata);
						_files.Add(metadata);
					}
				}
			}
		}

		public T GetConceptElement<T>(string qualifiedName) where T : MetadataElementBase
		{
			if(_concepts.TryGetValue(qualifiedName, out var element))
				return element as T;

			return null;
		}

		public T GetStorageElement<T>(string qualifiedName) where T : MetadataElementBase
		{
			if(_storages.TryGetValue(qualifiedName, out var element))
				return element as T;

			return null;
		}
		#endregion

		#region 私有方法
		private void LoadElements(MetadataFile file)
		{
			foreach(var concept in file.Concepts)
			{
				foreach(var entity in concept.Entities)
				{
					_concepts.Add(entity.QualifiedName, entity);
				}

				foreach(var command in concept.Commands)
				{
					_concepts.Add(command.FullName, command);
				}

				foreach(var association in concept.Associations)
				{
					_concepts.Add(association.FullName, association);
				}
			}

			foreach(var storage in file.Storages)
			{
				foreach(var entity in storage.Entities)
				{
					_storages.Add(entity.QualifiedName, entity);
				}

				foreach(var command in storage.Commands)
				{
					_storages.Add(command.FullName, command);
				}
			}
		}
		#endregion
	}
}
