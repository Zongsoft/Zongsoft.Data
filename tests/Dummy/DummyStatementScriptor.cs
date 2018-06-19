using System;
using System.Text;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyStatementScriptor : StatementScriptorBase
	{
		#region 构造函数
		public DummyStatementScriptor()
		{
		}
		#endregion

		#region 重写方法
		protected override void GenerateDelete(DeleteStatement statement, StringBuilder text)
		{
			throw new NotImplementedException();
		}

		protected override void GenerateInsert(InsertStatement statement, StringBuilder text)
		{
			throw new NotImplementedException();
		}

		protected override void GenerateUpsert(UpsertStatement statement, StringBuilder text)
		{
			throw new NotImplementedException();
		}

		protected override void GenerateUpdate(UpdateStatement statement, StringBuilder text)
		{
			throw new NotImplementedException();
		}

		protected override void GenerateSelect(SelectStatement statement, StringBuilder text)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
