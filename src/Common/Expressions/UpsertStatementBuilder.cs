using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class UpsertStatementBuilder : IStatementBuilder
	{
		public UpsertStatement Build(DataUpsertionContext context)
		{
			throw new NotImplementedException();
		}

		IStatement IStatementBuilder.Build(DataAccessContextBase context)
		{
			if(context.Method == DataAccessMethod.Upsert)
				return this.Build((DataUpsertionContext)context);

			//抛出数据异常
			throw new DataException($"The {this.GetType().Name} builder does not support the {context.Method} operation.");
		}
	}
}
