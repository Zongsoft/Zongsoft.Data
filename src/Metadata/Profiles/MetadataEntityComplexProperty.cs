﻿/*
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
	/// 表示数据实体复合属性的元数据类。
	/// </summary>
	public class MetadataEntityComplexProperty : MetadataEntityProperty, IDataEntityComplexProperty
	{
		#region 成员字段
		private IDataEntity _foreign;
		private IDataEntityProperty _foreignProperty;
		#endregion

		#region 构造函数
		public MetadataEntityComplexProperty(IDataEntity entity, string name, string role, bool immutable = true) : base(entity, name, immutable)
		{
			if(string.IsNullOrWhiteSpace(role))
				throw new ArgumentNullException(nameof(role));

			this.Role = role.Trim();
		}
		#endregion

		#region 公共属性
		public IDataEntity Foreign
		{
			get
			{
				if(_foreign == null)
					this.UpdateForeign();

				return _foreign;
			}
		}

		public IDataEntityProperty ForeignProperty
		{
			get
			{
				if(_foreign == null)
					this.UpdateForeign();

				return _foreignProperty;
			}
		}

		public string Role
		{
			get;
		}

		public DataAssociationMultiplicity Multiplicity
		{
			get;
			set;
		}

		public DataAssociationLink[] Links
		{
			get;
			set;
		}

		public DataAssociationConstraint[] Constraints
		{
			get;
			set;
		}
		#endregion

		#region 重写属性
		/// <summary>
		/// 获取一个值，指示数据实体属性是否为主键。该重写方法始终返回假(False)。
		/// </summary>
		public override bool IsPrimaryKey
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为复合类型。该重写方法始终返回真(True)。
		/// </summary>
		public override bool IsComplex
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// 获取一个值，指示数据实体属性是否为单值类型。该重写方法始终返回假(False)。
		/// </summary>
		public override bool IsSimplex
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var text = new System.Text.StringBuilder();

			foreach(var link in this.Links)
			{
				if(text.Length > 0)
					text.Append(" AND ");

				text.Append(link.ToString());
			}

			return $"{this.Name} -> {this.Role} ({text.ToString()})";
		}
		#endregion

		#region 私有方法
		private void UpdateForeign()
		{
			var index = this.Role.IndexOf(':');

			if(index < 0)
				_foreign = this.Entity.Metadata.Manager.Entities.Get(this.Role);
			else
			{
				_foreign = this.Entity.Metadata.Manager.Entities.Get(this.Role.Substring(0, index));
				_foreignProperty = _foreign.Properties.Get(this.Role.Substring(index + 1));
			}
		}
		#endregion
	}
}
