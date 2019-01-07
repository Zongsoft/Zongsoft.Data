using System;
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class SelectStatementBuilderTest
	{
		#region 常量定义
		private const string APPLICATION_NAME = "Community";
		#endregion

		#region 成员变量
		private readonly IDataProvider _provider;
		private readonly DataAccess _accessor;
		#endregion

		#region 构造函数
		public SelectStatementBuilderTest()
		{
			_provider = Utility.GetProvider(APPLICATION_NAME);
			_accessor = new DataAccess(APPLICATION_NAME);
		}
		#endregion

		#region 测试方法
		[Fact]
		public void Test()
		{
			const string NAME = "Security.User";

			var schema = _accessor.Schema.Parse(NAME, "*, Creator{UserId, Name, FullName}", typeof(Zongsoft.Security.Membership.User));

			var context = new DataSelectContext(_accessor,
				NAME, //name
				schema.EntityType, //entityType
				null, //grouping
				Condition.Equal("UserId", 100) | (Condition.Like("Modifier.Name", "Popeye*") & Condition.GreaterThan("Status", 2)),
				schema, //schema
				null, //paging
				Sorting.Descending("UserId") + Sorting.Ascending("Creator.Name"));

			var statements = context.Build();
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var command = context.Build(statements.First());
			Assert.NotNull(command);
			Assert.NotNull(command.CommandText);
			Assert.True(command.CommandText.Length > 0);
			Assert.True(command.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(command.CommandText);
		}

		[Fact]
		public void TestGrouping()
		{
			const string NAME = "Security.User";

			var grouping = Grouping.Group("Grade");
			grouping.Aggregates.Sum("Points").Count("*");

			var schema = _accessor.Schema.Parse(NAME, "Password, Creator{*, Modifier{*}}", typeof(Zongsoft.Security.Membership.User));

			var context = new DataSelectContext(_accessor,
				NAME, //name
				schema.EntityType, //entityType
				grouping, //grouping
				Condition.Equal("UserId", 100) | (Condition.Like("Modifier.Name", "Popeye*") & Condition.GreaterThan("Grade", 2)),
				schema, //schema
				null, //paging
				Sorting.Descending("UserId") + Sorting.Ascending("Creator.Name"));

			var statements = context.Build();
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var command = context.Build(statements.First());
			Assert.NotNull(command);
			Assert.NotNull(command.CommandText);
			Assert.True(command.CommandText.Length > 0);
			Assert.True(command.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(command.CommandText);
		}

		[Fact]
		public void TestCollectionProperties()
		{
			const string NAME = "Security.Role";

			var schema = _accessor.Schema.Parse(NAME, "*, Creator{*, Modifier{*}}, Users(~CreatedTime){*}", typeof(Models.RoleModel));

			var context = new DataSelectContext(_accessor,
				NAME, //name
				schema.EntityType, //entityType
				null, //grouping
				Condition.Between("RoleId", 10, 100) | Condition.Like("Modifier.Name", "Popeye*"),
				schema, //schema
				null, //paging
				Sorting.Descending("RoleId") + Sorting.Ascending("Creator.Name"));

			var statements = context.Build();
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var command = context.Build(statements.First());
			Assert.NotNull(command);
			Assert.NotNull(command.CommandText);
			Assert.True(command.CommandText.Length > 0);
			Assert.True(command.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(command.CommandText);
		}
		#endregion
	}
}
