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
		protected override IStatementBuilder CreateSelectStatementBuilder()
		{
			return new DummySelectStatementBuilder();
		}

		protected override IStatementBuilder CreateDeleteStatementBuilder()
		{
			return new DummyDeleteStatementBuilder();
		}

		protected override IStatementBuilder CreateInsertStatementBuilder()
		{
			return new DummyInsertStatementBuilder();
		}

		protected override IStatementBuilder CreateUpsertStatementBuilder()
		{
			return new DummyUpsertStatementBuilder();
		}

		protected override IStatementBuilder CreateUpdateStatementBuilder()
		{
			return new DummyUpdateStatementBuilder();
		}

		protected override IStatementBuilder CreateCountStatementBuilder()
		{
			throw new NotImplementedException();
		}

		protected override IStatementBuilder CreateExistStatementBuilder()
		{
			throw new NotImplementedException();
		}

		protected override IStatementBuilder CreateExecutionStatementBuilder()
		{
			throw new NotImplementedException();
		}

		protected override IStatementBuilder CreateIncrementStatementBuilder()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
