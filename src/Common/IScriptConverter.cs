using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common
{
	public interface IScriptConverter
	{
		string GetName(Grouping.GroupAggregationMethod method);
		string GetOperator(Expressions.Operator @operator);
		string GetVariable(string name);
	}
}
