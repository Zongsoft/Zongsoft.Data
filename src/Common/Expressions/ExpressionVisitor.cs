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
using System.Text;
using System.Threading;

namespace Zongsoft.Data.Common.Expressions
{
	public class ExpressionVisitor : IExpressionVisitor
	{
		#region 事件定义
		public event EventHandler<ExpressionEventArgs> Unrecognized;
		#endregion

		#region 私有变量
		private int _conditionDepth;
		#endregion

		#region 成员字段
		private int _depth;
		private readonly StringBuilder _output;
		#endregion

		#region 构造函数
		protected ExpressionVisitor(StringBuilder output = null)
		{
			_depth = -1;
			_output = output ?? new StringBuilder(256);
		}
		#endregion

		#region 公共属性
		public int Depth
		{
			get
			{
				return _depth;
			}
		}

		public virtual IExpressionDialect Dialect
		{
			get
			{
				return NormalDialect.Instance;
			}
		}

		public StringBuilder Output
		{
			get
			{
				return _output;
			}
		}
		#endregion

		#region 公共方法
		public IExpression Visit(IExpression expression)
		{
			if(expression == null)
				return null;

			IExpression result;

			//递增深度变量
			Interlocked.Increment(ref _depth);

			switch(expression)
			{
				case TableIdentifier table:
					result = this.VisitTable(table);
					break;
				case FieldIdentifier field:
					result = this.VisitField(field);
					break;
				case FieldDefinition field:
					result = this.VisitField(field);
					break;
				case VariableIdentifier variable:
					result = this.VisitVariable(variable);
					break;
				case ParameterExpression parameter:
					result = this.VisitParameter(parameter);
					break;
				case LiteralExpression literal:
					result = this.VisitLiteral(literal);
					break;
				case CommentExpression comment:
					result = this.VisitComment(comment);
					break;
				case ConstantExpression constant:
					result = this.VisitConstant(constant);
					break;
				case UnaryExpression unary:
					result = this.VisitUnary(unary);
					break;
				case BinaryExpression binary:
					result = this.VisitBinary(binary);
					break;
				case RangeExpression range:
					result = this.VisitRange(range);
					break;
				case AggregateExpression aggregate:
					result = this.VisitAggregate(aggregate);
					break;
				case MethodExpression method:
					result = this.VisitMethod(method);
					break;
				case ConditionExpression condition:
					result = this.VisitCondition(condition);
					break;
				case ExpressionCollection collection:
					result = this.VisitCollection(collection);
					break;
				case IStatement statement:
					result = this.VisitStatement(statement);
					break;
				default:
					result = this.OnUnrecognized(expression);
					break;
			}

			//递减深度变量
			Interlocked.Decrement(ref _depth);

			//返回访问后的表达式
			return result;
		}
		#endregion

		#region 虚拟方法
		protected virtual IExpression VisitTable(TableIdentifier table)
		{
			if(table.IsTemporary)
				_output.Append(table.Name);
			else
				_output.Append(this.GetIdentifier(table.Name));

			if(!string.IsNullOrEmpty(table.Alias) && !string.Equals(table.Name, table.Alias))
				_output.Append(" AS " + table.Alias);

			return table;
		}

		protected virtual IExpression VisitField(FieldIdentifier field)
		{
			if(field.Table == null || string.IsNullOrEmpty(field.Table.Alias))
				_output.Append(this.GetIdentifier(field.Name));
			else
				_output.Append(field.Table.Alias + "." + this.GetIdentifier(field.Name));

			if(!string.IsNullOrEmpty(field.Alias))
				_output.Append(" AS " + this.GetAlias(field.Alias));

			return field;
		}

		protected virtual IExpression VisitField(FieldDefinition field)
		{
			_output.Append($"{field.Name} {this.Dialect.GetDbType(field.DbType)}");

			switch(field.DbType)
			{
				case DbType.Binary:
				case DbType.String:
				case DbType.StringFixedLength:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					if(field.Length > 0)
						_output.Append("(" + field.Length.ToString() + ")");
					break;
				case DbType.Single:
				case DbType.Double:
				case DbType.Decimal:
				case DbType.VarNumeric:
					if(field.Precision > 0)
						_output.Append($"({field.Precision.ToString()},{field.Scale.ToString()})");
					break;
			}

			if(field.Nullable)
				_output.Append(" NULL");
			else
				_output.Append(" NOT NULL");

			return field;
		}

		protected virtual IExpression VisitVariable(VariableIdentifier variable)
		{
			if(variable.IsGlobal)
				_output.Append("@@" + variable.Name);
			else
				_output.Append("@" + variable.Name);

			return variable;
		}

		protected virtual IExpression VisitParameter(ParameterExpression parameter)
		{
			_output.Append("@" + parameter.Name);
			return parameter;
		}

		protected virtual IExpression VisitLiteral(LiteralExpression literal)
		{
			if(string.IsNullOrEmpty(literal.Text))
				return null;

			//输出字面量文本
			_output.Append(literal.Text);

			return literal;
		}

		protected virtual IExpression VisitComment(CommentExpression comment)
		{
			if(string.IsNullOrEmpty(comment.Text))
				return null;

			//输出注释文本
			_output.Append("/* " + comment.Text + " */");

			return comment;
		}

		protected virtual IExpression VisitConstant(ConstantExpression constant)
		{
			if(constant.Value == null)
			{
				_output.Append("NULL");
				return constant;
			}

			if(Zongsoft.Common.TypeExtension.IsNumeric(constant.ValueType))
				_output.Append(constant.Value.ToString());
			else
				_output.Append("'" + constant.Value.ToString() + "'");

			return constant;
		}

