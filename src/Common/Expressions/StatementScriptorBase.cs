﻿/*
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
using System.Text;

namespace Zongsoft.Data.Common.Expressions
{
	public abstract class StatementScriptorBase : IStatementScriptor
	{
		#region 构造函数
		protected StatementScriptorBase()
		{
		}
		#endregion

		#region 公共方法
		public virtual Script Script(IStatement statement)
		{
			if(statement == null)
				throw new ArgumentNullException(nameof(statement));

			var text = new StringBuilder(1024);

			switch(statement)
			{
				case SelectStatement select:
					this.GenerateSelect(select, text);
					break;
				case DeleteStatement delete:
					this.GenerateDelete(delete, text);
					break;
				case InsertStatement insert:
					this.GenerateInsert(insert, text);
					break;
				case UpsertStatement upsert:
					this.GenerateUpsert(upsert, text);
					break;
				case UpdateStatement update:
					this.GenerateUpdate(update, text);
					break;
			}

			if(statement.HasParameters)
				return new Script(text.ToString(), statement.Parameters);
			else
				return new Script(text.ToString());
		}
		#endregion

		#region 抽象方法
		protected abstract void GenerateSelect(SelectStatement statement, StringBuilder text);
		protected abstract void GenerateDelete(DeleteStatement statement, StringBuilder text);
		protected abstract void GenerateInsert(InsertStatement statement, StringBuilder text);
		protected abstract void GenerateUpsert(UpsertStatement statement, StringBuilder text);
		protected abstract void GenerateUpdate(UpdateStatement statement, StringBuilder text);
		#endregion
	}
}
