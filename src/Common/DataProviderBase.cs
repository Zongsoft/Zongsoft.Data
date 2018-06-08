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
using System.Data;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Common
{
	public abstract class DataProviderBase : IDataProvider
	{
		#region 成员字段
		private string _name;
		private IMetadataProviderManager _metadata;
		private IStatementBuilder _builder;
		private IStatementScriptor _scriptor;
		#endregion

		#region 构造函数
		protected DataProviderBase(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public IMetadataProviderManager Metadata
		{
			get
			{
				return _metadata;
			}
			protected set
			{
				_metadata = value ?? throw new ArgumentNullException();
			}
		}

		public IStatementBuilder Builder
		{
			get
			{
				return _builder;
			}
			protected set
			{
				_builder = value ?? throw new ArgumentNullException();
			}
		}

		public IStatementScriptor Scriptor
		{
			get
			{
				return _scriptor;
			}
			protected set
			{
				_scriptor = value ?? throw new ArgumentNullException();
			}
		}
		#endregion

		#region 执行方法
		public void Execute(DataAccessContextBase context)
		{
			this.OnExecute(context);
		}
		#endregion

		#region 抽象方法
		protected abstract void OnExecute(DataAccessContextBase context);
		#endregion
	}
}
