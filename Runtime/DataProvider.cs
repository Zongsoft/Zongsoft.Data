using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataProvider : IDataProvider
	{
		#region 成员字段
		private DbProviderFactory _dbProvider;
		private Configuration.DataProviderElement _configuration;
		#endregion

		#region 构造函数
		public DataProvider()
		{
		}

		public DataProvider(Configuration.DataProviderElement configuration, Zongsoft.Services.IServiceProvider services)
		{
			if(configuration == null)
				throw new ArgumentNullException("configuration");

			_configuration = configuration;

			if(services != null)
				_dbProvider = services.Resolve<DbProviderFactory>(this.DriverName);
		}
		#endregion

		public string DriverName
		{
			get
			{
				if(_configuration == null)
					return null;

				return _configuration.DriverName;
			}
		}

		public string ConnectionString
		{
			get
			{
				if(_configuration == null)
					return null;

				return _configuration.ConnectionString;
			}
		}

		public DataAccessMode AccessMode
		{
			get
			{
				if(_configuration == null)
					return DataAccessMode.ReadWrite;

				return _configuration.AccessMode;
			}
		}

		public DataProviderSchemaCollection Schemas
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Configuration.DataProviderElement Configuration
		{
			get
			{
				return _configuration;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_configuration = Configuration;
			}
		}

		public DbProviderFactory DbProvider
		{
			get
			{
				return _dbProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_dbProvider = value;
			}
		}

		public DbCommand CreateCommand()
		{
			var dbProvider = this.DbProvider;

			if(dbProvider == null)
				return null;

			return dbProvider.CreateCommand();
		}

		public DbConnection CreateConnection()
		{
			var dbProvider = this.DbProvider;

			if(dbProvider == null)
				return null;

			var connection = dbProvider.CreateConnection();

			if(connection != null)
				connection.ConnectionString = this.ConnectionString;

			return connection;
		}
	}
}
