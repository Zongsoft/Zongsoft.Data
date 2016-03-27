using System;
using System.ComponentModel;

namespace Zongsoft.Data
{
	/// <summary>
	/// 定义用于数据提供程序的访问方式。
	/// </summary>
	[Flags]
	public enum DataAccessMode
	{
		/// <summary>读取数据。</summary>
		Read = 1,
		/// <summary>写入数据(新增、删除、修改)。</summary>
		Write = 2,
		/// <summary>读写数据。</summary>
		ReadWrite = 3,
	}
}
