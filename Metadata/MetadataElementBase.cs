using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public abstract class MetadataElementBase : MarshalByRefObject
	{
		#region 成员字段
		private object _owner;
		private MetadataElementKind? _kind;
		private MetadataElementAttributeCollection _attributes;
		#endregion

		#region 构造函数
		protected MetadataElementBase()
		{
		}

		protected MetadataElementBase(MetadataElementKind kind, object owner)
		{
			_kind = kind;
			_owner = owner;
		}
		#endregion

		#region 公共属性
		public MetadataElementKind Kind
		{
			get
			{
				if(_kind == null)
				{
					var parent = _owner as MetadataElementBase;

					if(parent == null)
						_kind = MetadataElementKind.None;
					else
						_kind = parent.Kind;
				}

				return _kind.Value;
			}
		}

		public MetadataElementAttributeCollection Attributes
		{
			get
			{
				if(_attributes == null)
					System.Threading.Interlocked.CompareExchange(ref _attributes, new MetadataElementAttributeCollection(), null);

				return _attributes;
			}
		}
		#endregion

		#region 公共方法
		public string GetAttributeValue(string name)
		{
			if(_kind == MetadataElementKind.Concept)
				return this.GetAttributeValue(name, MetadataUri.Concept);

			if(_kind == MetadataElementKind.Storage)
				return this.GetAttributeValue(name, MetadataUri.Storage);

			return this.GetAttributeValue(name, null);
		}

		public string GetAttributeValue(string name, string namespaceUri)
		{
			var attributes = _attributes;

			if(attributes != null)
			{
				var attribute = attributes[name, namespaceUri];

				if(attribute != null)
					return attribute.Value;
			}

			return null;
		}
		#endregion

		#region 保护属性
		/// <summary>
		/// 获取当前元素的所有者对象。
		/// </summary>
		internal protected object Owner
		{
			get
			{
				return _owner;
			}
			internal set
			{
				_owner = value;
			}
		}
		#endregion
	}
}
