using System;
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
		#endregion

		#region 构造函数
		public SelectStatementBuilderTest()
		{
			DataEnvironment.Drivers.Add(new Dummy.DummyDriver());

			_provider = DataEnvironment.Providers.GetProvider(APPLICATION_NAME);
			_provider.Connector = Dummy.DummyConnector.Instance;

			if(_provider.Metadata.Loader is Metadata.Profiles.MetadataFileLoader loader)
				loader.Path = @"/Zongsoft/Zongsoft.Community/src/|/Zongsoft/Zongsoft.Security/src/";
		}
		#endregion

		[Fact]
		public void Test()
		{
			var context = new DataSelectContext(new DataAccess(APPLICATION_NAME),
				"Security.UserProfile", //name
				typeof(Zongsoft.Security.Membership.User), //entityType
				null, //grouping
				Condition.Equal("UserId", 100) | (Condition.Like("Modifier.Name", "Popeye*") & Condition.GreaterThan("Grade", 2)),
				"Password, Creator.Modifier", //scope
				null, //paging
				Sorting.Descending("UserId") + Sorting.Ascending("Creator.Name"));

			var source = _provider.Connector.GetSource(context);
			Assert.NotNull(source);

			var statement = source.Driver.Builder.Build(context, source);
			Assert.NotNull(statement);

			var script = source.Driver.Scriptor.Script(statement);
			Assert.NotNull(script);
			Assert.NotNull(script.Text);
			Assert.NotNull(script.Parameters);
			Assert.True(script.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(script.Text);
		}

		[Fact]
		public void TestGrouping()
		{
			var grouping = Grouping.Group("Grade");
			grouping.Aggregates.Sum("Points").Count("*");

			var context = new DataSelectContext(new DataAccess(APPLICATION_NAME),
				"Security.UserProfile", //name
				typeof(Zongsoft.Security.Membership.User), //entityType
				grouping, //grouping
				Condition.Equal("UserId", 100) | (Condition.Like("Modifier.Name", "Popeye*") & Condition.GreaterThan("Grade", 2)),
				"Password, Creator.Modifier", //scope
				null, //paging
				Sorting.Descending("UserId") + Sorting.Ascending("Creator.Name"));

			var source = _provider.Connector.GetSource(context);
			Assert.NotNull(source);

			var statement = source.Driver.Builder.Build(context, source);
			Assert.NotNull(statement);

			var script = source.Driver.Scriptor.Script(statement);
			Assert.NotNull(script);
			Assert.NotNull(script.Text);
			Assert.NotNull(script.Parameters);
			Assert.True(script.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(script.Text);
		}

		[Fact]
		public void TestCollectionProperties()
		{
			var context = new DataSelectContext(new DataAccess(APPLICATION_NAME),
				"Security.Role", //name
				typeof(RoleModel), //entityType
				null, //grouping
				Condition.Between("RoleId", 10, 100) | Condition.Like("Modifier.Name", "Popeye*"),
				"Creator.Modifier, Users.Name", //scope
				null, //paging
				Sorting.Descending("RoleId") + Sorting.Ascending("Creator.Name"));

			var source = _provider.Connector.GetSource(context);
			Assert.NotNull(source);

			var statement = source.Driver.Builder.Build(context, source);
			Assert.NotNull(statement);

			var script = source.Driver.Scriptor.Script(statement);
			Assert.NotNull(script);
			Assert.NotNull(script.Text);
			Assert.NotNull(script.Parameters);
			Assert.True(script.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(script.Text);
		}
	}
}
