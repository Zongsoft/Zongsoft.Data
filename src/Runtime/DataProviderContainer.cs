using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Data.Runtime
{
	public class DataProviderContainer
	{
		#region 成员字段
		private string _name;
		private HashSet<string> _entities;
		private HashSet<string> _commands;
		#endregion

		#region 构造函数
		public DataProviderContainer(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_entities = new HashSet<string>();
			_commands = new HashSet<string>();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public HashSet<string> Entities
		{
			get
			{
				return _entities;
			}
		}

		public HashSet<string> Commands
		{
			get
			{
				return _commands;
			}
		}
		#endregion
	}
}
