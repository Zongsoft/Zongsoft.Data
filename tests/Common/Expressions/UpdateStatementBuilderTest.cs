using System;
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Tests.Models;

using Xunit;

namespace Zongsoft.Data.Tests
{
	public class UpdateStatementBuilderTest
	{
		#region 常量定义
		private const string APPLICATION_NAME = "Community";
		#endregion

		#region 成员变量
		private readonly IDataProvider _provider;
		#endregion

		#region 构造函数
		public UpdateStatementBuilderTest()
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
			var schema = accessor.Schema.Parse(NAME, "*, Principal{*, Department{*, Managers{*}}}", typeof(Asset));

			var context = new DataUpdateContext(accessor,
				NAME, //name
				false, //isMultiple
				GetAsset(1001), //data
				Condition.Equal("AssetId", 1001), //condition
				schema //schema
				);

			var statements = context.Build().ToArray();
			Assert.NotNull(statements);
			Assert.NotEmpty(statements);

			var command = context.Build(statements[0]);
			Assert.NotNull(command);
			Assert.NotNull(command.CommandText);
			Assert.True(command.CommandText.Length > 0);

			foreach(var statement in statements)
			{
				if(statement.HasSlaves)
				{
					foreach(var slave in statement.Slaves)
					{
						var cmd = context.Build(slave);
					}
				}
			}

			System.Diagnostics.Debug.WriteLine(command.CommandText);
		}
		#endregion

		#region 私有方法
		private Asset GetAsset(uint assetId = 1, string name = null)
		{
			return new Asset
			{
				AssetId = assetId,
				Name = name ?? "测试设施",
				Namespace = "Zongsoft",
				Floor = 99,
				CreatorId = 100,
				CreatedTime = DateTime.Now,

				Principal = new UserProfile()
				{
					UserId = 100,
					Gender = Gender.Male,
					Nickname = "钟少(Popeye Zhong)",

					Department = new Department()
					{
						CorporationId = 1,
						DepartmentId = 10,
						Name = "软件研发部(Software Development)",
						AddressId = 0x01020304,
						AddressDetail = "广东省深圳市",

						Managers = new[]
						{
							new UserProfile
							{
								UserId = 901,
								Nickname = "老大",
							},
							new UserProfile
							{
								UserId = 902,
								Nickname = "小跟班",
							}
						}
					},
				},
			};
		}
		#endregion
	}
}
