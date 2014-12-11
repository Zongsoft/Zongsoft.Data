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
