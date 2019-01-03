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
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.MySql
{
	public class MySqlStatementBuilder : StatementBuilderBase
	{
		#region 单例字段
		public static readonly MySqlStatementBuilder Default = new MySqlStatementBuilder();
		#endregion

		#region 构造函数
		protected MySqlStatementBuilder()
		{
		}
		#endregion

		#region 重写方法
		protected override IStatementBuilder<DataSelectContext> CreateSelectStatementBuilder()
		{
			return new MySqlSelectStatementBuilder();
		}

		protected override IStatementBuilder<DataDeleteContext> CreateDeleteStatementBuilder()
		{
			return new MySqlDeleteStatementBuilder();
		}

		protected override IStatementBuilder<DataInsertContext> CreateInsertStatementBuilder()
		{
			return new MySqlInsertStatementBuilder();
		}

		protected override IStatementBuilder<DataUpdateContext> CreateUpdateStatementBuilder()
		{
			return new MySqlUpdateStatementBuilder();
		}

		protected override IStatementBuilder<DataUpsertContext> CreateUpsertStatementBuilder()
		{
			return new MySqlUpsertStatementBuilder();
		}

		protected override IStatementBuilder<DataCountContext> CreateCountStatementBuilder()
		{
			return new MySqlCountStatementBuilder();
		}

		protected override IStatementBuilder<DataExistContext> CreateExistStatementBuilder()
		{
			return new MySqlExistStatementBuilder();
		}

		protected override IStatementBuilder<DataExecuteContext> CreateExecutionStatementBuilder()
		{
			return new MySqlExecutionStatementBuilder();
		}

		protected override IStatementBuilder<DataIncrementContext> CreateIncrementStatementBuilder()
		{
			return new MySqlIncrementStatementBuilder();
		}
		#endregion
	}
}
