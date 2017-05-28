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
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataOperation
	{
		#region 成员字段
		private IsolationLevel _isolationLevel;
		private IList<DataCommand> _commands;
		#endregion

		#region 构造函数
		public DataOperation(IEnumerable<DataCommand> commands = null, IsolationLevel isolationLevel = System.Data.IsolationLevel.Unspecified)
		{
			_isolationLevel = isolationLevel;

			if(commands != null)
				_commands = new List<DataCommand>(commands);
			else
				_commands = new List<DataCommand>();
		}
		#endregion

		#region 公共属性
		public IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
			set
			{
				_isolationLevel = value;
			}
		}

		public IList<DataCommand> Commands
		{
			get
			{
				return _commands;
			}
		}
		#endregion

		public class DataCommand
		{
			DataCommandKind Kind;
			public DbCommand Command;
		}

		public enum DataCommandKind
		{
			None,
			Reader,
			Scalar,
		}
	}
}
