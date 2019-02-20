using System;
using System.Collections.Generic;

using Zongsoft.Security.Membership;

namespace Zongsoft.Data.Tests.Models
{
	public interface IRoleModel : IRole
	{
		IEnumerable<IUser> Users
		{
			get; set;
		}

		IEnumerable<IRole> Roles
		{
			get; set;
		}
	}
}
