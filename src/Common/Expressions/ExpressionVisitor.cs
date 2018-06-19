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

namespace Zongsoft.Data.Common.Expressions
{
	public class ExpressionVisitor : IExpressionVisitor
	{
		#region 事件定义
		public event EventHandler<ExpressionEventArgs> Unrecognized;
		#endregion

		#region 成员字段
		private StringBuilder _text;
		#endregion

		#region 构造函数
		protected ExpressionVisitor(StringBuilder text)
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
		#endregion

		#region 公共方法
		public virtual IExpression Visit(IExpression expression)
		{
			if(expression == null)
				return null;

			switch(expression)
			{
				case TableIdentifier table:
					return this.VisitTable(table);
				case FieldIdentifier field:
					return this.VisitField(field);
				case VariableIdentifier variable:
					return this.VisitVariable(variable);
				case ParameterExpression parameter:
					return this.VisitParameter(parameter);
				case LiteralExpression literal:
					return this.VisitLiteral(literal);
				case CommentExpression comment:
					return this.VisitComment(comment);
				case ConstantExpression constant:
					return this.VisitConstant(constant);
				case UnaryExpression unary:
					return this.VisitUnary(unary);
				case BinaryExpression binary:
					return this.VisitBinary(binary);
				case RangeExpression range:
					return this.VisitRange(range);
				case AggregateExpression aggregate:
					return this.VisitAggregate(aggregate);
				case MethodExpression method:
					return this.VisitMethod(method);
				case ConditionExpression condition:
					return this.VisitCondition(condition);
				case ExpressionCollection collection:
					return this.VisitCollection(collection);
				default:
					return this.OnUnrecognized(expression);
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual string GetSymbol(Operator @operator)
		{
			switch(@operator)
			{
				case Operator.Plus:
					return "+";
				case Operator.Minus:
					return "-";
				case Operator.Multiply:
					return "*";
				case Operator.Divide:
					return "/";
				case Operator.Modulo:
					return "%";
				case Operator.Assign:
					return "=";
				case Operator.Not:
					return "NOT";
				case Operator.AndAlso:
					return "AND";
				case Operator.OrElse:
					return "OR";
				case Operator.All:
					return "ALL";
				case Operator.Any:
					return "ANY";
				case Operator.Between:
					return "BETWEEN";
				case Operator.Exists:
					return "EXISTS";
				case Operator.NotExists:
					return "NOT EXISTS";
				case Operator.In:
					return "IN";
				case Operator.NotIn:
					return "NOT IN";
				case Operator.Like:
					return "LIKE";
				case Operator.Equal:
					return "=";
				case Operator.NotEqual:
					return "!=";
				case Operator.LessThan:
					return "<";
				case Operator.LessThanOrEqual:
					return "<=";
				case Operator.GreaterThan:
					return ">";
				case Operator.GreaterThanOrEqual:
					return ">=";
				default:
					return null;
			}
		}

		protected virtual string GetIdentifier(string name)
		{
			return name;
		}

		protected virtual string GetAlias(string alias)
		{
			return "'" + alias + "'";
		}

		protected virtual string GetAggregateMethodName(Grouping.AggregateMethod method)
		{
			switch(method)
			{
				case Grouping.AggregateMethod.Count:
					return "COUNT";
				case Grouping.AggregateMethod.Sum:
					return "SUM";
				case Grouping.AggregateMethod.Average:
					return "AVG";
				case Grouping.AggregateMethod.Maximum:
					return "MAX";
				case Grouping.AggregateMethod.Minimum:
					return "MIN";
				case Grouping.AggregateMethod.Deviation:
					return "STDEV";
				case Grouping.AggregateMethod.DeviationPopulation:
					return "STDEVP";
				case Grouping.AggregateMethod.Variance:
					return "VAR";
				case Grouping.AggregateMethod.VariancePopulation:
					return "VARP";
				default:
					return null;
			}
		}

		protected virtual IExpression VisitTable(TableIdentifier table)
		{
			_text.Append((table.IsTemporary ? "#" : string.Empty) + this.GetIdentifier(table.Name));

			if(!string.IsNullOrEmpty(table.Alias) && !string.Equals(table.Name, table.Alias))
				_text.Append(" AS " + table.Alias);

			return table;
		}

		protected virtual IExpression VisitField(FieldIdentifier field)
		{
			if(field.Table == null || string.IsNullOrEmpty(field.Table.Alias))
				_text.Append(this.GetIdentifier(field.Name));
			else
				_text.Append(field.Table.Alias + "." + this.GetIdentifier(field.Name));

			if(!string.IsNullOrEmpty(field.Alias))
				_text.Append(" AS " + this.GetAlias(field.Alias));

			return field;
		}

		protected virtual IExpression VisitVariable(VariableIdentifier variable)
		{
			if(variable.IsGlobal)
				_text.Append("@@" + variable.Name);
			else
				_text.Append("@" + variable.Name);

			return variable;
		}

		protected virtual IExpression VisitParameter(ParameterExpression parameter)
		{
			_text.Append("@" + parameter.Name);
			return parameter;
		}

		protected virtual IExpression VisitLiteral(LiteralExpression literal)
		{
			if(string.IsNullOrEmpty(literal.Text))
				return null;

			//输出字面量文本
			_text.Append(literal.Text);

			return literal;
		}

		protected virtual IExpression VisitComment(CommentExpression comment)
		{
			if(string.IsNullOrEmpty(comment.Text))
				return null;

			//输出注释文本
			_text.Append("/* " + comment.Text + " */");

			return comment;
		}

		protected virtual IExpression VisitConstant(ConstantExpression constant)
		{
			if(constant.Value == null)
			{
				_text.Append("NULL");
				return constant;
			}

			if(Zongsoft.Common.TypeExtension.IsNumeric(constant.ValueType))
				_text.Append(constant.Value.ToString());
			else
				_text.Append("'" + constant.Value.ToString() + "'");

			return constant;
		}

		protected virtual IExpression VisitUnary(UnaryExpression unary)
		{
			switch(unary.Operator)
			{
				case Operator.Minus:
					_text.Append("-");
					break;
				case Operator.Not:
					_text.Append("NOT ");
					break;
			}

			//只有常量和标记表达式才不需要加小括号
			bool parenthesisRequired = !(unary.Operand is ConstantExpression || unary.Operand is IIdentifier);

			if(parenthesisRequired)
				_text.Append("(");

			this.Visit(unary.Operand);

			if(parenthesisRequired)
				_text.Append(")");

			return unary;
		}

		protected virtual IExpression VisitBinary(BinaryExpression expression)
		{
			//是否需要括号引用
			var parenthesisRequired = false;

			switch(expression.Operator)
			{
				case Operator.All:
				case Operator.Any:
				case Operator.In:
				case Operator.NotIn:
				case Operator.Exists:
				case Operator.NotExists:
					parenthesisRequired = true;
					break;
			}

			this.Visit(expression.Left);
			_text.Append(" " + this.GetSymbol(expression.Operator) + (parenthesisRequired ? " (" : " "));
			this.Visit(expression.Right);

			if(parenthesisRequired)
				_text.Append(")");

			return expression;
		}

		protected virtual IExpression VisitRange(RangeExpression expression)
		{
			this.Visit(expression.Minimum);
			_text.Append(" AND ");
			this.Visit(expression.Maximum);

			return expression;
		}

		protected virtual IExpression VisitMethod(MethodExpression expression)
		{
			_text.Append(expression.Name + "(");

			var index = 0;

			foreach(var argument in expression.Arguments)
			{
				if(index++ > 0)
					_text.Append(",");

				this.Visit(argument);
			}

			_text.Append(")");

			if(!string.IsNullOrEmpty(expression.Alias))
				_text.Append(" AS " + this.GetAlias(expression.Alias));

			return expression;
		}

		protected virtual IExpression VisitAggregate(AggregateExpression aggregate)
		{
			aggregate.Name = this.GetAggregateMethodName(aggregate.Method);
			this.VisitMethod(aggregate);
			return aggregate;
		}

		protected virtual IExpression VisitCondition(ConditionExpression condition)
		{
			if(condition == null || condition.Count == 0)
				return null;

			int index = 0;
			var combination = Operator.AndAlso;

			if(condition.ConditionCombination == ConditionCombination.Or)
				combination = Operator.OrElse;

			if(condition.Count > 1)
				_text.Append("(");

			foreach(var item in condition)
			{
				if(index++ > 0)
					_text.Append(" " + this.GetSymbol(combination) + " ");

				this.Visit(item);
			}

			if(condition.Count > 1)
				_text.Append(")");

			return condition;
		}

		protected virtual IExpression VisitCollection(ExpressionCollection collection)
		{
			int index = 0;

			foreach(var item in collection)
			{
				if(index++ > 0)
					_text.Append(",");

				this.Visit(item);
			}

			return collection;
		}
		#endregion

		#region 事件激发
		protected virtual IExpression OnUnrecognized(IExpression expression)
		{
			var unrecognized = this.Unrecognized;

			if(unrecognized == null)
				return expression;

			var args = new ExpressionEventArgs(_text, expression);
			unrecognized(this, args);
			return args.Result;
		}
		#endregion
	}
}
