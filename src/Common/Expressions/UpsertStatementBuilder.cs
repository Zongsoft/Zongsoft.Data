using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpsertStatementBuilder : IStatementBuilder
	{
		public InsertStatement Build(DataInsertionContext context)
		{
			throw new NotImplementedException();
		}

		IExpression IStatementBuilder.Build(DataAccessContextBase context)
		{
			if(context.Method == DataAccessMethod.Upsert)
				return this.Build((DataInsertionContext)context);

			//抛出数据异常
			throw new DataException($"The {this.GetType().Name} builder does not support the {context.Method} operation.");
		}
	}
}
