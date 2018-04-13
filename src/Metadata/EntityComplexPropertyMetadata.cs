/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体复合属性的元数据类。
	/// </summary>
	public class EntityComplexPropertyMetadata : EntityPropertyMetadata, IEntityComplexProperty
	{
		#region 成员字段
		private bool _isMultiple;
		private IEntityAssociation _relationship;
		#endregion

		#region 构造函数
		public EntityComplexPropertyMetadata(IEntity entity, string name, Type type) : base(entity, name, type)
		{
			_isMultiple = false;
		}

		public EntityComplexPropertyMetadata(IEntity entity, string name, Type type, bool isMultiple) : base(entity, name, type)
		{
			_isMultiple = isMultiple;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示该复合属性关联的是否为一对多的关系。
		/// </summary>
		public bool IsMultiple
		{
			get
			{
				return _isMultiple;
			}
		}

		/// <summary>
		/// 获取复合属性关联的关系。
		/// </summary>
		public IEntityAssociation Relationship
		{
			get
			{
				return _relationship;
			}
		}
		#endregion

		#region 重写属性
		/// <summary>
		/// 获取一个值，指示数据实体属性是否为复合类型。该重写方法始终返回真(True)。
		/// </summary>
		public override bool IsComplex
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为单值类型。该重写方法始终返回假(False)。
		/// </summary>
		public override bool IsSimplex
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var text = new System.Text.StringBuilder();

			foreach(var member in _relationship.Members)
			{
				if(text.Length > 0)
					text.Append(" AND ");

				text.Append(member.Principal.Name + "=" + member.Foreign.Name);
			}

			return $"{this.Name} -> {_relationship.Foreign} ({text.ToString()})";
		}
		#endregion
	}
}
