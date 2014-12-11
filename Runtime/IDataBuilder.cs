using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public interface IDataBuilder
	{
		DataOperation Build(DataPipelineContext context);
	}
}
