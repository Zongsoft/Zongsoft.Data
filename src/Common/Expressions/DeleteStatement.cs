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

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class DeleteStatement : Statement
	{
		#region 成员字段
		private ICollection<DeleteStatement> _slaves;
		#endregion

		#region 构造函数
		public DeleteStatement(IEntityMetadata entity, params TableIdentifier[] tables)
		{
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.From = new List<ISource>();

			if(tables != null)
				this.Tables = new List<TableIdentifier>(tables);
		}
		#endregion

		#region 公共属性
		public IEntityMetadata Entity
		{
			get;
		}

		public IExpression Output
		{
			get;
			set;
		}

		public ICollection<TableIdentifier> Tables
		{
			get;
		}

		public ICollection<ISource> From
		{
			get;
		}

		public IExpression Where
		{
			get;
			set;
		}

		public bool HasSlaves
		{
			get
			{
				return _slaves != null && _slaves.Count > 0;
			}
		}

		public ICollection<DeleteStatement> Slaves
		{
			get
			{
				if(_slaves == null)
				{
					lock(this)
					{
						if(_slaves == null)
							_slaves = new List<DeleteStatement>();
					}
				}

				return _slaves;
			}
		}
		#endregion
	}
}
