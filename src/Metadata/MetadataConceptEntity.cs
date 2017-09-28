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
using System.Collections.Concurrent;

namespace Zongsoft.Data.Metadata
{
	public class MetadataConceptEntity : MetadataEntity
	{
		#region 成员字段
		private ConcurrentDictionary<string, MetadataEntityAction> _actions;
		#endregion

		#region 构造函数
		public MetadataConceptEntity(string name) : base(name)
		{
			_actions = new ConcurrentDictionary<string, MetadataEntityAction>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取实体类型元素所属的容器元素。
		/// </summary>
		public MetadataConceptContainer Container
		{
			get
			{
				return (MetadataConceptContainer)base.Owner;
			}
		}

		public MetadataEntityAction Delete
		{
			get
			{
				if(_actions.TryGetValue("delete", out var action))
					return action;

				return null;
			}
		}

		public MetadataEntityAction Insert
		{
			get
			{
				if(_actions.TryGetValue("insert", out var action))
					return action;

				return null;
			}
		}

		public MetadataEntityAction Update
		{
			get
			{
				if(_actions.TryGetValue("update", out var action))
					return action;

				return null;
			}
		}

		public MetadataEntityAction Upsert
		{
			get
			{
				if(_actions.TryGetValue("upsert", out var action))
					return action;

				return null;
			}
		}
		#endregion

		#region 内部方法
		internal bool SetAction(MetadataEntityAction action)
		{
			if(action == null)
				return false;

			action.Entity = this;
			_actions[action.Name] = action;
			return true;
		}
		#endregion
	}
}
