using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public abstract class StatementBuilderBase : IStatementBuilder
	{
		#region 构造函数
		protected StatementBuilderBase()
		{
		}
		#endregion

		#region 公共方法
		public virtual IStatement Build(DataAccessContextBase context)
		{
			IStatementBuilder builder = null;

			switch(context.Method)
			{
				case DataAccessMethod.Select:
					builder = this.GetSelectStatementBuilder();
					break;
				case DataAccessMethod.Delete:
					builder = this.GetDeleteStatementBuilder();
					break;
				case DataAccessMethod.Insert:
					builder = this.GetInsertStatementBuilder();
					break;
				case DataAccessMethod.Upsert:
					builder = this.GetUpsertStatementBuilder();
					break;
				case DataAccessMethod.Update:
					builder = this.GetUpdateStatementBuilder();
					break;
			}

			if(builder == null)
				throw new DataException("Can not get the statement builder from the context.");

			return builder.Build(context);
		}
		#endregion

		#region 抽象方法
		protected abstract IStatementBuilder GetSelectStatementBuilder();
		protected abstract IStatementBuilder GetDeleteStatementBuilder();
		protected abstract IStatementBuilder GetInsertStatementBuilder();
		protected abstract IStatementBuilder GetUpsertStatementBuilder();
		protected abstract IStatementBuilder GetUpdateStatementBuilder();
		#endregion
	}
}
