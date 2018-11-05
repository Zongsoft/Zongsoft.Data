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
using System.Data;
using System.Data.Common;

using MySql.Data.MySqlClient;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.MySql
{
	public class MySqlDriver : DataDriverBase
	{
		#region 构造函数
		public MySqlDriver()
		{
			//添加 MySQL 支持的功能特性集
			this.Features.Add(DeleteFeatures.Multitable);
		}
		#endregion

		#region 公共属性
		public override string Name
		{
			get
			{
				return "MySql";
			}
		}

		public override IStatementBuilder Builder
		{
			get
			{
				return MySqlStatementBuilder.Default;
			}
		}
		#endregion

		#region 公共方法
		public override DbCommand CreateCommand()
		{
			return new MySqlCommand();
		}

		public override DbCommand CreateCommand(string text, CommandType commandType = CommandType.Text)
		{
			return new MySqlCommand(text)
			{
				CommandType = commandType,
			};
		}

		public override DbConnection CreateConnection()
		{
			return new MySqlConnection();
		}

		public override DbConnection CreateConnection(string connectionString)
		{
			return new MySqlConnection(connectionString);
		}
		#endregion

		#region 保护方法
		protected override IExpressionVisitor GetVisitor(System.Text.StringBuilder output)
		{
			return new MySqlExpressionVisitor(output);
		}
		#endregion
	}
}
