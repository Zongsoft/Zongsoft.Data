using System;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data.Runtime
{
	public class DataProviderInvoker : ExecutionInvoker
	{
		protected override bool InvokeHandler(IExecutionPipelineContext context)
		{
			var dataProvider = context.Pipeline.Handler as IDataProvider;

			if(dataProvider != null)
			{
			}

			return base.InvokeHandler(context);
		}
	}
}
