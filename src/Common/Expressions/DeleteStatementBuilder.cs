using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class DeleteStatementBuilder : IStatementBuilder
	{
		public DeleteStatement Build(DataDeletionContext context)
		{
			throw new NotImplementedException();
		}

		IExpression IStatementBuilder.Build(DataAccessContextBase context)
		{
			if(context.Method == DataAccessMethod.Delete)
				return this.Build((DataDeletionContext)context);

			//抛出数据异常
			throw new DataException($"The {this.GetType().Name} builder does not support the {context.Method} operation.");
		}
	}
}
