using System;
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class CountStatementBuilderTest
	{
		#region 常量定义
		private const string APPLICATION_NAME = "Community";
		#endregion

		#region 成员变量
		private readonly IDataProvider _provider;
		#endregion

		#region 构造函数
		public CountStatementBuilderTest()
		{
			_provider = Utility.GetProvider(APPLICATION_NAME);
		}
		#endregion

		#region 测试方法
		[Fact]
		public void Test()
		{
			const string NAME = "Asset";

			var accessor = new DataAccess(APPLICATION_NAME);

			var context = new DataCountContext(accessor,
				NAME, //name
				Condition.Equal("Principal", 100), // Condition.Equal("Principal.User.Name", "Popeye"), //condition
				"Name" //member
				);

			var statements = context.Build().ToArray();
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var command = context.Session.Build(statements[0]);
			Assert.NotNull(command);
			Assert.NotNull(command.CommandText);
			Assert.True(command.CommandText.Length > 0);

			System.Diagnostics.Debug.WriteLine(command.CommandText);
		}
		#endregion
	}
}
