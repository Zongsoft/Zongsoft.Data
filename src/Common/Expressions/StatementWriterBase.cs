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
	public abstract class StatementWriterBase<TStatement> : IStatementWriter<TStatement> where TStatement : IStatement
	{
		#region 成员字段
		private StringBuilder _text;
		private IExpressionVisitor _visitor;
		#endregion

		#region 构造函数
		protected StatementWriterBase(StringBuilder text)
		{
			_text = text ?? throw new ArgumentNullException(nameof(text));
		}
		#endregion

		#region 公共属性
		public StringBuilder Text
		{
			get
			{
				return _text;
			}
		}

		public IExpressionVisitor Visitor
		{
			get
			{
				if(_visitor == null)
					_visitor = this.CreateVisitor();

				return _visitor;
			}
		}
		#endregion

		#region 抽象方法
		protected abstract IExpressionVisitor CreateVisitor();
		public abstract void Write(TStatement statement);
		#endregion

		#region 虚拟方法
		protected virtual IExpression Visit(IExpression expression)
		{
			return this.Visitor.Visit(expression);
		}
		#endregion
	}
}
