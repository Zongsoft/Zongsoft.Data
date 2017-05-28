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
using System.Linq;
using System.Text;

using Zongsoft.Services;

namespace Zongsoft.Data.Runtime
{
	public abstract class DataProviderBase : IDataProvider
	{
		private string _name;
		private DataAccessMode _accessMode;
		private DbProviderFactory _dbProvider;
		private DataProviderSchemaCollection _schemas;

		protected DataProviderBase(string name, Zongsoft.Services.IServiceProvider services)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();

			if(services != null && !string.IsNullOrWhiteSpace(this.DriverName))
				_dbProvider = services.Resolve<DbProviderFactory>(this.DriverName);
		}

		public virtual string DriverName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string ConnectionString
		{
			get
			{
				throw new NotImplementedException();
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

		public DataProviderSchemaCollection Schemas
		{
			get
			{
				return _schemas;
			}
		}

		public virtual DbCommand CreateCommand()
		{
			if(_dbProvider == null)
				return null;

			return _dbProvider.CreateCommand();
		}

		public virtual DbConnection CreateConnection()
		{
			if(_dbProvider == null)
				return null;

			var connection = _dbProvider.CreateConnection();
			connection.ConnectionString = this.ConnectionString;
			return connection;
		}
	}
}
