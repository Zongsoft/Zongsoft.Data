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
	public class MySqlSelectStatementWriter : SelectStatementWriterBase
	{
		#region 构造函数
		public MySqlSelectStatementWriter(StringBuilder text) : base(text)
		{
		}
		#endregion

		#region 重写方法
		public override void Write(SelectStatement statement)
		{
			if(this.IsCacheTable(statement))
				this.Text.AppendLine("CREATE TEMPORARY TABLE " + statement.Alias);

			//调用基类同名方法
			base.Write(statement);
		}

		protected override void OnWrote(SelectStatement statement)
		{
			if(statement.Paging != null && statement.Paging.PageSize > 0)
				this.WritePaging(statement.Paging);

			if(this.IsCacheTable(statement))
			{
				this.Text.AppendLine(";");
				this.Text.AppendLine("SELECT * FROM " + statement.Alias);
			}

			//调用基类同名方法
			base.OnWrote(statement);
		}
		#endregion

		#region 重写方法
		protected override IExpressionVisitor CreateVisitor()
		{
			return new MySqlExpressionVisitor(this.Text);
		}
		#endregion

		#region 虚拟方法
		protected virtual void WritePaging(Paging paging)
		{
			this.Text.Append("LIMIT " + paging.PageSize.ToString());

			if(paging.PageIndex > 1)
				this.Text.Append(" OFFSET " + ((paging.PageIndex - 1) * paging.PageSize).ToString());

			this.Text.AppendLine();
		}

		protected virtual bool IsCacheTable(SelectStatement statement)
		{
			return !string.IsNullOrEmpty(statement.Alias);
		}
		#endregion
	}
}
