using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class TableIdentifier : IIdentifier, ISource
	{
		#region 构造函数
		public TableIdentifier(string name, string alias = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();
			this.Alias = alias;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get;
		}

		public string Alias
		{
			get;
			set;
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 创建一个关联当前表的字段标识。
		/// </summary>
		/// <param name="name">指定的字段名称。</param>
		/// <param name="alias">指定的字段别名。</param>
		/// <returns>返回创建成功的字段标识。</returns>
		public FieldIdentifier CreateField(string name, string alias = null)
		{
			return new FieldIdentifier(this, name, alias);
		}
		#endregion
	}
}
