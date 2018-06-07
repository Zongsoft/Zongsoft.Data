using System;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyStatementBuilder : StatementBuilderBase
	{
		protected override IStatementBuilder GetSelectStatementBuilder()
		{
			return new DummySelectStatementBuilder();
		}

		protected override IStatementBuilder GetDeleteStatementBuilder()
		{
			return new DummyDeleteStatementBuilder();
		}

		protected override IStatementBuilder GetInsertStatementBuilder()
		{
			return new DummyInsertStatementBuilder();
		}

		protected override IStatementBuilder GetUpsertStatementBuilder()
		{
			return new DummyUpsertStatementBuilder();
		}

		protected override IStatementBuilder GetUpdateStatementBuilder()
		{
			return new DummyUpdateStatementBuilder();
		}
	}
}
