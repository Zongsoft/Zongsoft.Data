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

namespace Zongsoft.Data.Common.Expressions
{
	public abstract class Statement : Expression, IStatement
	{
		#region 成员字段
		private Collections.INamedCollection<ParameterExpression> _parameters;
		private ICollection<IStatement> _slaves;
		#endregion

		#region 构造函数
		protected Statement()
		{
		}

		protected Statement(IStatement master)
		{
		}
		#endregion

		#region 公共属性
		public IStatement Master
		{
			get;
		}

		public virtual bool HasSlaves
		{
			get
			{
				return _slaves != null && _slaves.Count > 0;
			}
		}

		public virtual ICollection<IStatement> Slaves
		{
			get
			{
				if(_slaves == null)
					System.Threading.Interlocked.CompareExchange(ref _slaves, new List<IStatement>(), null);

				return _slaves;
			}
		}

		public virtual bool HasParameters
		{
			get
			{
				return _parameters != null && _parameters.Count > 0;
			}
		}

		public virtual Collections.INamedCollection<ParameterExpression> Parameters
		{
			get
			{
				if(_parameters == null)
				{
					lock(this)
					{
						if(_parameters == null)
							_parameters = this.CreateParameters();
					}
				}

				return _parameters;
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual Collections.INamedCollection<ParameterExpression> CreateParameters()
		{
			return new ParameterCollection();
		}
		#endregion

		#region 嵌套子类
		private class ParameterCollection : Collections.NamedCollectionBase<ParameterExpression>
		{
			private int _index;

			protected override string GetKeyForItem(ParameterExpression item)
			{
				return item.Name;
			}

			protected override void AddItem(ParameterExpression item)
			{
				if(string.IsNullOrEmpty(item.Name) || item.Name == "?")
				{
					var index = System.Threading.Interlocked.Increment(ref _index);
					item.Name = "p" + index.ToString();
				}

				base.AddItem(item);
			}
		}
		#endregion
	}
}
