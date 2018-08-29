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
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

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
		public virtual IEnumerable<IStatement> Build(DataAccessContextBase context, IDataSource source)
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

			return builder.Build(context, source);
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
