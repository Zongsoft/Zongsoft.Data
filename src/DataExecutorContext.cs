using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Runtime;
using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data
{
	public class DataExecutorContext : Zongsoft.Services.Composition.ExecutorContext
	{
		#region 成员字段
		private MetadataManager _metadataManager;
		private DataAccessAction _action;
		private string _actionName;
		#endregion

		#region 构造函数
		public DataExecutorContext(DataExecutor executor, MetadataManager metadataManager, string actionName, DataParameter parameter) : base(executor, parameter)
		{
			if(metadataManager == null)
				throw new ArgumentNullException("metadataManager");

			if(parameter == null)
				throw new ArgumentNullException("parameter");

			_metadataManager = metadataManager;
			this.ActionName = actionName;
		}

		public DataExecutorContext(DataExecutor executor, MetadataManager metadataManager, DataAccessAction action, DataParameter parameter) : base(executor, parameter)
		{
			if(metadataManager == null)
				throw new ArgumentNullException("metadataManager");

			if(parameter == null)
				throw new ArgumentNullException("parameter");

			_metadataManager = metadataManager;
			_action = action;
			_actionName = action.ToString();
		}
		#endregion

		#region 公共属性
		public IDataAccess DataAccess
		{
			get
			{
				return ((DataExecutor)this.Executor).DataAccess;
			}
		}

		public MetadataManager MetadataManager
		{
			get
			{
				return _metadataManager;
			}
		}

		public DataAccessAction Action
		{
			get
			{
				return _action;
			}
		}

		public string ActionName
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_actionName))
					return _action.ToString();

				return _actionName;
			}
			private set
			{
				_actionName = value == null ? string.Empty : value.Trim();

				if(!Enum.TryParse(_actionName, out _action))
					_action = DataAccessAction.Other;
			}
		}
		#endregion
	}
}
