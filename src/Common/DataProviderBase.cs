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

namespace Zongsoft.Data.Common
{
	public abstract class DataProviderBase : IDataProvider
	{
		#region 成员字段
		private string _name;
		private string _scope;
		private string _driverName;
		private string _connectionString;
		private DataAccessMode _accessMode;
		#endregion

		#region 构造函数
		protected DataProviderBase(string name, string driverName)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(driverName));

			_name = name.Trim();
			_driverName = driverName;
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

		public string DriverName
		{
			get
			{
				return _driverName;
			}
		}

		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
			}
		}

		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}

		public DataAccessMode AccessMode
		{
			get
			{
				return _accessMode;
			}
			set
			{
				_accessMode = value;
			}
		}
		#endregion

		#region 抽象方法
		public abstract IDbCommand CreateCommand();
		public abstract IDbConnection CreateConnection();
		#endregion
	}
}
