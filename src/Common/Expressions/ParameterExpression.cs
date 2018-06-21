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
using System.Data;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class ParameterExpression : Expression
	{
		#region 构造函数
		public ParameterExpression(string name, string path, FieldIdentifier field)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			this.Name = name;
			this.Path = path;
			this.Field = field ?? throw new ArgumentNullException(nameof(field));
			this.Direction = ParameterDirection.Input;
		}

		public ParameterExpression(string name, object value, FieldIdentifier field)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name.Trim();
			this.Value = value;
			this.Field = field ?? throw new ArgumentNullException(nameof(field));
			this.Direction = ParameterDirection.Input;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取参数名称。
		/// </summary>
		/// <remarks>
		///		<para>如果参数名为空或问号(?)表示该参数名由集合定义，当该参数被加入到语句的参数集中，该名称将被更改为特定序号的名字。可参考<see cref="Statement.Parameters"/>属性的集合。</para>
		/// </remarks>
		public string Name
		{
			get;
			internal set;
		}

		public string Path
		{
			get;
		}

		public ParameterDirection Direction
		{
			get;
			set;
		}

		public DbType DbType
		{
			get;
			set;
		}

		public FieldIdentifier Field
		{
			get;
		}

		public object Value
		{
			get;
			set;
		}

		public Condition Condition
		{
			get;
		}
		#endregion

		#region 公共方法
		public IDbDataParameter Attach(IDbCommand command)
		{
			if(command == null)
				throw new ArgumentNullException(nameof(command));

			//通过命令创建一个新的空参数
			var parameter = command.CreateParameter();

			parameter.ParameterName = this.Name;
			parameter.DbType = this.DbType;
			parameter.Direction = this.Direction;

			if(string.IsNullOrEmpty(this.Path))
				parameter.Value = this.Value;

			//将参数加入到命令的参数集中
			command.Parameters.Add(parameter);

			//返回新建的参数对象
			return parameter;
		}
		#endregion
	}
}
