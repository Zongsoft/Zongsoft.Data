using System;
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class ExpressionWriter
	{
		#region 构造函数
		public ExpressionWriter()
		{
		}
		#endregion

		#region 公共方法
		public void Write(StringBuilder text, IEnumerable<IExpression> expressions)
		{
			if(text == null)
				throw new ArgumentNullException(nameof(text));

			if(expressions == null)
				return;

			text.Append("(");

			foreach(var expression in expressions)
			{
				this.Write(text, expression);
			}

			text.Append(")");
		}

		public void Write(StringBuilder text, IExpression expression)
		{
			if(text == null)
				throw new ArgumentNullException(nameof(text));

			if(expression == null)
				return;

			switch(expression)
			{
				case TableIdentifier table:
					this.WriteTable(text, table);
					break;
				case FieldIdentifier field:
					this.WriteField(text, field);
					break;
				case ConstantExpression constant:
					this.WriteConstant(text, constant);
					break;
				case UnaryExpression unary:
					this.WriteUnary(text, unary);
					break;
				case BinaryExpression binary:
					this.WriteBinary(text, binary);
					break;
				case AggregateExpression aggregate:
					this.WriteAggregate(text, aggregate);
					break;
				case MethodExpression method:
					this.WriteMethod(text, method);
					break;
				case ConditionExpression condition:
					this.WriteCondition(text, condition);
					break;
				default:
					this.WriteUnrecognizable(text, expression);
					break;
			}
		}
		#endregion

		#region 虚拟方法
		public virtual string GetSymbol(Operator @operator)
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

		public virtual string GetIdentifier(string name)
		{
			return name;
		}

		public virtual string GetAlias(string alias)
		{
			return "'" + alias + "'";
		}

		public virtual string GetVariable(string name)
		{
			return "@" + name;
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

		protected virtual void WriteComment(StringBuilder text, string comment)
		{
			if(string.IsNullOrWhiteSpace(comment))
				return;

			text.Append("/* " + comment + " */");
		}

		protected virtual void WriteHint(StringBuilder text, string hint)
		{
			if(string.IsNullOrWhiteSpace(hint))
				return;

			text.Append("/* " + hint + " */");
		}

		protected virtual void WriteTable(StringBuilder text, TableIdentifier table)
		{
			text.Append(this.GetIdentifier(table.Name));

			if(!string.IsNullOrEmpty(table.Alias))
				text.Append(" AS " + this.GetIdentifier(table.Alias));
		}

		protected virtual void WriteField(StringBuilder text, FieldIdentifier field)
		{
			if(field.Table == null || string.IsNullOrEmpty(field.Table.Alias))
				text.Append(this.GetIdentifier(field.Name));
			else
				text.Append(field.Table.Alias + "." + this.GetIdentifier(field.Name));

			if(!string.IsNullOrEmpty(field.Alias))
				text.Append(" AS " + this.GetAlias(field.Alias));
		}

		protected virtual void WriteConstant(StringBuilder text, ConstantExpression expression)
		{
			if(expression.Value == null)
			{
				text.Append("NULL");
				return;
			}

			if(Zongsoft.Common.TypeExtension.IsNumeric(expression.ValueType))
				text.Append(expression.Value.ToString());
			else
				text.Append("'" + expression.Value.ToString() + "'");
		}

		protected virtual void WriteUnary(StringBuilder text, UnaryExpression expression)
		{
			switch(expression.Operator)
			{
				case Operator.Minus:
					text.Append("-");
					break;
				case Operator.Not:
					text.Append("NOT ");
					break;
			}

			//只有常量和标记表达式才不需要加小括号
			bool parenthesisRequired = !(expression.Operand is ConstantExpression || expression.Operand is IIdentifier);

			if(parenthesisRequired)
				text.Append("(");

			this.Write(text, expression.Operand);

			if(parenthesisRequired)
				text.Append(")");
		}

		protected virtual void WriteBinary(StringBuilder text, BinaryExpression expression)
		{
			this.Write(text, expression.Left);
			text.Append(" " + this.GetSymbol(expression.Operator) + " ");
			this.Write(text, expression.Right);
		}

		protected virtual void WriteMethod(StringBuilder text, MethodExpression expression)
		{
			text.Append(expression.Name + "(");

			var index = 0;

			foreach(var argument in expression.Arguments)
			{
				if(index++ > 0)
					text.Append(",");

				this.Write(text, argument);
			}

			text.Append(")");

			if(!string.IsNullOrEmpty(expression.Alias))
				text.Append(" AS " + this.GetAlias(expression.Alias));
		}

		protected virtual void WriteAggregate(StringBuilder text, AggregateExpression expression)
		{
			expression.Name = this.GetAggregateMethodName(expression.Method);
			this.WriteMethod(text, expression);
		}

		protected virtual void WriteCondition(StringBuilder text, ConditionExpression expression)
		{
			if(expression == null || expression.Count == 0)
				return;

			int index = 0;
			var combination = Operator.AndAlso;

			if(expression.ConditionCombination == ConditionCombination.Or)
				combination = Operator.OrElse;

			if(expression.Count > 1)
				text.Append("(");

			foreach(var item in expression)
			{
				if(index++ > 0)
					text.Append(" " + this.GetSymbol(combination) + " ");

				this.Write(text, item);
			}

			if(expression.Count > 1)
				text.Append(")");
		}

		protected virtual void WriteUnrecognizable(StringBuilder text, IExpression expression)
		{
			throw new NotSupportedException($"Not supports '{expression.GetType()}' expression type.");
		}
		#endregion
	}
}
