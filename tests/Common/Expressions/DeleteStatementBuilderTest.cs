using System;
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class DeleteStatementBuilderTest
	{
		#region 常量定义
		private const string APPLICATION_NAME = "Community";
		#endregion

		#region 成员变量
		private readonly IDataProvider _provider;
		#endregion

		#region 构造函数
		public DeleteStatementBuilderTest()
		{
			_provider = Utility.GetProvider(APPLICATION_NAME);
		}
		#endregion

		#region 测试方法
		[Fact]
		public void Test()
		{
			var context = new DataDeleteContext(new DataAccess(APPLICATION_NAME),
				"Asset", //name
				Condition.Equal("Principal.User.Name", "Popeye"), //condition
				"Principal{Department}" //schema
				);

			var source = _provider.Connector.GetSource(context);
			Assert.NotNull(source);

			var statements = source.Driver.Builder.Build(context, source);
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var script = source.Driver.CreateCommand(statements.First());
			Assert.NotNull(script);
			Assert.NotNull(script.CommandText);
			Assert.NotNull(script.Parameters);
			Assert.True(script.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(script.CommandText);
		}
		#endregion
	}
}
