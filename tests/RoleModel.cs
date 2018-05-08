using System;
using System.Collections.Generic;

using Zongsoft.Security.Membership;

namespace Zongsoft.Data.Tests
{
	public class RoleModel : Role
	{
		public IEnumerable<User> Users
		{
			get;
			set;
		}

		public IEnumerable<Role> Roles
		{
			get;
			set;
		}
	}
}
