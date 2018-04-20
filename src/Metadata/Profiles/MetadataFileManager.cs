using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Zongsoft.Collections;

namespace Zongsoft.Data.Metadata.Profiles
{
	public class MetadataFileManager : IMetadataProviderManager, IDisposable
	{
		#region 成员字段
		private readonly string _path;
		private readonly object _syncRoot;
		private ObservableCollection<IMetadataProvider> _providers;
		private INamedCollection<IEntity> _entities;
		private INamedCollection<ICommand> _commands;
		#endregion

		#region 构造函数
		public MetadataFileManager(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			_path = path;
			_syncRoot = new object();

			_entities = new NamedCollection<IEntity>(p => p.Name);
			_commands = new NamedCollection<ICommand>(p => p.Name);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取元数据提供程序集（即数据映射文件集合）。
		/// </summary>
		public ICollection<IMetadataProvider> Providers
		{
			get
			{
				if(_providers == null)
					this.Initialize();

				return _providers;
			}
		}

		/// <summary>
		/// 获取全局的实体元数据集。
		/// </summary>
		public INamedCollection<IEntity> Entities
		{
			get
			{
				if(_providers == null)
					this.Initialize();

				return _entities;
			}
		}

		/// <summary>
		/// 获取全局的命令元数据集。
		/// </summary>
		public INamedCollection<ICommand> Commands
		{
			get
			{
				if(_providers == null)
					this.Initialize();

				return _commands;
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnAddProvider(IMetadataProvider provider)
		{
			if(provider == null)
				return;

			//将元数据提供程序中的实体定义依次添加到全局实体集中
			foreach(var entity in provider.Entities)
			{
				_entities.Add(entity);
			}

			//将元数据提供程序中的命令定义依次添加到全局命令集中
			foreach(var command in provider.Commands)
			{
				_commands.Add(command);
			}
		}

		protected virtual void OnRemoveProvider(IMetadataProvider provider)
		{
			if(provider == null)
				return;

			//将元数据提供程序中的实体定义依次从全局实体集中删除
			foreach(var entity in provider.Entities)
			{
				_entities.Remove(entity);
			}

			//将元数据提供程序中的命令定义依次从全局命令集中删除
			foreach(var command in provider.Commands)
			{
				_commands.Remove(command);
			}
		}
		#endregion

		#region 私有方法
		private bool Initialize()
		{
			if(_providers == null)
			{
				lock(_syncRoot)
				{
					if(_providers == null)
					{
						//如果指定的目录不存在则返回初始化失败
						if(!Directory.Exists(_path))
							return false;

						//创建映射文件列表
						_providers = new ObservableCollection<IMetadataProvider>();

						//查找指定目录下的所有映射文件
						var files = Directory.GetFiles(_path, "*.mapping", SearchOption.AllDirectories);

						foreach(var file in files)
						{
							//加载指定的映射文件
							var provider = MetadataFile.Load(file);

							//通知新增了一个映射文件
							this.OnAddProvider(provider);

							//将加载成功的映射文件加入到提供程序集中
							_providers.Add(provider);
						}

						//挂载提供程序集的改变事件处理
						_providers.CollectionChanged += Providers_CollectionChanged;

						//返回真（表示初始化完成）
						return true;
					}
				}
			}

			//返回假（表示不需要再初始化，即已经初始化过了）
			return false;
		}

		private void Providers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach(var item in e.NewItems)
					{
						this.OnAddProvider(item as IMetadataProvider);
					}

					break;
				case NotifyCollectionChangedAction.Remove:
					foreach(var item in e.OldItems)
					{
						this.OnRemoveProvider(item as IMetadataProvider);
					}

					break;
				case NotifyCollectionChangedAction.Replace:
					foreach(var item in e.OldItems)
					{
						this.OnRemoveProvider(item as IMetadataProvider);
					}

					foreach(var item in e.NewItems)
					{
						this.OnAddProvider(item as IMetadataProvider);
					}

					break;
				case NotifyCollectionChangedAction.Reset:
					_entities.Clear();
					_commands.Clear();
					break;
			}
		}
		#endregion

		#region 处置方法
		void IDisposable.Dispose()
		{
			var providers = System.Threading.Interlocked.Exchange(ref _providers, null);

			if(providers != null)
			{
				providers.CollectionChanged -= Providers_CollectionChanged;
				providers.Clear();

				_entities.Clear();
				_commands.Clear();
			}
		}
		#endregion
	}
}
