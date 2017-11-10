using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class EntitySimplexPropertyMetadata : EntityPropertyMetadata
	{
		#region 成员字段
		private int _length;
		private bool _nullable;
		private byte _precision;
		private byte _scale;
		#endregion

		#region 构造函数
		public EntitySimplexPropertyMetadata(EntityMetadata entity, string name, Type type) : base(entity, name, type)
		{
		}
		#endregion

		#region 公共属性
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

		#region 重写属性
		public override bool IsComplex
		{
			get
			{
				return false;
			}
		}

		public override bool IsSimplex
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}
