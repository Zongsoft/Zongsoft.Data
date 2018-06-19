﻿using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Dummy
{
	public class DummyUpsertStatementWriter : UpsertStatementWriterBase
	{
		public DummyUpsertStatementWriter(StringBuilder text) : base(text)
		{
		}

		protected override IExpressionVisitor CreateVisitor()
		{
			return new DummyExpressionVisitor(this.Text);
		}
	}
}
