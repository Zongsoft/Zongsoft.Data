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

namespace Zongsoft.Data.Common
{
	public class DataSource : IDataSource
	{
		#region 成员字段
		private string _name;
		private string _connectionString;
		private string _driverName;
		private IDataDriver _driver;
		private readonly ConnectionPool _pool;
		#endregion

		#region 构造函数
		public DataSource(Zongsoft.Options.Configuration.ConnectionStringElement connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			_name = connectionString.Name;
			_connectionString = connectionString.Value;
			_driverName = connectionString.Provider;
			this.Mode = DataAccessMode.All;
			_pool = new ConnectionPool(this);

			if(connectionString.HasExtendedProperties)
			{
				if(connectionString.ExtendedProperties.TryGetValue("mode", out var mode) && mode != null && mode.Length > 0)
				{
					switch(mode.Trim().ToLowerInvariant())
					{
						case "r":
						case "read":
						case "readonly":
							this.Mode = DataAccessMode.ReadOnly;
							break;
						case "w":
						case "write":
						case "writeonly":
							this.Mode = DataAccessMode.WriteOnly;
							break;
						case "*":
						case "all":
						case "none":
						case "both":
						case "readwrite":
						case "writeread":
							this.Mode = DataAccessMode.All;
							break;
						default:
							throw new Options.Configuration.OptionConfigurationException($"Invalid '{mode}' mode value of the ConnectionString configuration.");
					}
				}
			}
		}

		public DataSource(string name, string connectionString, string driverName = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			if(string.IsNullOrEmpty(connectionString))
				throw new ArgumentNullException(nameof(connectionString));

			_name = name;
			_connectionString = connectionString;
			_driverName = driverName;
			this.Mode = DataAccessMode.All;
			_pool = new ConnectionPool(this);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value;
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
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_connectionString = value;
			}
		}

		public ConnectionPool ConnectionManager
		{
			get
			{
				return _pool;
			}
		}

		public DataAccessMode Mode
		{
			get;
			set;
		}

		public IDataDriver Driver
		{
			get
			{
				if(_driver == null && !string.IsNullOrEmpty(_driverName))
				{
					if(DataEnvironment.Drivers.TryGet(_driverName, out var driver))
						_driver = driver;
					else
						throw new DataException($"The '{_driverName}' data driver does not exist.");
				}

				return _driver;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return $"[{_driverName}]{_name} ({_connectionString})";
		}
		#endregion
	}
}
