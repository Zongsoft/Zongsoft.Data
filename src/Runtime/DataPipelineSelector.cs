/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

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
