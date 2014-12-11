using System;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data.Runtime
{
	public interface IDataHandler : IExecutionHandler<DataPipelineContext>, IDataProvider
	{
	}
}
