using System;
using System.ComponentModel;

namespace Zongsoft.Data.Benchmark.Models
{
	/// <summary>
	/// 表示资产设施数据来源的枚举。
	/// </summary>
	public enum AssetSource : byte
	{
		/// <summary>手动添加</summary>
		Manually,

		/// <summary>设备采集</summary>
		Gather,
	}
}
