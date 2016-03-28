using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data.Runtime
{
	public class DataPipelineContext : Zongsoft.Services.Composition.ExecutionPipelineContext
	{
		#region 构造函数
		public DataPipelineContext(DataExecutorContext executorContext, ExecutionPipeline pipeline, object parameter) : base(executorContext, pipeline, parameter)
		{
		}
		#endregion

		#region 公共属性
		public DataExecutor DataExecutor
		{
			get
			{
				return (DataExecutor)base.Executor;
			}
		}

		public DataExecutorContext DataExecutorContext
		{
			get
			{
				return (DataExecutorContext)base.Context;
			}
		}
		#endregion
	}
}
