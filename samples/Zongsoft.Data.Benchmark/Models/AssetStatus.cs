using System;
using System.ComponentModel;

namespace Zongsoft.Data.Benchmark.Models
{
	/// <summary>
	/// 表示资产设施状态的枚举。
	/// </summary>
	public enum AssetStatus : byte
	{
		/// <summary>正常</summary>
		Normal,

		/// <summary>禁用(备案)</summary>
		Disabled,

		/// <summary>废弃(拆除)</summary>
		Discarded,

		/// <summary>隐患</summary>
		Danger,

		/// <summary>故障</summary>
		Fault,

		/// <summary>未交付</summary>
		Undelivered,

		/// <summary>未知</summary>
		Unknown = 99,
	}
}
