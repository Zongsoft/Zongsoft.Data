using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Data.Runtime
{
	public class DataPipelineSelector : IExecutionPipelineSelector<DataExecutorContext>
	{
		public virtual IEnumerable<ExecutionPipeline> Pipelines
		{
			get
			{
				return null;
			}
		}

		public IEnumerable<ExecutionPipeline> SelectPipelines(DataExecutorContext context)
		{
			if(context == null)
				yield break;

			var pipelines = this.Pipelines ?? context.Executor.Pipelines;

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

		IEnumerable<ExecutionPipeline> IExecutionPipelineSelector.SelectPipelines(IExecutorContext context)
		{
			return this.SelectPipelines(context as DataExecutorContext);
		}
	}
}
