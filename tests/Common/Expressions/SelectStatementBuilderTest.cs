using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Zongsoft.Data.Common.Expressions.Tests
{
	public class SelectStatementBuilderTest
	{
		public SelectStatementBuilderTest()
		{
			DataEnvironment.Metadata = new Metadata.Profiles.MetadataFileManager(@"/temp/");
		}

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

			var statements = builder.Build(context);

			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var statement = statements.FirstOrDefault();
			Assert.NotNull(statement);

			var writer = new ExpressionWriter();
			var scriptor = new SelectStatementScriptor(writer);
			var text = new StringBuilder();

			scriptor.Generate(text, statements);

			System.Diagnostics.Debug.WriteLine(text.ToString());

			Assert.True(text.Length > 0);
		}
	}
}
