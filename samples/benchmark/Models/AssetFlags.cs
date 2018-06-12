using System;
using System.ComponentModel;

namespace Zongsoft.Data.Benchmark.Models
{
	/// <summary>
	/// 表示设施标记的枚举。
	/// </summary>
	public enum AssetFlags : uint
	{
		/// <summary>未定义</summary>
		None = 0,

		/// <summary>已挂牌</summary>
		Plated = 1,
	}
}
