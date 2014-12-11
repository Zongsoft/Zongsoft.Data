using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Data
{
	public enum DataAccessAction
	{
		Count,
		Execute,
		Delete,
		Insert,
		Update,
		Select,
		Other = 99,
	}
}
