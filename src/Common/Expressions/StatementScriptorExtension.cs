using System;
using System.Data;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public static class StatementScriptorExtension
	{
		public static IDbCommand Command(this IStatementScriptor scriptor, IStatement statement, out Script script)
		{
			//通过脚本生成器生成指定语句对应的脚本
			script = scriptor.Script(statement);

			//根据生成的脚本创建对应的数据命令
			var command = scriptor.Provider.CreateCommand(script.Text);

			//根据脚本的参数生成对应的数据命令参数并加入到命令参数集中
			foreach(var parameter in script.Parameters)
			{
				parameter.Inject(command);
			}

			//返回生成的数据命令
			return command;
		}
	}
}
