using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Data.Benchmark.Models;

namespace Zongsoft.Data.Benchmark.Tests
{
	public static class CreateObjectTest
	{
		private const string DIVIDING = "-------------------------------------------------------";

		public static void Test(int count)
		{
			Console.WriteLine();

			var stopwach = new Stopwatch();

			stopwach.Restart();
			var result1 = CreateAssets(count);
			var length1 = result1.Count();
			stopwach.Stop();

			Console.WriteLine($"创建 {length1} 个 '{typeof(Asset).Name}' 对象，\t\t耗时 {stopwach.ElapsedMilliseconds} 毫秒。");
			Console.WriteLine(DIVIDING);

			stopwach.Restart();
			var result2 = CreateAssetModels(count);
			var length2 = result2.Count();
			stopwach.Stop();

			Console.WriteLine($"创建 {length2} 个 '{typeof(AssetModel).Name}' 对象，\t耗时 {stopwach.ElapsedMilliseconds} 毫秒。");
			Console.WriteLine(DIVIDING);
			Console.WriteLine();
		}

		private static IEnumerable<Asset> CreateAssets(int count)
		{
			for(int i = 0; i < count; i++)
			{
				yield return new Asset
				{
					AssetId = (ulong)i,
					AssetNo = "A" + i.ToString(),
					Barcode = i.ToString(),
					PlateNo = i.ToString(),
					Name = "资产设施 " + i.ToString(),
					PinYin = "ZCSS-ZiChanSheShi " + i.ToString(),
					NameEx = null,
					Spec = "规格型号 [" + i.ToString() + "]",
					ActiveTime = DateTime.Now,
					AddressId = 123456,
					AddressDetail = "深圳水福田区梅林路·斯达大厦607",
					Appearance = 0,
					AssetClassId = 100,
					AssetTypeId = 200,
					AttachmentMark = "1,2,3,4,5",
					CorporationId = 1,
					BranchId = 101,
					Schema = "Hydrants",
					Altitude = 1000,
					Longitude = 123.12345678m,
					Latitude = 123.87654321m,
					DepartmentId = 10,
					ExaminationId = null,
					ExaminationResultId = null,
					Flags = AssetFlags.None,
					FlagsTimestamp = DateTime.Now,
					Grade = 0,
					GradeDescription = "尚未评级",
					Kind = 0,
					ManufacturerId = 100,
					ManufacturedDate = null,
					MeasuredValue = 100.123m,
					MeasuredDescription = string.Empty,
					OperatingVendorId = 0,
					OperatingVendorName = null,
					OperatingVendorPhoneNumber = null,
					Parent = null,
					ParentId = null,
					PersonId = 100,
					PlaceId = 200,
					PlatedTime = new DateTime(2001, 1, 1),
					PointId = (ulong)i,
					ReservedData = null,
					ReservedAmount1 = 0m,
					ReservedAmount2 = 0m,
					ReservedAmount3 = 0m,
					ReservedAmount4 = 0m,
					ReservedCount1 = 0,
					ReservedCount2 = 0,
					ReservedCount3 = 0,
					ReservedCount4 = 0,
					ReservedText1 = null,
					ReservedText2 = null,
					ReservedText3 = null,
					ReservedText4 = null,
					ResponsibleId = 100,
					ResponsibleName = "钟大大",
					ResponsiblePhoneNumber = "13012345678",
					Score = 99,
					ScoringTimestamp = DateTime.Now,
					Size = 100.99m,
					Source = AssetSource.Manually,
					Status = AssetStatus.Normal,
					StatusTimestamp = DateTime.Now,
					StatusDescription = "正常",
					Summary = null,
					SummaryPath = "zfs.oss:/wayto-files/attachments/201709/xxx.txt",
					Tags = "标签x, 标签y, 标签z",
					Village = "上梅林",
					Visible = true,
					CreatorId = 100,
					CreatedTime = DateTime.Now,
					Remark = "这是一个备注信息。"
				};
			}
		}

		private static IEnumerable<AssetModel> CreateAssetModels(int count)
		{
			for(int i = 0; i < count; i++)
			{
				yield return new AssetModel
				{
					AssetId = (ulong)i,
					AssetNo = "A" + i.ToString(),
					Barcode = i.ToString(),
					PlateNo = i.ToString(),
					Name = "资产设施 " + i.ToString(),
					PinYin = "ZCSS-ZiChanSheShi " + i.ToString(),
					NameEx = null,
					Spec = "规格型号 [" + i.ToString() + "]",
					ActiveTime = DateTime.Now,
					AddressId = 123456,
					AddressDetail = "深圳水福田区梅林路·斯达大厦607",
					Appearance = 0,
					AssetClassId = 100,
					AssetTypeId = 200,
					AttachmentMark = "1,2,3,4,5",
					CorporationId = 1,
					BranchId = 101,
					Schema = "Hydrants",
					Altitude = 1000,
					Longitude = 123.12345678m,
					Latitude = 123.87654321m,
					DepartmentId = 10,
					ExaminationId = null,
					ExaminationResultId = null,
					Flags = AssetFlags.None,
					FlagsTimestamp = DateTime.Now,
					Grade = 0,
					GradeDescription = "尚未评级",
					Kind = 0,
					ManufacturerId = 100,
					ManufacturedDate = null,
					MeasuredValue = 100.123m,
					MeasuredDescription = string.Empty,
					OperatingVendorId = 0,
					OperatingVendorName = null,
					OperatingVendorPhoneNumber = null,
					Parent = null,
					ParentId = null,
					PersonId = 100,
					PlaceId = 200,
					PlatedTime = new DateTime(2001, 1, 1),
					PointId = (ulong)i,
					ReservedData = null,
					ReservedAmount1 = 0m,
					ReservedAmount2 = 0m,
					ReservedAmount3 = 0m,
					ReservedAmount4 = 0m,
					ReservedCount1 = 0,
					ReservedCount2 = 0,
					ReservedCount3 = 0,
					ReservedCount4 = 0,
					ReservedText1 = null,
					ReservedText2 = null,
					ReservedText3 = null,
					ReservedText4 = null,
					ResponsibleId = 100,
					ResponsibleName = "钟大大",
					ResponsiblePhoneNumber = "13012345678",
					Score = 99,
					ScoringTimestamp = DateTime.Now,
					Size = 100.99m,
					Source = AssetSource.Manually,
					Status = AssetStatus.Normal,
					StatusTimestamp = DateTime.Now,
					StatusDescription = "正常",
					Summary = null,
					SummaryPath = "zfs.oss:/wayto-files/attachments/201709/xxx.txt",
					Tags = "标签x, 标签y, 标签z",
					Village = "上梅林",
					Visible = true,
					CreatorId = 100,
					CreatedTime = DateTime.Now,
					Remark = "这是一个备注信息。"
				};
			}
		}
	}
}
