using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class MethodExpression : Expression
	{
		#region 构造函数
		protected MethodExpression(string name, MethodType type, IEnumerable<IExpression> arguments)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();
			this.Type = type;
			this.Arguments = new List<IExpression>(arguments);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get;
			set;
		}

		public string Alias
		{
			get;
			set;
		}

		public MethodType Type
		{
			get;
		}

		public IList<IExpression> Arguments
		{
			get;
		}
		#endregion

		#region 静态方法
		public static MethodExpression Function(string name, params IExpression[] arguments)
		{
			return new MethodExpression(name, MethodType.Function, arguments);
		}

		public static MethodExpression Function(string name, IEnumerable<IExpression> arguments)
		{
			return new MethodExpression(name, MethodType.Function, arguments);
		}

		public static MethodExpression Procedure(string name, params IExpression[] arguments)
		{
			return new MethodExpression(name, MethodType.Procedure, arguments);
		}

		public static MethodExpression Procedure(string name, IEnumerable<IExpression> arguments)
		{
			return new MethodExpression(name, MethodType.Procedure, arguments);
		}
		#endregion
	}
}
