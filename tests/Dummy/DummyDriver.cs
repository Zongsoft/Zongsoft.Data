using System;
using System.Data;
using System.Data.OleDb;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyDriver : IDataDriver
	{
		#region 公共属性
		public string Name
		{
			get
			{
				return "dummy";
			}
		}

		public FeatureCollection Features
		{
			get;
		}

		public IStatementBuilder Builder
		{
			get
			{
				return DummyStatementBuilder.Default;
			}
		}

		public IStatementScriptor Scriptor
		{
			get
			{
				return DummyStatementScriptor.Default;
			}
		}
		#endregion

		#region 公共方法
		public IDbCommand CreateCommand()
		{
			return new OleDbCommand();
		}

		public IDbCommand CreateCommand(IStatement statement)
		{
			if(statement == null)
				throw new ArgumentNullException(nameof(statement));

			var script = this.Scriptor.Script(statement);
			var command = new OleDbCommand(script.Text);

			foreach(var parameter in script.Parameters)
			{
				parameter.Attach(command);
			}

			return command;
		}

		public IDbCommand CreateCommand(string text, CommandType commandType = CommandType.Text)
		{
			return new OleDbCommand(text)
			{
				CommandType = commandType,
			};
		}

		public IDbConnection CreateConnection()
		{
			return new OleDbConnection();
		}

		public IDbConnection CreateConnection(string connectionString)
		{
			return new OleDbConnection(connectionString);
		}
		#endregion
	}
}
