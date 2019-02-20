using System;

namespace Zongsoft.Data.Tests.Models
{
	/// <summary>
	/// 表示用户配置信息的实体类。
	/// </summary>
	public class UserProfile
	{
		/// <summary>
		/// 获取或设置用户编号。
		/// </summary>
		public uint UserId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户性别。
		/// </summary>
		public Gender Gender
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户昵称。
		/// </summary>
		public string Nickname
		{
			get; set;
		}

		public string IdentityId
		{
			get; set;
		}

		public DateTime? Birthdate
		{
			get; set;
		}

		public byte Grade
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户的照片文件路径。
		/// </summary>
		public string PhotoPath
		{
			get; set;
		}

		/// <summary>
		/// 获取用户的照片文件访问URL。
		/// </summary>
		public string PhotoUrl
		{
			get;
		}

		/// <summary>
		/// 获取或设置用户累计发布的帖子总数。
		/// </summary>
		public uint TotalPosts
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户累积发布的主题总数。
		/// </summary>
		public uint TotalThreads
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户最后回帖的帖子编号。
		/// </summary>
		public ulong? MostRecentPostId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户最后回帖的时间。
		/// </summary>
		public DateTime? MostRecentPostTime
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户最新发布的主题编号。
		/// </summary>
		public ulong? MostRecentThreadId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户最新发布的主题标题。
		/// </summary>
		public string MostRecentThreadSubject
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户最新主题的发布时间。
		/// </summary>
		public DateTime? MostRecentThreadTime
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户配置信息的创建时间。
		/// </summary>
		public DateTime CreatedTime
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户配置对应的用户对象。
		/// </summary>
		public Zongsoft.Security.Membership.IUser User
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户所属的企业编号。
		/// </summary>
		public uint CorporationId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户所属的部门编号。
		/// </summary>
		public ushort DepartmentId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置用户所属的部门对象。
		/// </summary>
		public Department Department
		{
			get; set;
		}
	}
}
