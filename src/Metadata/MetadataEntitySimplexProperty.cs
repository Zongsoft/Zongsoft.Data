using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示简单属性元素的类。
	/// </summary>
	public class MetadataEntitySimplexProperty : MetadataEntityProperty
	{
		#region 成员字段
		private Type _type;
		private string _typeName;
		private int _length;
		private bool _nullable;
		private byte _precision;
		private byte _scale;
		#endregion

		#region 构造函数
		public MetadataEntitySimplexProperty(string name, string typeName) : base(name)
		{
			if(string.IsNullOrWhiteSpace(typeName))
				throw new ArgumentNullException("typeName");

			_typeName = typeName;
		}

		public MetadataEntitySimplexProperty(string name, Type type) : base(name)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			_type = type;
			_typeName = type.Name;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取单值属性的本地类型。
		/// </summary>
		public Type Type
		{
			get
			{
				if(_type == null && this.Kind == MetadataElementKind.Concept)
					_type = Zongsoft.Common.TypeExtension.GetType(_typeName);

				return _type;
			}
		}

		/// <summary>
		/// 获取单值属性的类型名。
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
		}

		/// <summary>
		/// 获取或设置属性/字段的最大长度，单位：字节。
		/// </summary>
		public int Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = Math.Max(value, -1);
			}
		}

		/// <summary>
		/// 获取或设置属性/字段是否允许为空。
		/// </summary>
		public bool Nullable
		{
			get
			{
				return _nullable;
			}
			set
			{
				_nullable = value;
			}
		}

		/// <summary>
		/// 获取或设置属性/字段的精度。
		/// </summary>
		public byte Precision
		{
			get
			{
				return _precision;
			}
			set
			{
				_precision = value;
			}
		}

		/// <summary>
		/// 获取或设置属性/字段的小数点位数。
		/// </summary>
		public byte Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
			}
		}
		#endregion
	}
}
