using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class JoinClause : ISource
	{
		#region 成员字段
		private IExpression _condition;
		#endregion

		#region 构造函数
		public JoinClause(string name, ISource target, JoinType type = JoinType.Left)
		{
			this.Name = name ?? string.Empty;
			this.Target = target ?? throw new ArgumentNullException(nameof(target));
			this.Condition = ConditionExpression.And();
			this.Type = type;
		}

		public JoinClause(string name, ISource target, IExpression condition, JoinType type = JoinType.Left)
		{
			this.Name = name ?? string.Empty;
			this.Target = target ?? throw new ArgumentNullException(nameof(target));
			this.Condition = condition ?? throw new ArgumentNullException(nameof(IExpression));
			this.Type = type;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关联子句的标识名。
		/// </summary>
		/// <remarks>
		///		<para>在概念层生成中通过该属性来表示导航属性的名称。</para>
		/// </remarks>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取关联子句的别名，即关联的目标表（源）的别名。
		/// </summary>
		public string Alias
		{
			get
			{
				return this.Target.Alias;
			}
		}

		/// <summary>
		/// 获取关联的目标表（源）。
		/// </summary>
		public ISource Target
		{
			get;
		}

		/// <summary>
		/// 获取关联的种类。
		/// </summary>
		public JoinType Type
		{
			get;
		}

		/// <summary>
		/// 获取或设置关联的连接条件。
		/// </summary>
		public IExpression Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value ?? throw new ArgumentNullException();
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 创建一个关联当前关联子句的字段标识。
		/// </summary>
		/// <param name="name">指定的字段名称。</param>
		/// <param name="alias">指定的字段别名。</param>
		/// <returns>返回创建成功的字段标识。</returns>
		public FieldIdentifier CreateField(string name, string alias = null)
		{
			if(string.IsNullOrWhiteSpace(alias) && string.IsNullOrEmpty(this.Name))
				alias = this.Name + "." + name;

			return new FieldIdentifier(this, name, alias);
		}
		#endregion
	}
}
