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
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public abstract class UpsertStatementWriterBase : StatementWriterBase<UpsertStatement>
	{
		#region 构造函数
		protected UpsertStatementWriterBase(StringBuilder text) : base(text)
		{
		}
		#endregion

		#region 公共方法
		public override void Write(UpsertStatement statement)
		{
			if(statement.Fields == null || statement.Fields.Count == 0)
				throw new DataException("Missing required fields in the insert statment.");

			this.Text.Append("INSERT INTO ");
			this.Visit(statement.Table);

			this.Text.Append(" (");

			var index = 0;

			foreach(var field in statement.Fields)
			{
				if(index++ > 0)
					this.Text.Append(",");

				this.Visit(field);
			}

			this.Text.Append(") VALUES ");
			index = 0;

			foreach(var value in statement.Values)
			{
				if(index++ > 0)
					this.Text.Append(",");

				if(index % statement.Fields.Count == 0)
					this.Text.Append("(");

				this.Visit(value);

				if(index % statement.Fields.Count == 0)
					this.Text.Append(")");
			}

			this.Text.Append(")");
		}
		#endregion
	}
}
