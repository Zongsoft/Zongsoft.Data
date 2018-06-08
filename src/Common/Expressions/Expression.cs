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
	public abstract class Expression : IExpression
	{
		#region 构造函数
		protected Expression()
		{
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个字面量表达式。
		/// </summary>
		/// <param name="text">指定的字面量文本。</param>
		/// <returns>返回新建的字面量表达式对象。</returns>
		public static LiteralExpression Literal(string text)
		{
			return new LiteralExpression(text);
		}

		/// <summary>
		/// 创建一个注释表达式。
		/// </summary>
		/// <param name="text">指定的注释文本。</param>
		/// <returns>返回新建的注释表达式对象。</returns>
		public static CommentExpression Comment(string text)
		{
			return new CommentExpression(text);
		}

		/// <summary>
		/// 创建一个常量表达式。
		/// </summary>
		/// <param name="value">指定的常量值。</param>
		/// <returns>返回新建的常量表达式对象。</returns>
		public static ConstantExpression Constant(object value, Type valueType = null)
		{
			if(value == null)
				return ConstantExpression.Null;

			return new ConstantExpression(value, valueType);
		}

		public static VariableIdentifier Variable(string name, bool isGlobal = false)
		{
			return new VariableIdentifier(name, isGlobal);
		}

		public static ParameterExpression Parameter(string name, object value, FieldIdentifier field)
		{
			return new ParameterExpression(name, value, field);
		}

		public static ParameterExpression Parameter(string name, string path, FieldIdentifier field)
		{
			return new ParameterExpression(name, path, field);
		}

		/// <summary>
		/// 创建一个表示算术求反运算的单目表达式。
		/// </summary>
		/// <param name="operand">指定的要求反运算的算术表达式。</param>
		/// <returns>返回的求反后的算术表达式。</returns>
		public static UnaryExpression Negate(IExpression operand)
		{
			return new UnaryExpression(Operator.Minus, operand);
		}

		public static UnaryExpression Not(IExpression operand)
		{
			return new UnaryExpression(Operator.Not, operand);
		}

		public static BinaryExpression Assign(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Assign, left, right);
		}

		public static BinaryExpression Add(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Plus, left, right);
		}

		public static BinaryExpression Subtract(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Minus, left, right);
		}

		public static BinaryExpression Multiply(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Multiply, left, right);
		}

		public static BinaryExpression Divide(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Divide, left, right);
		}

		public static BinaryExpression Modulo(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Modulo, left, right);
		}

		public static BinaryExpression AndAlso(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.AndAlso, left, right);
		}

		public static BinaryExpression OrElse(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.OrElse, left, right);
		}

		public static BinaryExpression Exists(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Exists, left, right);
		}

		public static BinaryExpression NotExists(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.NotExists, left, right);
		}

		public static BinaryExpression In(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.In, left, right);
		}

		public static BinaryExpression NotIn(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.NotIn, left, right);
		}

		public static BinaryExpression Like(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Like, left, right);
		}

		public static BinaryExpression Between(IExpression operand, RangeExpression range)
		{
			return new BinaryExpression(Operator.Between, operand, range);
		}

		public static BinaryExpression Equal(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.Equal, left, right);
		}

		public static BinaryExpression NotEqual(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.NotEqual, left, right);
		}

		public static BinaryExpression LessThan(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.LessThan, left, right);
		}

		public static BinaryExpression LessThanOrEqual(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.LessThanOrEqual, left, right);
		}

		public static BinaryExpression GreaterThan(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.GreaterThan, left, right);
		}

		public static BinaryExpression GreaterThanOrEqual(IExpression left, IExpression right)
		{
			return new BinaryExpression(Operator.GreaterThanOrEqual, left, right);
		}
		#endregion

		#region 访问方法
		IExpression IExpression.Accept(IExpressionVisitor visitor)
		{
			return this.Accept(visitor);
		}

		internal protected virtual IExpression Accept(IExpressionVisitor visitor)
		{
			return visitor.Visit(this);
		}
		#endregion
	}
}
