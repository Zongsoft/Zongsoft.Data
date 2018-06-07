using System;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class SelectStatementBuilderTest
	{
		#region 构造函数
		public SelectStatementBuilderTest()
		{
			DataEnvironment.Metadata = new Metadata.Profiles.MetadataFileManager(@"/Zongsoft/Zongsoft.Data/src/");
		}
		#endregion

		[Fact]
		public void Test()
		{
			var context = new DataSelectionContext(new DataAccess(),
				"Security.UserProfile", //name
				typeof(Zongsoft.Security.Membership.User), //entityType
				null, //grouping
				Condition.Equal("UserId", 100) | (Condition.Like("Modifier.Name", "Popeye*") & Condition.GreaterThan("Grade", 2)),
				"Password, Creator.Modifier", //scope
				null, //paging
				Sorting.Descending("UserId") + Sorting.Ascending("Creator.Name"));

			var provider = new Zongsoft.Data.Dummy.DummyProvider();
			var statement = provider.Builder.Build(context);
			Assert.NotNull(statement);

			var script = provider.Scriptor.Script(statement);

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

			var context = new DataSelectionContext(new DataAccess(),
				"Security.UserProfile", //name
				typeof(Zongsoft.Security.Membership.User), //entityType
				grouping, //grouping
				Condition.Equal("UserId", 100) | (Condition.Like("Modifier.Name", "Popeye*") & Condition.GreaterThan("Grade", 2)),
				"Password, Creator.Modifier", //scope
				null, //paging
				Sorting.Descending("UserId") + Sorting.Ascending("Creator.Name"));

			var provider = new Zongsoft.Data.Dummy.DummyProvider();
			var statement = provider.Builder.Build(context);
			Assert.NotNull(statement);

			var script = provider.Scriptor.Script(statement);

			Assert.NotNull(script);
			Assert.NotNull(script.Text);
			Assert.NotNull(script.Parameters);
			Assert.True(script.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(script.Text);
		}

		[Fact]
		public void TestCollectionProperties()
		{
			var context = new DataSelectionContext(new DataAccess(),
				"Security.Role", //name
				typeof(RoleModel), //entityType
				null, //grouping
				Condition.Between("RoleId", 1, 100) | Condition.Like("Modifier.Name", "Popeye*"),
				"Creator.Modifier, Users.Name", //scope
				null, //paging
				Sorting.Descending("RoleId") + Sorting.Ascending("Creator.Name"));

			var provider = new Zongsoft.Data.Dummy.DummyProvider();
			var statement = provider.Builder.Build(context);
			Assert.NotNull(statement);

			var script = provider.Scriptor.Script(statement);

			Assert.NotNull(script);
			Assert.NotNull(script.Text);
			Assert.NotNull(script.Parameters);
			Assert.True(script.Parameters.Count > 0);

			System.Diagnostics.Debug.WriteLine(script.Text);
		}
	}
}
