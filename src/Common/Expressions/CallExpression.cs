using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class CallExpression : IExpression
	{
		#region 构造函数
		protected CallExpression(string name, CallType type, IEnumerable<IExpression> arguments)
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
		}

		public CallType Type
		{
			get;
		}

		public ICollection<IExpression> Arguments
		{
			get;
		}
		#endregion

		#region 静态方法
		public static CallExpression Function(string name, params IExpression[] arguments)
		{
			return new CallExpression(name, CallType.Function, arguments);
		}

		public static CallExpression Function(string name, IEnumerable<IExpression> arguments)
		{
			return new CallExpression(name, CallType.Function, arguments);
		}

		public static CallExpression Procedure(string name, params IExpression[] arguments)
		{
			return new CallExpression(name, CallType.Procedure, arguments);
		}

		public static CallExpression Procedure(string name, IEnumerable<IExpression> arguments)
		{
			return new CallExpression(name, CallType.Procedure, arguments);
		}
		#endregion
	}
}
