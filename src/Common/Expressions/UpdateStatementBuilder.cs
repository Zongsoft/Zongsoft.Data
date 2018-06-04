using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpdateStatementBuilder : IStatementBuilder
	{
		public UpdateStatement Build(DataUpdationContext context)
		{
			throw new NotImplementedException();
		}

		IExpression IStatementBuilder.Build(DataAccessContextBase context)
		{
			if(context.Method == DataAccessMethod.Delete)
				return this.Build((DataUpdationContext)context);

			//抛出数据异常
			throw new DataException($"The {this.GetType().Name} builder does not support the {context.Method} operation.");
		}
	}
}
