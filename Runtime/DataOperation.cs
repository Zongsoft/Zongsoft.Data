using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataOperation
	{
		#region 成员字段
		private IsolationLevel _isolationLevel;
		private IList<DataCommand> _commands;
		#endregion

		#region 构造函数
		public DataOperation(IEnumerable<DataCommand> commands = null, IsolationLevel isolationLevel = System.Data.IsolationLevel.Unspecified)
		{
			_isolationLevel = isolationLevel;

			if(commands != null)
				_commands = new List<DataCommand>(commands);
			else
				_commands = new List<DataCommand>();
		}
		#endregion

		#region 公共属性
		public IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
			set
			{
				_isolationLevel = value;
			}
		}

		public IList<DataCommand> Commands
		{
			get
			{
				return _commands;
			}
		}
		#endregion

		public class DataCommand
		{
			DataCommandKind Kind;
			public DbCommand Command;
		}

		public enum DataCommandKind
		{
			None,
			Reader,
			Scalar,
		}
	}
}
