using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			DataEnvironment.Metadata = new Metadata.Profiles.MetadataFileManager(@"/temp/");
		}
		#endregion

		[Fact]
		public void Test()
		{
			var grouping = Grouping.Group("Grade");
			grouping.Aggregates.Sum("Points").Count("*");

			var context = new DataSelectionContext(new DataAccess(),
				"Security.UserProfile", // name
				typeof(Zongsoft.Security.Membership.User), //entityType
				grouping, //grouping
				Condition.Equal("UserId", 100) | (Condition.Like("Modifier.Name", "Popeye*") & Condition.GreaterThan("Grade", 2)),
				"Password, Creator.Modifier", //scope
				null, //paging
				Sorting.Descending("UserId") + Sorting.Ascending("Creator.Name"));

			var builder = new SelectStatementBuilder();
			var statement = builder.Build(context);
			Assert.NotNull(statement);

			var writer = new ExpressionWriter();
			var scriptor = new SelectStatementScriptor(writer);
			var text = new StringBuilder();

			scriptor.Generate(text, statement);

			System.Diagnostics.Debug.WriteLine(text.ToString());

			Assert.True(text.Length > 0);
		}

		[Fact]
		public void TestCollectionProperties()
		{
			var context = new DataSelectionContext(new DataAccess(),
				"Security.Role", // name
				typeof(RoleModel), //entityType
				null, //grouping
				Condition.Equal("RoleId", 100) | Condition.Like("Modifier.Name", "Popeye*"),
				"Users, Creator.Modifier", //scope
				null, //paging
				Sorting.Descending("RoleId") + Sorting.Ascending("Creator.Name"));

			var builder = new SelectStatementBuilder();
			var statement = builder.Build(context);
			Assert.NotNull(statement);

			var writer = new ExpressionWriter();
			var scriptor = new SelectStatementScriptor(writer);
			var text = new StringBuilder();

			scriptor.Generate(text, statement);

			System.Diagnostics.Debug.WriteLine(text.ToString());

			Assert.True(text.Length > 0);
		}
	}
}
