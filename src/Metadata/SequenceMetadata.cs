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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示序号器的元数据类。
	/// </summary>
	public class SequenceMetadata
	{
		#region 成员字段
		private IList<string> _referenceNames;
		private IEntitySimplexPropertyMetadata[] _references;
		#endregion

		#region 构造函数
		public SequenceMetadata(IEntitySimplexPropertyMetadata property, string name, int seed, int interval = 1)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Property = property ?? throw new ArgumentNullException(nameof(property));
			this.Name = name.Trim();
			this.Seed = seed;
			this.Interval = interval == 0 ? 1 : interval;

			if(this.Name == "#")
				this.Name = "#" + property.Entity.Name + ":" + property.Name;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取序号所属的数据实体元素。
		/// </summary>
		public IEntityMetadata Entity
		{
			get => this.Property.Entity;
		}

		/// <summary>
		/// 获取序号所属的数据属性元素。
		/// </summary>
		public IEntitySimplexPropertyMetadata Property
		{
			get;
		}

		/// <summary>
		/// 获取序号器的名称。
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取或设置序号器的种子数。
		/// </summary>
		public int Seed
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置序号器的递增量，默认为1。
		/// </summary>
		public int Interval
		{
			get; set;
		}

		/// <summary>
		/// 获取一个值，指示是否采用数据库内置序号方案。
		/// </summary>
		public bool IsBuiltin
		{
			get
			{
				var name = this.Name;
				return name == null || name.Length == 0 || name[0] != '#';
			}
		}

		/// <summary>
		/// 获取一个值，指示是否采用外置序号器方案。
		/// </summary>
		public bool IsExternal
		{
			get
			{
				var name = this.Name;
				return name != null && name.Length > 0 && name[0] == '#';
			}
		}

		/// <summary>
		/// 获取序号的引用的属性数组。
		/// </summary>
		public IEntitySimplexPropertyMetadata[] References
		{
			get
			{
				if(_references == null)
				{
					if(_referenceNames == null || _referenceNames.Count == 0)
						return null;

					var references = new IEntitySimplexPropertyMetadata[_referenceNames.Count];

					for(int i = 0; i < references.Length; i++)
					{
						if(this.Entity.Properties.TryGet(_referenceNames[i], out var property) && property.IsSimplex)
							references[i] = (IEntitySimplexPropertyMetadata)property;
						else
						{
							if(property == null)
								throw new DataException($"The specified '{_referenceNames[i]}' member of the '{this.Name}' sequence is a property that does not exist.");
							else
								throw new DataException($"The specified '{_referenceNames[i]}' member of the '{this.Name}' sequence must be a simplex property.");
						}
					}

					_references = references;
				}

				return _references;
			}
		}
		#endregion

		#region 内部方法
		internal void SetReferences(IList<string> references)
		{
			if(references == null || references.Count == 0)
				return;

			_referenceNames = references;
		}
		#endregion
	}
}
