using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Zongsoft.Data.Metadata
{
	public class MetadataManager : IMetadataProvider
	{
		#region 单例字段
		public static readonly MetadataManager Instance = new MetadataManager();
		#endregion

		#region 成员字段
		private ObservableCollection<IMetadataProvider> _providers;
		#endregion

		#region 构造函数
		public MetadataManager()
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

		public ICollection<EntityMetadata> Entities
		{
			get
			{
				return null;
			}
		}

		public ICollection<CommandMetadata> Commands
		{
			get
			{
				return null;
			}
		}
		#endregion

		#region 公共方法
		public CommandMetadata GetCommand(string name)
		{
			throw new NotImplementedException();
		}

		public EntityMetadata GetEntity(string name)
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
