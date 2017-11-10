using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

using MySql.Data;
using MySql.Data.MySqlClient;

using Zongsoft.Data.Benchmark.Models;

namespace Zongsoft.Data.Benchmark
{
	public static class DataPopulatorTest
	{
		private const string DIVIDING = "-------------------------------------------------------";

		public static DbDataReader GetReader(string tableName, int count)
		{
			var connection = CreateConnection();
			var command = connection.CreateCommand();
			command.CommandText = $"SELECT * FROM `{tableName}` LIMIT " + count.ToString();
			command.CommandType = CommandType.Text;

			connection.Open();
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}

		private static DbConnection CreateConnection()
		{
			const string ConnectionString = @"server=119.23.160.108;user id=develop;Password=Wayto2017;database=wayto-saas;persist security info=False;Charset=utf8;Convert zero Datetime=true;Allow zero Datetime=True;";
			//const string ConnectionString = @"server=127.0.0.1;user id=develop;Password=Wayto2017;database=wayto-saas;persist security info=False;Charset=utf8;Convert zero Datetime=true;Allow zero Datetime=True;";

			return new MySqlConnection(ConnectionString);
		}

		private static void SetAsset(Models.Asset entity, IDataRecord record)
		{
			if(!record.IsDBNull(record.GetOrdinal("ActiveTime")))
				entity.ActiveTime = record.GetDateTime(record.GetOrdinal("ActiveTime"));

			if(!record.IsDBNull(record.GetOrdinal("AddressDetail")))
				entity.AddressDetail = (string)record["AddressDetail"];

			if(!record.IsDBNull(record.GetOrdinal("AddressId")))
				entity.AddressId = (uint?)record["AddressId"];

			if(!record.IsDBNull(record.GetOrdinal("Altitude")))
				entity.Altitude = (short)record["Altitude"];

			if(!record.IsDBNull(record.GetOrdinal("Appearance")))
				entity.Appearance = (byte)record["Appearance"];

			if(!record.IsDBNull(record.GetOrdinal("AssetClassId")))
				entity.AssetClassId = (uint?)record["AssetClassId"];

			if(!record.IsDBNull(record.GetOrdinal("AssetId")))
				entity.AssetId = (ulong)record["AssetId"];

			if(!record.IsDBNull(record.GetOrdinal("AssetNo")))
				entity.AssetNo = (string)record["AssetNo"];

			if(!record.IsDBNull(record.GetOrdinal("AssetTypeId")))
				entity.AssetTypeId = (uint?)record["AssetTypeId"];

			if(!record.IsDBNull(record.GetOrdinal("AttachmentMark")))
				entity.AttachmentMark = (string)record["AttachmentMark"];

			if(!record.IsDBNull(record.GetOrdinal("Barcode")))
				entity.Barcode = (string)record["Barcode"];

			if(!record.IsDBNull(record.GetOrdinal("BranchId")))
				entity.BranchId = (ushort)record["BranchId"];

			if(!record.IsDBNull(record.GetOrdinal("CorporationId")))
				entity.CorporationId = (uint)record["CorporationId"];

			if(!record.IsDBNull(record.GetOrdinal("CreatedTime")))
				entity.CreatedTime = record.GetDateTime(record.GetOrdinal("CreatedTime"));

			if(!record.IsDBNull(record.GetOrdinal("CreatorId")))
				entity.CreatorId = (uint)record["CreatorId"];

			if(!record.IsDBNull(record.GetOrdinal("DepartmentId")))
				entity.DepartmentId = (ushort?)record["DepartmentId"];

			if(!record.IsDBNull(record.GetOrdinal("ExaminationId")))
				entity.ExaminationId = (ulong?)record["ExaminationId"];

			if(!record.IsDBNull(record.GetOrdinal("ExaminationResultId")))
				entity.ExaminationResultId = (ulong?)record["ExaminationResultId"];

			if(!record.IsDBNull(record.GetOrdinal("Flags")))
				entity.Flags = (AssetFlags)record["Flags"];

			if(!record.IsDBNull(record.GetOrdinal("FlagsTimestamp")))
				entity.FlagsTimestamp = record.GetDateTime(record.GetOrdinal("FlagsTimestamp"));

			if(!record.IsDBNull(record.GetOrdinal("Grade")))
				entity.Grade = (byte)record["Grade"];

			if(!record.IsDBNull(record.GetOrdinal("GradeDescription")))
				entity.GradeDescription = (string)record["GradeDescription"];

			if(!record.IsDBNull(record.GetOrdinal("Kind")))
				entity.Kind = (byte)record["Kind"];

			if(!record.IsDBNull(record.GetOrdinal("Latitude")))
				entity.Latitude = (decimal)record["Latitude"];

			if(!record.IsDBNull(record.GetOrdinal("Longitude")))
				entity.Longitude = (decimal)record["Longitude"];

			if(!record.IsDBNull(record.GetOrdinal("ManufacturedDate")))
				entity.ManufacturedDate = record.GetDateTime(record.GetOrdinal("ManufacturedDate"));

			if(!record.IsDBNull(record.GetOrdinal("ManufacturerId")))
				entity.ManufacturerId = (uint?)record["ManufacturerId"];

			if(!record.IsDBNull(record.GetOrdinal("MeasuredDescription")))
				entity.MeasuredDescription = (string)record["MeasuredDescription"];

			if(!record.IsDBNull(record.GetOrdinal("MeasuredValue")))
				entity.MeasuredValue = (decimal)record["MeasuredValue"];

			if(!record.IsDBNull(record.GetOrdinal("Name")))
				entity.Name = (string)record["Name"];

			if(!record.IsDBNull(record.GetOrdinal("NameEx")))
				entity.NameEx = (string)record["NameEx"];

			if(!record.IsDBNull(record.GetOrdinal("OperatingVendorId")))
				entity.OperatingVendorId = (uint?)record["OperatingVendorId"];

			if(!record.IsDBNull(record.GetOrdinal("OperatingVendorName")))
				entity.OperatingVendorName = (string)record["OperatingVendorName"];

			if(!record.IsDBNull(record.GetOrdinal("OperatingVendorPhoneNumber")))
				entity.OperatingVendorPhoneNumber = (string)record["OperatingVendorPhoneNumber"];

			if(!record.IsDBNull(record.GetOrdinal("ParentId")))
				entity.ParentId = (ulong?)record["ParentId"];

			if(!record.IsDBNull(record.GetOrdinal("PinYin")))
				entity.PinYin = (string)record["PinYin"];

			if(!record.IsDBNull(record.GetOrdinal("PlaceId")))
				entity.PlaceId = (uint?)record["PlaceId"];

			if(!record.IsDBNull(record.GetOrdinal("PlatedTime")))
				entity.PlatedTime = record.GetDateTime(record.GetOrdinal("PlatedTime"));

			if(!record.IsDBNull(record.GetOrdinal("PlateNo")))
				entity.PlateNo = (string)record["PlateNo"];

			if(!record.IsDBNull(record.GetOrdinal("PointId")))
				entity.PointId = (ulong)record["PointId"];

			if(!record.IsDBNull(record.GetOrdinal("Remark")))
				entity.Remark = (string)record["Remark"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedAmount1")))
				entity.ReservedAmount1 = (decimal)record["ReservedAmount1"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedAmount2")))
				entity.ReservedAmount2 = (decimal)record["ReservedAmount2"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedAmount3")))
				entity.ReservedAmount3 = (decimal)record["ReservedAmount3"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedAmount4")))
				entity.ReservedAmount4 = (decimal)record["ReservedAmount4"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedCount1")))
				entity.ReservedCount1 = (int)record["ReservedCount1"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedCount2")))
				entity.ReservedCount2 = (int)record["ReservedCount2"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedCount3")))
				entity.ReservedCount3 = (int)record["ReservedCount3"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedCount4")))
				entity.ReservedCount4 = (int)record["ReservedCount4"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedData")))
				entity.ReservedData = (string)record["ReservedData"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedText1")))
				entity.ReservedText1 = (string)record["ReservedText1"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedText2")))
				entity.ReservedText2 = (string)record["ReservedText2"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedText3")))
				entity.ReservedText3 = (string)record["ReservedText3"];

			if(!record.IsDBNull(record.GetOrdinal("ReservedText4")))
				entity.ReservedText4 = (string)record["ReservedText4"];

			if(!record.IsDBNull(record.GetOrdinal("ResponsibleId")))
				entity.ResponsibleId = (uint?)record["ResponsibleId"];

			if(!record.IsDBNull(record.GetOrdinal("ResponsibleName")))
				entity.ResponsibleName = (string)record["ResponsibleName"];

			if(!record.IsDBNull(record.GetOrdinal("ResponsiblePhoneNumber")))
				entity.ResponsiblePhoneNumber = (string)record["ResponsiblePhoneNumber"];

			if(!record.IsDBNull(record.GetOrdinal("Schema")))
				entity.Schema = (string)record["Schema"];

			if(!record.IsDBNull(record.GetOrdinal("Score")))
				entity.Score = (byte)record["Score"];

			if(!record.IsDBNull(record.GetOrdinal("ScoringTimestamp")))
				entity.ScoringTimestamp = record.GetDateTime(record.GetOrdinal("ScoringTimestamp"));

			if(!record.IsDBNull(record.GetOrdinal("Size")))
				entity.Size = (decimal)record["Size"];

			if(!record.IsDBNull(record.GetOrdinal("Source")))
				entity.Source = (AssetSource)record["Source"];

			if(!record.IsDBNull(record.GetOrdinal("Spec")))
				entity.Spec = (string)record["Spec"];

			if(!record.IsDBNull(record.GetOrdinal("Status")))
				entity.Status = (AssetStatus)record["Status"];

			if(!record.IsDBNull(record.GetOrdinal("StatusDescription")))
				entity.StatusDescription = (string)record["StatusDescription"];

			if(!record.IsDBNull(record.GetOrdinal("StatusTimestamp")))
				entity.StatusTimestamp = record.GetDateTime(record.GetOrdinal("StatusTimestamp"));

			if(!record.IsDBNull(record.GetOrdinal("SummaryPath")))
				entity.SummaryPath = (string)record["SummaryPath"];

			if(!record.IsDBNull(record.GetOrdinal("Tags")))
				entity.Tags = (string)record["Tags"];

			if(!record.IsDBNull(record.GetOrdinal("Village")))
				entity.Village = (string)record["Village"];

			if(!record.IsDBNull(record.GetOrdinal("Visible")))
				entity.Visible = (bool)record["Visible"];
		}

		public static void Test(int count = 1000, int round = 5)
		{
			//预热动态组装
			TestReadAssetsWithEmitting(1, false);

			for(int i = 0; i < round; i++)
			{
				Console.WriteLine($">>>>>>>>>> 第{i + 1}轮 <<<<<<<<<<");
				Console.WriteLine();

				TestReadAssetsWithEmitting(count, true);
				Console.WriteLine(DIVIDING);

				TestReadAssetsWithManually(count, true);
				Console.WriteLine();
			}

			Console.WriteLine();
		}

		public static void TestReadAssetsWithEmitting(int count, bool outputEnabled = false)
		{
			var populator = DataPopulator.Build(typeof(Models.Asset));

			Stopwatch stopwach = null;

			if(outputEnabled)
			{
				stopwach = new Stopwatch();
				stopwach.Restart();
			}

			using(var reader = GetReader("Asset", count))
			{
				var entities = populator.Populate(reader);

				foreach(var entity in entities)
				{
					//if(outputEnabled)
					//	Console.WriteLine(entity);
				}
			}

			if(outputEnabled)
			{
				stopwach.Stop();

				Console.WriteLine($"动态组装 {count} 条数据实体耗时 {stopwach.ElapsedMilliseconds} 毫秒。");
			}
		}

		public static async void TestReadAssetsWithManually(int count, bool outputEnabled = false)
		{
			Stopwatch stopwach = null;

			if(outputEnabled)
			{
				stopwach = new Stopwatch();
				stopwach.Restart();
			}

			using(var reader = GetReader("Asset", count))
			{
				while(await reader.ReadAsync())
				{
					var entity = new Models.Asset();

					SetAsset(entity, reader);

					//for(var i = 0; i < reader.FieldCount; i++)
					//{
					//	var name = reader.GetName(i);

					//	switch(name)
					//	{
					//		case "AssetId":
					//			entity.AssetId = (ulong)reader[i];
					//			break;
					//		case "AssetNo":
					//			entity.AssetNo = (string)reader[i];
					//			break;
					//		case "Name":
					//			entity.Name = (string)reader[i];
					//			break;
					//		case "Status":
					//			entity.Status = (AssetStatus)reader[i];
					//			break;
					//	}
					//}
				}
			}

			if(outputEnabled)
			{
				stopwach.Stop();

				Console.WriteLine($"手动组装 {count} 条数据实体耗时 {stopwach.ElapsedMilliseconds} 毫秒。");
			}
		}
	}
}
