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
using System.Data;
using System.Data.Common;

namespace Zongsoft.Data.Common
{
	public abstract class DataDriverBase : IDataDriver
	{
		#region 成员字段
		private readonly FeatureCollection _features;
		#endregion

		#region 构造函数
		protected DataDriverBase()
		{
			//创建功能特性集合
			_features = new FeatureCollection();
		}
		#endregion

		#region 公共属性
		public abstract string Name
		{
			get;
		}

		public FeatureCollection Features
		{
			get
			{
				return _features;
			}
		}

		public abstract Expressions.IStatementBuilder Builder
		{
			get;
		}
		#endregion

		#region 公共方法
		public virtual DbCommand CreateCommand()
		{
			return this.CreateCommand(null, CommandType.Text);
		}

		public virtual DbCommand CreateCommand(Expressions.IStatement statement)
		{
			if(statement == null)
				throw new ArgumentNullException(nameof(statement));

			var output = new StringBuilder(1024);
			var visitor = this.GetVisitor(output);

			//访问指定的语句
			visitor.Visit(statement);

			//创建指定语句的数据命令
			var command = this.CreateCommand(output.ToString());

			if(statement.HasParameters)
			{
				foreach(var parameter in statement.Parameters)
				{
					parameter.Attach(command);
				}
			}

			return command;
		}

		public abstract DbCommand CreateCommand(string text, CommandType commandType = CommandType.Text);

		public virtual DbConnection CreateConnection()
		{
			return this.CreateConnection();
		}

		public abstract DbConnection CreateConnection(string connectionString);
		#endregion

		#region 保护方法
		protected abstract Expressions.IExpressionVisitor GetVisitor(StringBuilder output);
		#endregion
	}
}
