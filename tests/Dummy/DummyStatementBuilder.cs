using System;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public sealed class DummyStatementBuilder : StatementBuilderBase
	{
		#region 单例字段
		public static readonly DummyStatementBuilder Default = new DummyStatementBuilder();
		#endregion

		#region 私有构造
		private DummyStatementBuilder()
		{
		}
		#endregion

		#region 重写方法
		protected override IStatementBuilder<DataSelectContext> CreateSelectStatementBuilder()
		{
			return new DummySelectStatementBuilder();
		}

		protected override IStatementBuilder<DataDeleteContext> CreateDeleteStatementBuilder()
		{
			return new DummyDeleteStatementBuilder();
		}

		protected override IStatementBuilder<DataInsertContext> CreateInsertStatementBuilder()
		{
			return new DummyInsertStatementBuilder();
		}

		protected override IStatementBuilder<DataUpdateContext> CreateUpdateStatementBuilder()
		{
			return new DummyUpdateStatementBuilder();
		}

		protected override IStatementBuilder<DataUpsertContext> CreateUpsertStatementBuilder()
		{
			return new DummyUpsertStatementBuilder();
		}

		protected override IStatementBuilder<DataCountContext> CreateCountStatementBuilder()
		{
			return new DummyCountStatementBuilder();
		}

		protected override IStatementBuilder<DataExistContext> CreateExistStatementBuilder()
		{
			return new DummyExistStatementBuilder();
		}

		protected override IStatementBuilder<DataExecuteContext> CreateExecutionStatementBuilder()
		{
			return new DummyExecutionStatementBuilder();
		}

		protected override IStatementBuilder<DataIncrementContext> CreateIncrementStatementBuilder()
		{
			return new DummyIncrementStatementBuilder();
		}
		#endregion
	}
}
