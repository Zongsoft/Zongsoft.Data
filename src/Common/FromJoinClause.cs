/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Metadata.Schema;

namespace Zongsoft.Data.Common
{
	public class FromJoinClause
	{
		#region 成员字段
		private string _alias;
		private MetadataEntity _entity;
		private IList<MetadataEntityProperty> _refs;
		private IList<MetadataEntityProperty> _dependentRefs;
		private string _navigationPropertyName;
		private FromClause _from;
		private FromJoinClause _parent;
		private FromJoinClauseCollection _children;
		#endregion

		#region 构造函数
		public FromJoinClause(int aliasId, MetadataEntity entity, IList<MetadataEntityProperty> refs, IList<MetadataEntityProperty> dependentRefs)
		{
			_alias = "t" + aliasId.ToString();
			_entity = entity;
			_refs = refs;
			_dependentRefs = dependentRefs;
		}

		public FromJoinClause(string alias, MetadataEntity entity, IList<MetadataEntityProperty> refs, IList<MetadataEntityProperty> dependentRefs)
		{
			_alias = alias;
			_entity = entity;
			_refs = refs;
			_dependentRefs = dependentRefs;
		}
		#endregion

		#region 公共属性
		public string Alias
		{
			get
			{
				return _alias;
			}
		}

		public MetadataEntity Entity
		{
			get
			{
				return _entity;
			}
		}

		public IList<MetadataEntityProperty> Refs
		{
			get
			{
				return _refs;
			}
		}

		public IList<MetadataEntityProperty> DependentRefs
		{
			get
			{
				return _dependentRefs;
			}
		}

		public string NavigationPropertyName
		{
			get
			{
				return _navigationPropertyName;
			}
			set
			{
				_navigationPropertyName = value;
			}
		}

		public FromJoinClauseCollection Children
		{
			get
			{
				if(_children == null)
					System.Threading.Interlocked.CompareExchange(ref _children, new FromJoinClauseCollection(this), null);

				return _children;
			}
		}
		#endregion
	}
}
