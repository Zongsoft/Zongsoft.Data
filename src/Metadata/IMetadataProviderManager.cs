using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示元数据的提供程序管理者的接口。
	/// </summary>
	public interface IMetadataProviderManager : IMetadataProvider
	{
		/// <summary>
		/// 获取元数据提供程序集合。
		/// </summary>
		ICollection<IMetadataProvider> Providers
		{
			get;
		}
	}
}
