using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Runtime
{
	public class DataEvaluator
	{
		public void Evaluate(DataExecutorContext context)
		{
		}

		public void EvaluateSelect(DataExecutorContext context)
		{
			var parameter = context.Parameter as DataSelectParameter;

			if(parameter == null)
				return;

			var concept = context.MetadataManager.GetConceptContainer(parameter.ContainerName, parameter.Namespace);
			var entity = concept.Entities[parameter.ElementName];
			var mapping = context.MetadataManager.GetMapping(parameter.QualifiedName);

			
		}
	}
}
