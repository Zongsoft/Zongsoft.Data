using System;
using System.Text;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class SelectStatementScriptor
	{
		#region 构造函数
		public SelectStatementScriptor(ExpressionWriter writer)
		{
			this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
			this.Writer.Unrecognized += Writer_Unrecognized;
		}
		#endregion

		#region 公共属性
		public ExpressionWriter Writer
		{
			get;
		}
		#endregion

		#region 公共方法
		public void Generate(StringBuilder text, IEnumerable<SelectStatement> statements)
		{
			if(text == null)
				throw new ArgumentNullException(nameof(text));

			if(statements == null)
				return;

			foreach(var statement in statements)
			{
				this.Generate(text, statement);
				text.AppendLine();
			}
		}

		public void Generate(StringBuilder text, SelectStatement statement)
		{
			if(text == null)
				throw new ArgumentNullException(nameof(text));

			if(statement == null)
				return;

			if(statement.Select != null && statement.Select.Members.Count > 0)
				this.WriteSelect(text, statement);

			if(statement.Into != null)
				this.WriteInto(text, statement);

			if(statement.From != null && statement.From.Count > 0)
				this.WriteFrom(text, statement);

			if(statement.Where != null)
				this.WriteWhere(text, statement);

			if(statement.GroupBy != null && statement.GroupBy.Keys.Count > 0)
				this.WriteGroupBy(text, statement);

			if(statement.OrderBy != null && statement.OrderBy.Members.Count > 0)
				this.WriteOrderBy(text, statement);

			if(statement.HasSlaves)
			{
				text.AppendLine();

				foreach(var slave in statement.Slaves)
				{
					text.AppendLine($"/* {slave.Name} */");
					this.Generate(text, slave);
				}
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual void WriteSelect(StringBuilder text, SelectStatement statement)
		{
			text.Append("SELECT ");

			if(statement.Select.IsDistinct)
				text.Append("DISTINCT ");

			int index = 0;

			foreach(var member in statement.Select.Members)
			{
				if(index++ > 0)
					text.AppendLine(",");

				this.Writer.Write(text, member);
			}

			text.AppendLine();
		}

		protected virtual void WriteInto(StringBuilder text, SelectStatement statement)
		{
			text.Append("INTO ");
			this.Writer.Write(text, statement.Into);
			text.AppendLine();
		}

		protected virtual void WriteFrom(StringBuilder text, SelectStatement statement)
		{
			text.Append("FROM ");

			foreach(var source in statement.From)
			{
				switch(source)
				{
					case TableIdentifier table:
						this.Writer.Write(text, table);
						text.AppendLine();

						break;
					case SelectStatement subquery:
						text.Append("(");

						//递归生成子查询语句
						this.Generate(text, subquery);

						if(string.IsNullOrEmpty(subquery.Alias))
							text.AppendLine(")");
						else
							text.AppendLine(") AS " + this.Writer.GetAlias(subquery.Alias));

						break;
					case JoinClause joining:
						this.WriteJoin(text, joining);

						break;
				}
			}
		}

		protected virtual void WriteJoin(StringBuilder text, JoinClause joining)
		{
			switch(joining.Type)
			{
				case JoinType.Inner:
					text.Append("INNER JOIN ");
					break;
				case JoinType.Left:
					text.Append("LEFT JOIN ");
					break;
				case JoinType.Right:
					text.Append("RIGHT JOIN ");
					break;
				case JoinType.Full:
					text.Append("FULL JOIN ");
					break;
			}

			switch(joining.Target)
			{
				case TableIdentifier table:
					this.Writer.Write(text, table);
					text.AppendLine(" ON /* " + joining.Name + " */");

					break;
				case SelectStatement subquery:
					text.Append("(");

					//递归生成子查询语句
					this.Generate(text, subquery);

					if(string.IsNullOrEmpty(subquery.Alias))
						text.AppendLine(") ON");
					else
						text.AppendLine(") AS " + this.Writer.GetAlias(subquery.Alias) + " ON");

					break;
			}

			this.Writer.Write(text, joining.Condition);
			text.AppendLine();
		}

		protected virtual void WriteWhere(StringBuilder text, SelectStatement statement)
		{
			text.Append("WHERE ");
			this.Writer.Write(text, statement.Where);
			text.AppendLine();
		}

		protected virtual void WriteGroupBy(StringBuilder text, SelectStatement statement)
		{
			text.Append("GROUP BY ");

			int index = 0;

			foreach(var key in statement.GroupBy.Keys)
			{
				if(index++ > 0)
					text.Append(",");

				this.Writer.Write(text, key);
			}

			text.AppendLine();

			if(statement.GroupBy.Having != null)
			{
				text.Append("HAVING ");
				this.Writer.Write(text, statement.GroupBy.Having);
				text.AppendLine();
			}
		}

		protected virtual void WriteOrderBy(StringBuilder text, SelectStatement statement)
		{
			text.Append("ORDER BY ");

			int index = 0;

			foreach(var member in statement.OrderBy.Members)
			{
				if(index++ > 0)
					text.Append(",");

				this.Writer.Write(text, member.Field);

				if(member.Mode == SortingMode.Descending)
					text.Append(" DESC");
			}

			text.AppendLine();
		}
		#endregion

		#region 事件处理
		private void Writer_Unrecognized(object sender, ExpressionEventArgs e)
		{
			if(e.Expression is SelectStatement statement)
				this.Generate(e.Output, statement);
		}
		#endregion
	}
}
