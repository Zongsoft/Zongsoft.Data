using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data.Runtime
{
	public class DataPipelineSelector : IExecutionPipelineSelector
	{
		public IEnumerable<ExecutionPipeline> SelectPipelines(DataExecutorContext context, IEnumerable<ExecutionPipeline> pipelines)
		{
			if(context == null)
				yield break;

			if(pipelines == null)
				pipelines = context.Executor.Pipelines;

			switch(context.Action)
			{
				case DataAccessAction.Execute:
					foreach(var pipeline in pipelines)
					{
						yield return pipeline;
					}
					break;
				case DataAccessAction.Count:
				case DataAccessAction.Select:
					foreach(var pipeline in pipelines)
					{
						var dataHandler = pipeline.Handler as IDataHandler;

						if(dataHandler != null && (dataHandler.AccessMode & DataAccessMode.Read) == DataAccessMode.Read)
							yield return pipeline;
					}
					break;
				case DataAccessAction.Delete:
				case DataAccessAction.Insert:
				case DataAccessAction.Update:
					foreach(var pipeline in pipelines)
					{
						var dataHandler = pipeline.Handler as IDataHandler;

						if(dataHandler != null && (dataHandler.AccessMode & DataAccessMode.Write) == DataAccessMode.Write)
							yield return pipeline;
					}
					break;
			}

			yield break;
		}

		IEnumerable<ExecutionPipeline> IExecutionPipelineSelector.SelectPipelines(IExecutionContext context, IEnumerable<ExecutionPipeline> pipelines)
		{
			return this.SelectPipelines(context as DataExecutorContext, pipelines);
		}
	}
}
