using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data
{
	public class DataExecutor : Executor
	{
		#region 构造函数
		public DataExecutor(IDataAccess host) : base(host)
		{
			this.PipelineSelector = new Runtime.DataPipelineSelector();
		}
		#endregion

		#region 公共属性
		public IDataAccess DataAccess
		{
			get
			{
				return (IDataAccess)this.Host;
			}
		}
		#endregion

		#region 重写方法
		protected override IExecutionContext CreateContext(object parameter)
		{
			var context = parameter as IExecutionContext;

			if(context != null)
				return context;

			return base.CreateContext(parameter);
		}
		#endregion
	}
}
