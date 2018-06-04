using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class VariableIdentifier : Expression, IIdentifier
	{
		#region 构造函数
		public VariableIdentifier(string name, bool isGlobal = false)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();
			this.IsGlobal = isGlobal;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get;
		}

		public bool IsGlobal
		{
			get;
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个全局变量标识。
		/// </summary>
		/// <param name="name">指定的要创建的变量名。</param>
		/// <returns>返回新建的全局变量标识。</returns>
		public static VariableIdentifier Global(string name)
		{
			return new VariableIdentifier(name, true);
		}
		#endregion
	}
}
