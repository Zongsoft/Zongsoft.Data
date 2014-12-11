using System;
using System.ComponentModel;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示元素所属类别的枚举。
	/// </summary>
	public enum MetadataElementKind
	{
		/// <summary>未定义。</summary>
		None,

		/// <summary>概念层元素。</summary>
		Concept,

		/// <summary>存储层元素。</summary>
		Storage,

		/// <summary>映射元素。</summary>
		Mapping,
	}
}
