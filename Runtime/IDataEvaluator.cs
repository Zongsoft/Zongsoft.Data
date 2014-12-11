using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data.Runtime
{
	public interface IDataEvaluator
	{
		void Evaluate(DataExecutorContext context, IDataProvider[] providers);
	}
}
