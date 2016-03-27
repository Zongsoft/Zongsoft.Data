using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public interface IDataProvider
	{
		/// <summary>
		/// 获取底层驱动程序的名称。
		/// </summary>
		string DriverName
		{
			get;
		}

		/// <summary>
		/// 获取连接字符串内容。
		/// </summary>
		string ConnectionString
		{
			get;
		}

		/// <summary>
		/// 获取一个表示当前数据提供程序支持的访问模式。
		/// </summary>
		DataAccessMode AccessMode
		{
			get;
		}

		/// <summary>
		/// 获取当前数据提供程序支持的数据模式集合。
		/// </summary>
		DataProviderSchemaCollection Schemas
		{
			get;
		}

		/// <summary>
		/// 创建一个数据命令对象。
		/// </summary>
		/// <returns>返回创建成功的命令对象。</returns>
		DbCommand CreateCommand();

		/// <summary>
		/// 创建一个数据连接对象，连接对象的连接字符串默认为<see cref="ConnectionString"/>属性值。
		/// </summary>
		/// <returns>返回创建成功的数据连接对象。</returns>
		DbConnection CreateConnection();
	}
}
