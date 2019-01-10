using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyDriver : DataDriverBase
	{
		#region 构造函数
		public DummyDriver()
		{
			//this.Features.Add(Feature.MultipleActiveResultSets);
			//this.Features.Add(DeleteFeatures.Multitable);
		}
		#endregion

		#region 公共属性
		public override string Name
		{
			get
			{
				return "dummy";
			}
		}

		public override IStatementBuilder Builder
		{
			get
			{
				return DummyStatementBuilder.Default;
			}
		}
		#endregion

		#region 公共方法
		public override DbCommand CreateCommand()
		{
			return new OleDbCommand();
		}

		public override DbCommand CreateCommand(string text, CommandType commandType = CommandType.Text)
		{
			return new OleDbCommand(text)
			{
				CommandType = commandType,
			};
		}

		public override DbConnection CreateConnection()
		{
			return new OleDbConnection();
		}

		public override DbConnection CreateConnection(string connectionString)
		{
			return new OleDbConnection(connectionString);
		}
		#endregion

		#region 保护方法
		protected override IExpressionVisitor CreateVisitor()
		{
			return new DummyExpressionVisitor();
		}
		#endregion
	}
}
