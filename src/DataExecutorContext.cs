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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Runtime;

namespace Zongsoft.Data
{
	public class DataExecutorContext : Zongsoft.Services.Composition.ExecutionContext
	{
		#region 成员字段
		private MetadataManager _metadataManager;
		private DataAccessAction _action;
		private string _actionName;
		#endregion

		#region 构造函数
		public DataExecutorContext(DataExecutor executor, MetadataManager metadataManager, string actionName, DataParameter parameter) : base(executor, parameter)
		{
			if(metadataManager == null)
				throw new ArgumentNullException("metadataManager");

			if(parameter == null)
				throw new ArgumentNullException("parameter");

			_metadataManager = metadataManager;
			this.ActionName = actionName;
		}

		public DataExecutorContext(DataExecutor executor, MetadataManager metadataManager, DataAccessAction action, DataParameter parameter) : base(executor, parameter)
		{
			if(metadataManager == null)
				throw new ArgumentNullException("metadataManager");

			if(parameter == null)
				throw new ArgumentNullException("parameter");

			_metadataManager = metadataManager;
			_action = action;
			_actionName = action.ToString();
		}
		#endregion

		#region 公共属性
		public IDataAccess DataAccess
		{
			get
			{
				return ((DataExecutor)this.Executor).DataAccess;
			}
		}

		public MetadataManager MetadataManager
		{
			get
			{
				return _metadataManager;
			}
		}

		public DataAccessAction Action
		{
			get
			{
				return _action;
			}
		}

		public string ActionName
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_actionName))
					return _action.ToString();

				return _actionName;
			}
			private set
			{
				_actionName = value == null ? string.Empty : value.Trim();

				if(!Enum.TryParse(_actionName, out _action))
					_action = DataAccessAction.Other;
			}
		}
		#endregion
	}
}
