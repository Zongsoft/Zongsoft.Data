using System;
using System.Data;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class ParameterExpression : Expression
	{
		#region 构造函数
		public ParameterExpression(string name, string path, FieldIdentifier field)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			this.Name = name;
			this.Path = path;
			this.Field = field ?? throw new ArgumentNullException(nameof(field));
			this.Direction = ParameterDirection.Input;
		}

		public ParameterExpression(string name, object value, FieldIdentifier field)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();
			this.Value = value;
			this.Field = field ?? throw new ArgumentNullException(nameof(field));
			this.Direction = ParameterDirection.Input;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get;
			internal set;
		}

		public string Path
		{
			get;
		}

		public ParameterDirection Direction
		{
			get;
			set;
		}

		public DbType DbType
		{
			get;
			set;
		}

		public FieldIdentifier Field
		{
			get;
		}

		public object Value
		{
			get;
			set;
		}

		public Condition Condition
		{
			get;
		}
		#endregion

		#region 公共方法
		public IDbDataParameter Inject(IDbCommand command)
		{
			if(command == null)
				throw new ArgumentNullException(nameof(command));

			//通过命令创建一个新的空参数
			var parameter = command.CreateParameter();

			parameter.ParameterName = this.Name;
			parameter.DbType = this.DbType;
			parameter.Direction = this.Direction;

			if(string.IsNullOrEmpty(this.Path))
				parameter.Value = this.Value;

			//将参数加入到命令的参数集中
			command.Parameters.Add(parameter);

			//返回新建的参数对象
			return parameter;
		}
		#endregion
	}
}
