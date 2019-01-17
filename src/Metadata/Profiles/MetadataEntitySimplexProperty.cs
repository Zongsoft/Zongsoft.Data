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
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata.Profiles
{
	/// <summary>
	/// 表示数据实体单值属性的元数据类。
	/// </summary>
	public class MetadataEntitySimplexProperty : MetadataEntityProperty, IEntitySimplexPropertyMetadata
	{
		#region 成员字段
		private bool _isPrimaryKey;
		private int _length;
		private bool _nullable;
		private byte _precision;
		private byte _scale;
		private string _valueText;
		#endregion

		#region 构造函数
		public MetadataEntitySimplexProperty(MetadataEntity entity, string name, System.Data.DbType type) : base(entity, name, type)
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置属性的默认值。
		/// </summary>
		public object Value
		{
			get
			{
				var type = Common.Utility.FromDbType(this.Type);

				if(type == typeof(DateTime))
				{
					switch(_valueText)
					{
						case "today":
						case "today()":
							return DateTime.Today;
						case "now":
						case "now()":
							return DateTime.Now;
					}
				}
				else if(type == typeof(DateTimeOffset))
				{
					switch(_valueText)
					{
						case "today":
						case "today()":
							return DateTime.Today.ToUniversalTime();
						case "now":
						case "now()":
							return DateTime.UtcNow;
					}
				}

				if(type.IsValueType && _nullable)
				{
					if(string.IsNullOrEmpty(_valueText))
						return null;

					type = typeof(Nullable<>).MakeGenericType(type);
				}

				return Zongsoft.Common.Convert.ConvertValue(_valueText, type);
			}
		}

		/// <summary>
		/// 获取或设置文本或数组属性的最大长度，单位：字节。
		/// </summary>
		public int Length
		{
			get
			{
				var length = _length;

				if(length == 0)
				{
					switch(this.Type)
					{
						case System.Data.DbType.Binary:
							return 100;
						case System.Data.DbType.AnsiString:
						case System.Data.DbType.AnsiStringFixedLength:
						case System.Data.DbType.String:
						case System.Data.DbType.StringFixedLength:
							return 100;
					}
				}

				return length;
			}
			set
			{
				_length = Math.Max(value, -1);
			}
		}

		/// <summary>
		/// 获取或设置属性是否允许为空。
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
		/// 获取或设置序号器名。
		/// </summary>
		public string Sequence
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置数值属性的精度。
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
		/// 获取或设置数值属性的小数点位数。
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
		/// <summary>
		/// 获取一个值，指示数据实体属性是否为主键。
		/// </summary>
		public override bool IsPrimaryKey
		{
			get
			{
				return _isPrimaryKey;
			}
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为复合类型。该重写方法始终返回假(False)。
		/// </summary>
		public override bool IsComplex
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为单值类型。该重写方法始终返回真(True)。
		/// </summary>
		public override bool IsSimplex
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region 内部方法
		internal void SetPrimaryKey()
		{
			_isPrimaryKey = true;
		}

		internal void SetDefaultValue(string value)
		{
			if(value != null)
				_valueText = value.ToLowerInvariant().Trim();
			else
				_valueText = value;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var nullable = _nullable ? "NULL" : "NOT NULL";

			switch(this.Type)
			{
				//处理小数类型
				case System.Data.DbType.Currency:
				case System.Data.DbType.Decimal:
				case System.Data.DbType.Double:
				case System.Data.DbType.Single:
					return $"{this.Name} {this.Type.ToString()}({_precision},{_scale}) [{nullable}]";

				//处理字符串或数组类型
				case System.Data.DbType.Binary:
				case System.Data.DbType.AnsiString:
				case System.Data.DbType.AnsiStringFixedLength:
				case System.Data.DbType.String:
				case System.Data.DbType.StringFixedLength:
					return $"{this.Name} {this.Type.ToString()}({_length}) [{nullable}]";
			}

			return $"{this.Name} {this.Type.ToString()} [{nullable}]";
		}
		#endregion
	}
}
