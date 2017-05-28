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
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Zongsoft.Data.Metadata
{
	public class MetadataManager
	{
		#region 单例字段
		public static readonly MetadataManager Default = new MetadataManager();
		#endregion

		#region 成员字段
		private ObservableCollection<MetadataFile> _files;
		private List<MetadataMapping> _mappings;
		private ConcurrentDictionary<string, IList<MetadataFile>> _innerDictionary;
		#endregion

		#region 私有构造
		private MetadataManager()
		{
			_files = new ObservableCollection<MetadataFile>();
			_innerDictionary = new ConcurrentDictionary<string, IList<MetadataFile>>(StringComparer.OrdinalIgnoreCase);

			_files.CollectionChanged += Files_CollectionChanged;
		}
		#endregion

		#region 公共属性
		public IList<MetadataFile> Files
		{
			get
			{
				return _files;
			}
		}
		#endregion

		#region 公共方法
		public MetadataContainer GetConceptContainer(string name, string @namespace)
		{
			return this.FindElement(@namespace, file => file.Concepts[name]);
		}

		public MetadataContainer GetStorageContainer(string name, string @namespace)
		{
			return this.FindElement(@namespace, file => file.Storages[name]);
		}

		public T GetConceptElement<T>(string qualifiedName) where T : MetadataElementBase
		{
			var name = DataName.Parse(qualifiedName);
			var container = this.GetConceptContainer(name.ContainerName, name.Namespace);

			if(container == null)
				return null;

			if(typeof(MetadataEntity).IsAssignableFrom(typeof(T)))
				return container.Entities[name.ElementName] as T;

			if(typeof(MetadataCommand).IsAssignableFrom(typeof(T)))
				return container.Commands[name.ElementName] as T;

			if(typeof(MetadataAssociation).IsAssignableFrom(typeof(T)))
				return container.Associations[name.ElementName] as T;

			throw new InvalidOperationException();
		}

		public T GetStorageElement<T>(string qualifiedName) where T : MetadataElementBase
		{
			var name = DataName.Parse(qualifiedName);
			var container = this.GetStorageContainer(name.ContainerName, name.Namespace);

			if(container == null)
				return null;

			if(typeof(MetadataEntity).IsAssignableFrom(typeof(T)))
				return container.Entities[name.ElementName] as T;

			if(typeof(MetadataCommand).IsAssignableFrom(typeof(T)))
				return container.Commands[name.ElementName] as T;

			if(typeof(MetadataAssociation).IsAssignableFrom(typeof(T)))
				return container.Associations[name.ElementName] as T;

			throw new InvalidOperationException();
		}

		public MetadataMapping GetMapping(string qualifiedName)
		{
			var index = qualifiedName.LastIndexOf('@');

			if(index < 1 || index >= qualifiedName.Length)
				throw new ArgumentException("Unspecified namespace part in the qualifiedName parameter.");

			return this.GetMapping(qualifiedName.Substring(0, index), qualifiedName.Substring(index + 1));
		}

		public MetadataMapping GetMapping(string conceptionName, string @namespace)
		{
			return this.FindElement(@namespace, file => file.Mappings[conceptionName]);
		}

		public IEnumerable<MetadataFile> GetFiles(string @namespace)
		{
			IList<MetadataFile> result;

			if(_innerDictionary.TryGetValue(@namespace, out result))
				return result;

			return System.Linq.Enumerable.Empty<MetadataFile>();
		}
		#endregion

		#region 私有方法
		private T FindElement<T>(string @namespace, Func<MetadataFile, T> finder) where T : class
		{
			IList<MetadataFile> files;

			if(_innerDictionary.TryGetValue(@namespace, out files))
			{
				foreach(var file in files)
				{
					var result = finder(file);

					if(result != null)
						return result;
				}
			}

			return null;
		}
		#endregion

		#region 事件处理
		private void Files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Reset:
					_innerDictionary.Clear();
					break;
				case NotifyCollectionChangedAction.Add:
					foreach(MetadataFile item in e.NewItems)
					{
						if(!_innerDictionary.TryAdd(item.Namespace, new List<MetadataFile>(new MetadataFile[] { item })))
						{
							var files = _innerDictionary[item.Namespace];
							files.Add(item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach(MetadataFile item in e.OldItems)
					{
						IList<MetadataFile> files;

						if(_innerDictionary.TryGetValue(item.Namespace, out files))
							files.Remove(item);
					}
					break;
			}

			_mappings = null;
		}
		#endregion
	}
}