		protected virtual IExpression VisitUnary(UnaryExpression unary)
		{
			switch(unary.Operator)
			{
				case Operator.Minus:
					_output.Append("-");
					break;
				case Operator.Not:
					_output.Append("NOT ");
					break;
			}

			//只有常量和标记表达式才不需要加小括号
			bool parenthesisRequired = !(unary.Operand is ConstantExpression || unary.Operand is IIdentifier);

			if(parenthesisRequired)
				_output.Append("(");

			this.Visit(unary.Operand);

			if(parenthesisRequired)
				_output.Append(")");

			return unary;
		}

		protected virtual IExpression VisitBinary(BinaryExpression expression)
		{
			//是否需要括号包裹
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
			_output.Append(" " + this.GetSymbol(expression.Operator) + (parenthesisRequired ? " (" : " "));
			this.Visit(expression.Right);

			if(parenthesisRequired)
				_output.Append(")");

			return expression;
		}

		protected virtual IExpression VisitRange(RangeExpression expression)
		{
			this.Visit(expression.Minimum);
			_output.Append(" AND ");
			this.Visit(expression.Maximum);

			return expression;
		}

		protected virtual IExpression VisitMethod(MethodExpression expression)
		{
			_output.Append(expression.Name + "(");

			var index = 0;

			foreach(var argument in expression.Arguments)
			{
				if(index++ > 0)
					_output.Append(",");

				this.Visit(argument);
			}

			_output.Append(")");

			if(!string.IsNullOrEmpty(expression.Alias))
				_output.Append(" AS " + this.GetAlias(expression.Alias));

			return expression;
		}

		protected virtual IExpression VisitAggregate(AggregateExpression aggregate)
		{
			aggregate.Name = this.GetFunctionName(aggregate.Method);
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

			if(condition.Count > 1 && _conditionDepth++ > 0)
				_output.Append("(");

			foreach(var item in condition)
			{
				if(index++ > 0)
					_output.Append(" " + this.GetSymbol(combination) + " ");

				this.Visit(item);
			}

			if(condition.Count > 1 && --_conditionDepth > 0)
				_output.Append(")");

			return condition;
		}

		protected virtual IExpression VisitCollection(ExpressionCollection collection)
		{
			if(collection.Count == 0)
				return collection;

			//输出左括号
			_output.Append("(");

			int index = 0;

			foreach(var item in collection)
			{
				if(index++ > 0)
					_output.Append(",");

				this.Visit(item);
			}

			//输出右括号
			_output.Append(")");

			return collection;
		}

		protected virtual IExpression VisitStatement(IStatement statement)
		{
			return statement;
		}
		#endregion

		#region 事件激发
		protected virtual IExpression OnUnrecognized(IExpression expression)
		{
			var unrecognized = this.Unrecognized;

			if(unrecognized == null)
				return expression;

			var args = new ExpressionEventArgs(_output, expression);
			unrecognized(this, args);
			return args.Result;
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetSymbol(Operator @operator)
		{
			return this.Dialect.GetSymbol(@operator) ?? NormalDialect.Instance.GetSymbol(@operator);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetIdentifier(string name)
		{
			return this.Dialect.GetIdentifier(name);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetAlias(string alias)
		{
			return this.Dialect.GetAlias(alias);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetFunctionName(Grouping.AggregateMethod method)
		{
			return this.Dialect.GetFunctionName(method);
		}
		#endregion

		#region 嵌套子类
		private class NormalDialect : IExpressionDialect
		{
			#region 单例字段
			public static readonly NormalDialect Instance = new NormalDialect();
			#endregion

			#region 私有构造
			private NormalDialect()
			{
			}
			#endregion

			#region 公共方法
			public string GetDbType(DbType dbType)
			{
				switch(dbType)
				{
					case DbType.AnsiString:
						return "varchar";
					case DbType.AnsiStringFixedLength:
						return "char";
					case DbType.Binary:
						return "varbinary";
					case DbType.Boolean:
						return "bit";
					case DbType.Byte:
						return "unsigned tinyint";
					case DbType.Currency:
						return "currency";
					case DbType.Date:
						return "date";
					case DbType.DateTime:
						return "datetime";
					case DbType.DateTime2:
						return "datetime2";
					case DbType.DateTimeOffset:
						return "datetime";
					case DbType.Decimal:
						return "decimal";
					case DbType.Double:
						return "double";
					case DbType.Guid:
						return "guid";
					case DbType.Int16:
						return "smallint";
					case DbType.Int32:
						return "int";
					case DbType.Int64:
						return "bigint";
					case DbType.Object:
						return "object";
					case DbType.SByte:
						return "tinyint";
					case DbType.Single:
						return "float";
					case DbType.String:
						return "nvarchar";
					case DbType.StringFixedLength:
						return "nchar";
					case DbType.Time:
						return "time";
					case DbType.UInt16:
						return "unsigned smallint";
					case DbType.UInt32:
						return "unsigned int";
					case DbType.UInt64:
						return "unsigned bigint";
					case DbType.Xml:
						return "xml";
				}

				return dbType.ToString();
			}

			public string GetSymbol(Operator @operator)
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
						throw new DataException($"Unsupported '{@operator}' operator.");
				}
			}

			public string GetIdentifier(string name)
			{
				return name;
			}

			public string GetAlias(string alias)
			{
				return "'" + alias + "'";
			}

			public string GetFunctionName(Grouping.AggregateMethod method)
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
			#endregion
		}
		#endregion
	}
}
