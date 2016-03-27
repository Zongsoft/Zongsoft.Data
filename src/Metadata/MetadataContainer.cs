using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataContainer : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private MetadataCommandCollection _commands;
		private MetadataEntityCollection _entities;
		private MetadataAssociationCollection _associations;
		#endregion

		#region 构造函数
		public MetadataContainer(string name, MetadataFile file, MetadataElementKind kind) : base(kind, file)
		{
			_name = name == null ? string.Empty : name.Trim();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取容器的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取容器所属的映射文件。
		/// </summary>
		public MetadataFile File
		{
			get
			{
				return (MetadataFile)base.Owner;
			}
		}

		public MetadataCommandCollection Commands
		{
			get
			{
				if(_commands == null)
					System.Threading.Interlocked.CompareExchange(ref _commands, new MetadataCommandCollection(this), null);

				return _commands;
			}
		}

		public MetadataEntityCollection Entities
		{
			get
			{
				if(_entities == null)
					System.Threading.Interlocked.CompareExchange(ref _entities, new MetadataEntityCollection(this), null);

				return _entities;
			}
		}

		public MetadataAssociationCollection Associations
		{
			get
			{
				if(_associations == null)
					System.Threading.Interlocked.CompareExchange(ref _associations, new MetadataAssociationCollection(this), null);

				return _associations;
			}
		}
		#endregion
	}
}
