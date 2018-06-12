/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.MySql.
 *
 * Zongsoft.Data.MySql is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data.MySql is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data.MySql; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.MySql
{
	public class MySqlExpressionVisitor : ExpressionVisitor
	{
		#region 构造函数
		public MySqlExpressionVisitor(StringBuilder text) : base(text)
		{
		}
		#endregion

		#region 重写方法
		protected override string GetIdentifier(string name)
		{
			return $"`{name}`";
		}

		protected override string GetAggregateMethodName(Grouping.AggregateMethod method)
		{
			switch(method)
			{
				case Grouping.AggregateMethod.Count:
					return "COUNT";
				case Grouping.AggregateMethod.Sum:
					return "SUM";
				case Grouping.AggregateMethod.Average:
					return "AVG";
				case Grouping.AggregateMethod.Maximum:
					return "MAX";
				case Grouping.AggregateMethod.Minimum:
					return "MIN";
				case Grouping.AggregateMethod.Deviation:
					return "STDEV";
				case Grouping.AggregateMethod.DeviationPopulation:
					return "STDEV_POP";
				case Grouping.AggregateMethod.Variance:
					return "VARIANCE";
				case Grouping.AggregateMethod.VariancePopulation:
					return "VAR_POP";
			}

			//其他采用基类实现
			return base.GetAggregateMethodName(method);
		}

		protected override IExpression VisitVariable(VariableIdentifier variable)
		{
			if(variable.IsGlobal)
				this.Text.Append("@@" + variable.Name);
			else
				this.Text.Append("@" + variable.Name);

			return variable;
		}

		protected override IExpression VisitParameter(ParameterExpression parameter)
		{
			this.Text.Append("@" + parameter.Name);
			return parameter;
		}
		#endregion
	}
}
