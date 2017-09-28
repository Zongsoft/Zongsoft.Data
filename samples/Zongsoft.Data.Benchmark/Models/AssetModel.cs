using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Benchmark.Models
{
	/// <summary>
	/// 表示资产设施的业务实体类。
	/// </summary>
	public class AssetModel : ModelBase
	{
		#region 成员字段
		private ulong _assetId;
		private string _assetNo;
		private string _barcode;
		private string _plateNo;
		private string _name;
		private string _nameEx;
		private string _pinYin;
		private string _spec;
		private string _tags;
		private decimal _size;
		private ulong? _parentId;
		private AssetModel _parent;
		private ulong _pointId;
		private uint? _assetTypeId;
		private uint? _assetClassId;
		private uint? _manufacturerId;
		private DateTime? _manufacturedDate;
		private uint _corporationId;
		private ushort _branchId;
		private ushort? _departmentId;
		private string _schema;
		private byte _kind;
		private byte _grade;
		private string _gradeDescription;
		private byte _score;
		private DateTime? _scoringTimestamp;
		private AssetSource _source;
		private byte _appearance;
		private AssetFlags _flags;
		private DateTime? _flagsTimestamp;
		private AssetStatus _status;
		private DateTime? _statusTimestamp;
		private string _statusDescription;
		private decimal _measuredValue;
		private string _measuredDescription;
		private ulong? _examinationId;
		private ulong? _examinationResultId;
		private uint? _personId;
		private uint? _placeId;
		private uint? _addressId;
		private string _addressDetail;
		private string _village;
		private short _altitude;
		private decimal _longitude;
		private decimal _latitude;
		private bool _visible;
		private DateTime? _activeTime;
		private DateTime? _platedTime;
		private string _summary;
		private string _summaryPath;
		private string _attachmentMark;
		private uint? _responsibleId;
		private string _responsibleName;
		private string _responsiblePhoneNumber;
		private uint? _operatingVendorId;
		private string _operatingVendorName;
		private string _operatingVendorPhoneNumber;
		private string _reservedData;
		private string _reservedText1;
		private string _reservedText2;
		private string _reservedText3;
		private string _reservedText4;
		private int _reservedCount1;
		private int _reservedCount2;
		private int _reservedCount3;
		private int _reservedCount4;
		private decimal _reservedAmount1;
		private decimal _reservedAmount2;
		private decimal _reservedAmount3;
		private decimal _reservedAmount4;
		private uint _creatorId;
		private DateTime _createdTime;
		private string _remark;

		private IEnumerable<AssetModel> _children;
		#endregion

		#region 构造函数
		public AssetModel()
		{
			this.Visible = true;
			this.CreatedTime = DateTime.Now;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置资产设施编号。
		/// </summary>
		public ulong AssetId
		{
			get
			{
				return _assetId;
			}
			set
			{
				this.SetPropertyValue(nameof(AssetId), ref _assetId, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施代号。
		/// </summary>
		public string AssetNo
		{
			get
			{
				return _assetNo;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				this.SetPropertyValue(nameof(AssetNo), ref _assetNo, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施条形码。
		/// </summary>
		public string Barcode
		{
			get
			{
				return _barcode;
			}
			set
			{
				this.SetPropertyValue(nameof(Barcode), ref _barcode, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施牌号。
		/// </summary>
		public string PlateNo
		{
			get
			{
				return _plateNo;
			}
			set
			{
				this.SetPropertyValue(nameof(PlateNo), ref _plateNo, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				this.SetPropertyValue(nameof(Name), ref _name, value);
			}
		}

		/// <summary>
		/// 获取或设置名称的拼音。
		/// </summary>
		public string PinYin
		{
			get
			{
				return _pinYin;
			}
			set
			{
				this.SetPropertyValue(nameof(PinYin), ref _pinYin, value);
			}
		}

		/// <summary>
		/// 获取或设置扩展名称。
		/// </summary>
		public string NameEx
		{
			get
			{
				return _nameEx;
			}
			set
			{
				this.SetPropertyValue(nameof(NameEx), ref _nameEx, value);
			}
		}

		/// <summary>
		/// 获取或设置规格型号。
		/// </summary>
		public string Spec
		{
			get
			{
				return _spec;
			}
			set
			{
				this.SetPropertyValue(nameof(Spec), ref _spec, value);
			}
		}

		/// <summary>
		/// 获取或设置标签名集。
		/// </summary>
		public string Tags
		{
			get
			{
				return _tags;
			}
			set
			{
				this.SetPropertyValue(nameof(Tags), ref _tags, value);
			}
		}

		/// <summary>
		/// 获取或设置尺寸大小。
		/// </summary>
		public decimal Size
		{
			get
			{
				return _size;
			}
			set
			{
				this.SetPropertyValue(nameof(Size), ref _size, value);
			}
		}

		/// <summary>
		/// 获取或设置父级资产设施编号。
		/// </summary>
		public ulong? ParentId
		{
			get
			{
				return _parentId;
			}
			set
			{
				this.SetPropertyValue(nameof(ParentId), ref _parentId, value);
			}
		}

		/// <summary>
		/// 获取或设置父级资产设施对象。
		/// </summary>
		public AssetModel Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				this.SetPropertyValue(nameof(Parent), ref _parent, value);
			}
		}

		/// <summary>
		/// 获取或设置设施对应的点位编号。
		/// </summary>
		public ulong PointId
		{
			get
			{
				return _pointId;
			}
			set
			{
				this.SetPropertyValue(nameof(PointId), ref _pointId, value);
			}
		}

		/// <summary>
		/// 获取或设置默认评测问卷编号。
		/// </summary>
		public ulong? ExaminationId
		{
			get
			{
				return _examinationId;
			}
			set
			{
				this.SetPropertyValue(nameof(ExaminationId), ref _examinationId, value);
			}
		}

		/// <summary>
		/// 获取或设置测量数值，默认为零。
		/// </summary>
		public decimal MeasuredValue
		{
			get
			{
				return _measuredValue;
			}
			set
			{
				this.SetPropertyValue(nameof(MeasuredValue), ref _measuredValue, value);
			}
		}

		/// <summary>
		/// 测量描述。
		/// </summary>
		public string MeasuredDescription
		{
			get
			{
				return _measuredDescription;
			}
			set
			{
				this.SetPropertyValue(nameof(MeasuredDescription), ref _measuredDescription, value);
			}
		}

		/// <summary>
		/// 获取或设置最新测评答卷编号。
		/// </summary>
		public ulong? ExaminationResultId
		{
			get
			{
				return _examinationResultId;
			}
			set
			{
				this.SetPropertyValue(nameof(ExaminationResultId), ref _examinationResultId, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施的类型编号。
		/// </summary>
		public uint? AssetTypeId
		{
			get
			{
				return _assetTypeId;
			}
			set
			{
				this.SetPropertyValue(nameof(AssetTypeId), ref _assetTypeId, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施的分类编号。
		/// </summary>
		public uint? AssetClassId
		{
			get
			{
				return _assetClassId;
			}
			set
			{
				this.SetPropertyValue(nameof(AssetClassId), ref _assetClassId, value);
			}
		}

		/// <summary>
		/// 获取或设置生产厂商的编号。
		/// </summary>
		public uint? ManufacturerId
		{
			get
			{
				return _manufacturerId;
			}
			set
			{
				this.SetPropertyValue(nameof(ManufacturerId), ref _manufacturerId, value);
			}
		}

		/// <summary>
		/// 获取或设置生产日期。
		/// </summary>
		public DateTime? ManufacturedDate
		{
			get
			{
				return _manufacturedDate;
			}
			set
			{
				this.SetPropertyValue(nameof(ManufacturedDate), ref _manufacturedDate, value);
			}
		}

		/// <summary>
		/// 获取或设置所属企业编号。
		/// </summary>
		public uint CorporationId
		{
			get
			{
				return _corporationId;
			}
			set
			{
				this.SetPropertyValue(nameof(CorporationId), ref _corporationId, value);
			}
		}

		/// <summary>
		/// 获取或设置所属分支机构编号。
		/// </summary>
		public ushort BranchId
		{
			get
			{
				return _branchId;
			}
			set
			{
				this.SetPropertyValue(nameof(BranchId), ref _branchId, value);
			}
		}

		/// <summary>
		/// 获取或设置归属部门(即资产设施的可见范围)。
		/// </summary>
		public ushort? DepartmentId
		{
			get
			{
				return _departmentId;
			}
			set
			{
				this.SetPropertyValue(nameof(DepartmentId), ref _departmentId, value);
			}
		}

		/// <summary>
		/// 获取或设置所属数据模式（有关[数据模式]的更多详情请参考数据库结构设计文档）。
		/// </summary>
		public string Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				this.SetPropertyValue(nameof(Schema), ref _schema, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施种类。
		/// </summary>
		public byte Kind
		{
			get
			{
				return _kind;
			}
			set
			{
				this.SetPropertyValue(nameof(Kind), ref _kind, value);
			}
		}

		/// <summary>
		/// 获取或设置评测等级。
		/// </summary>
		public byte Grade
		{
			get
			{
				return _grade;
			}
			set
			{
				this.SetPropertyValue(nameof(Grade), ref _grade, value);
			}
		}

		/// <summary>
		/// 获取或设置评测等级描述。
		/// </summary>
		public string GradeDescription
		{
			get
			{
				return _gradeDescription;
			}
			set
			{
				this.SetPropertyValue(nameof(GradeDescription), ref _gradeDescription, value);
			}
		}

		/// <summary>
		/// 获取或设置评测分数。
		/// </summary>
		public byte Score
		{
			get
			{
				return _score;
			}
			set
			{
				this.SetPropertyValue(nameof(Score), ref _score, value);
			}
		}

		/// <summary>
		/// 获取或设置评分时间。
		/// </summary>
		public DateTime? ScoringTimestamp
		{
			get
			{
				return _scoringTimestamp;
			}
			set
			{
				this.SetPropertyValue(nameof(ScoringTimestamp), ref _scoringTimestamp, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施的数据来源。
		/// </summary>
		public AssetSource Source
		{
			get
			{
				return _source;
			}
			set
			{
				this.SetPropertyValue(nameof(Source), ref _source, value);
			}
		}

		/// <summary>
		/// 获取或设置外观状态。
		/// </summary>
		public byte Appearance
		{
			get
			{
				return _appearance;
			}
			set
			{
				this.SetPropertyValue(nameof(Appearance), ref _appearance, value);
			}
		}

		/// <summary>
		/// 获取或设置标记值。
		/// </summary>
		public AssetFlags Flags
		{
			get
			{
				return _flags;
			}
			set
			{
				this.SetPropertyValue(nameof(Flags), ref _flags, value);
			}
		}

		/// <summary>
		/// 获取或设置标记变更时间。
		/// </summary>
		public DateTime? FlagsTimestamp
		{
			get
			{
				return _flagsTimestamp;
			}
			set
			{
				this.SetPropertyValue(nameof(FlagsTimestamp), ref _flagsTimestamp, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施状态。
		/// </summary>
		public AssetStatus Status
		{
			get
			{
				return _status;
			}
			set
			{
				this.SetPropertyValue(nameof(Status), ref _status, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施状态变更时间。
		/// </summary>
		public DateTime? StatusTimestamp
		{
			get
			{
				return _statusTimestamp;
			}
			set
			{
				this.SetPropertyValue(nameof(StatusTimestamp), ref _statusTimestamp, value);
			}
		}

		/// <summary>
		/// 获取或设置状态描述。
		/// </summary>
		public string StatusDescription
		{
			get
			{
				return _statusDescription;
			}
			set
			{
				this.SetPropertyValue(nameof(StatusDescription), ref _statusDescription, value);
			}
		}

		/// <summary>
		/// 获取或设置关联的人员编号。
		/// </summary>
		public uint? PersonId
		{
			get
			{
				return _personId;
			}
			set
			{
				this.SetPropertyValue(nameof(PersonId), ref _personId, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施所在位置编号。
		/// </summary>
		public uint? PlaceId
		{
			get
			{
				return _placeId;
			}
			set
			{
				this.SetPropertyValue(nameof(PlaceId), ref _placeId, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施所在地址编号（行政区划代码）。
		/// </summary>
		public uint? AddressId
		{
			get
			{
				return _addressId;
			}
			set
			{
				this.SetPropertyValue(nameof(AddressId), ref _addressId, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施所在的详细地址。
		/// </summary>
		public string AddressDetail
		{
			get
			{
				return _addressDetail;
			}
			set
			{
				this.SetPropertyValue(nameof(AddressDetail), ref _addressDetail, value);
			}
		}

		/// <summary>
		/// 获取或设置资产设施所在社区（村镇）。
		/// </summary>
		public string Village
		{
			get
			{
				return _village;
			}
			set
			{
				this.SetPropertyValue(nameof(Village), ref _village, value);
			}
		}

		/// <summary>
		/// 获取或设置海拔高度（单位：米）。
		/// </summary>
		public short Altitude
		{
			get
			{
				return _altitude;
			}
			set
			{
				this.SetPropertyValue(nameof(Altitude), ref _altitude, value);
			}
		}

		/// <summary>
		/// 获取或设置经度坐标。
		/// </summary>
		public decimal Longitude
		{
			get
			{
				return _longitude;
			}
			set
			{
				this.SetPropertyValue(nameof(Longitude), ref _longitude, value);
			}
		}

		/// <summary>
		/// 获取或设置纬度坐标。
		/// </summary>
		public decimal Latitude
		{
			get
			{
				return _latitude;
			}
			set
			{
				this.SetPropertyValue(nameof(Latitude), ref _latitude, value);
			}
		}

		/// <summary>
		/// 获取或设置一个值，表示该项数据是否可见。
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				this.SetPropertyValue(nameof(Visible), ref _visible, value);
			}
		}

		/// <summary>
		/// 获取或设置启用时间。
		/// </summary>
		public DateTime? ActiveTime
		{
			get
			{
				return _activeTime;
			}
			set
			{
				this.SetPropertyValue(nameof(ActiveTime), ref _activeTime, value);
			}
		}

		/// <summary>
		/// 获取或设置挂牌时间。
		/// </summary>
		public DateTime? PlatedTime
		{
			get
			{
				return _platedTime;
			}
			set
			{
				this.SetPropertyValue(nameof(PlatedTime), ref _platedTime, value);
			}
		}

		/// <summary>
		/// 获取或设置简介文件内容。
		/// </summary>
		public string Summary
		{
			get
			{
				return _summary;
			}
			set
			{
				this.SetPropertyValue(nameof(Summary), ref _summary, value);
			}
		}

		/// <summary>
		/// 获取简介文件的路径。
		/// </summary>
		public string SummaryPath
		{
			get
			{
				return _summaryPath;
			}
			internal set
			{
				this.SetPropertyValue(nameof(SummaryPath), ref _summaryPath, value);
			}
		}

		/// <summary>
		/// 获取或设置附件编号标识(编号以逗号或分号分隔)。
		/// </summary>
		public string AttachmentMark
		{
			get
			{
				return _attachmentMark;
			}
			set
			{
				this.SetPropertyValue(nameof(AttachmentMark), ref _attachmentMark, value);
			}
		}

		/// <summary>
		/// 获取或设置责任人编号。
		/// </summary>
		public uint? ResponsibleId
		{
			get
			{
				return _responsibleId;
			}
			set
			{
				this.SetPropertyValue(nameof(ResponsibleId), ref _responsibleId, value);
			}
		}

		/// <summary>
		/// 获取或设置责任人名称。
		/// </summary>
		public string ResponsibleName
		{
			get
			{
				return _responsibleName;
			}
			set
			{
				this.SetPropertyValue(nameof(ResponsibleName), ref _responsibleName, value);
			}
		}

		/// <summary>
		/// 获取或设置责任人电话号码。
		/// </summary>
		public string ResponsiblePhoneNumber
		{
			get
			{
				return _responsiblePhoneNumber;
			}
			set
			{
				this.SetPropertyValue(nameof(ResponsiblePhoneNumber), ref _responsiblePhoneNumber, value);
			}
		}

		/// <summary>
		/// 获取或设置运营单位编号。
		/// </summary>
		public uint? OperatingVendorId
		{
			get
			{
				return _operatingVendorId;
			}
			set
			{
				this.SetPropertyValue(nameof(OperatingVendorId), ref _operatingVendorId, value);
			}
		}

		/// <summary>
		/// 获取或设置运营单位名称。
		/// </summary>
		public string OperatingVendorName
		{
			get
			{
				return _operatingVendorName;
			}
			set
			{
				this.SetPropertyValue(nameof(OperatingVendorName), ref _operatingVendorName, value);
			}
		}

		/// <summary>
		/// 获取或设置运营单位电话号码。
		/// </summary>
		public string OperatingVendorPhoneNumber
		{
			get
			{
				return _operatingVendorPhoneNumber;
			}
			set
			{
				this.SetPropertyValue(nameof(OperatingVendorPhoneNumber), ref _operatingVendorPhoneNumber, value);
			}
		}

		/// <summary>
		/// 获取或设置保留数据（不要展示，仅限程序使用）。
		/// </summary>
		public string ReservedData
		{
			get
			{
				return _reservedData;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedData), ref _reservedData, value);
			}
		}

		/// <summary>
		/// 保留文本字段1。
		/// </summary>
		public string ReservedText1
		{
			get
			{
				return _reservedText1;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedText1), ref _reservedText1, value);
			}
		}

		/// <summary>
		/// 保留文本字段2。
		/// </summary>
		public string ReservedText2
		{
			get
			{
				return _reservedText2;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedText2), ref _reservedText2, value);
			}
		}

		/// <summary>
		/// 保留文本字段3。
		/// </summary>
		public string ReservedText3
		{
			get
			{
				return _reservedText3;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedText3), ref _reservedText3, value);
			}
		}

		/// <summary>
		/// 保留文本字段4。
		/// </summary>
		public string ReservedText4
		{
			get
			{
				return _reservedText4;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedText4), ref _reservedText4, value);
			}
		}

		/// <summary>
		/// 保留计数字段1。
		/// </summary>
		public int ReservedCount1
		{
			get
			{
				return _reservedCount1;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedCount1), ref _reservedCount1, value);
			}
		}

		/// <summary>
		/// 保留计数字段2。
		/// </summary>
		public int ReservedCount2
		{
			get
			{
				return _reservedCount2;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedCount2), ref _reservedCount2, value);
			}
		}

		/// <summary>
		/// 保留计数字段3。
		/// </summary>
		public int ReservedCount3
		{
			get
			{
				return _reservedCount3;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedCount3), ref _reservedCount3, value);
			}
		}

		/// <summary>
		/// 保留计数字段4。
		/// </summary>
		public int ReservedCount4
		{
			get
			{
				return _reservedCount4;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedCount4), ref _reservedCount4, value);
			}
		}

		/// <summary>
		/// 保留金额字段1。
		/// </summary>
		public decimal ReservedAmount1
		{
			get
			{
				return _reservedAmount1;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedAmount1), ref _reservedAmount1, value);
			}
		}

		/// <summary>
		/// 保留金额字段2。
		/// </summary>
		public decimal ReservedAmount2
		{
			get
			{
				return _reservedAmount2;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedAmount2), ref _reservedAmount2, value);
			}
		}

		/// <summary>
		/// 保留金额字段3。
		/// </summary>
		public decimal ReservedAmount3
		{
			get
			{
				return _reservedAmount3;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedAmount3), ref _reservedAmount3, value);
			}
		}

		/// <summary>
		/// 保留金额字段4。
		/// </summary>
		public decimal ReservedAmount4
		{
			get
			{
				return _reservedAmount4;
			}
			set
			{
				this.SetPropertyValue(nameof(ReservedAmount4), ref _reservedAmount4, value);
			}
		}

		/// <summary>
		/// 获取或设置创建人编号。
		/// </summary>
		public uint CreatorId
		{
			get
			{
				return _creatorId;
			}
			set
			{
				this.SetPropertyValue(nameof(CreatorId), ref _creatorId, value);
			}
		}

		/// <summary>
		/// 获取或设置创建时间。
		/// </summary>
		public DateTime CreatedTime
		{
			get
			{
				return _createdTime;
			}
			set
			{
				this.SetPropertyValue(nameof(CreatedTime), ref _createdTime, value);
			}
		}

		/// <summary>
		/// 获取或设置备注说明。
		/// </summary>
		public string Remark
		{
			get
			{
				return _remark;
			}
			set
			{
				this.SetPropertyValue(nameof(Remark), ref _remark, value);
			}
		}
		#endregion

		#region 关联属性
		/// <summary>
		/// 获取或设置子资产设施集。
		/// </summary>
		public IEnumerable<AssetModel> Children
		{
			get
			{
				return _children;
			}
			set
			{
				this.SetPropertyValue(nameof(Children), ref _children, value);
			}
		}
		#endregion
	}
}
