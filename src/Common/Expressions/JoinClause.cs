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

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Common.Expressions
{
	public class JoinClause : ISource
	{
		#region 常量定义
		private const string INHERIT_SYMBOL = "$";
		#endregion

		#region 成员字段
		private IExpression _condition;
		#endregion

		#region 构造函数
		public JoinClause(string name, ISource target, JoinType type = JoinType.Left)
		{
			this.Name = name ?? string.Empty;
			this.Target = target ?? throw new ArgumentNullException(nameof(target));
			this.Condition = ConditionExpression.And();
			this.Type = type;
		}

		public JoinClause(string name, ISource target, IExpression condition, JoinType type = JoinType.Left)
		{
			this.Name = name ?? string.Empty;
			this.Target = target ?? throw new ArgumentNullException(nameof(target));
			this.Condition = condition ?? throw new ArgumentNullException(nameof(IExpression));
			this.Type = type;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关联子句的标识名。
		/// </summary>
		/// <remarks>
		///		<para>在概念层生成中通过该属性来表示导航属性的名称。</para>
		/// </remarks>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取关联子句的别名，即关联的目标表（源）的别名。
		/// </summary>
		public string Alias
		{
			get
			{
				return this.Target.Alias;
			}
		}

		/// <summary>
		/// 获取关联的目标表（源）。
		/// </summary>
		public ISource Target
		{
			get;
		}

		/// <summary>
		/// 获取关联的种类。
		/// </summary>
		public JoinType Type
		{
			get;
		}

		/// <summary>
		/// 获取或设置关联的连接条件。
		/// </summary>
		public IExpression Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value ?? throw new ArgumentNullException();
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 创建一个关联当前关联子句的字段标识。
		/// </summary>
		/// <param name="name">指定的字段名称。</param>
		/// <param name="alias">指定的字段别名。</param>
		/// <returns>返回创建成功的字段标识。</returns>
		public FieldIdentifier CreateField(string name, string alias = null)
		{
			if(string.IsNullOrWhiteSpace(alias) && string.IsNullOrEmpty(this.Name))
				alias = this.Name + "." + name;

			return new FieldIdentifier(this, name, alias);
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 获取指定继承表的关联子句名称。
		/// </summary>
		/// <param name="entity">指定要关联的继承表实体（父实体）。</param>
		/// <param name="fullPath">指定的继承表所对应成员完整路径。</param>
		/// <returns>返回关联子句的名称。</returns>
		internal static string GetName(IEntityMetadata entity, string fullPath)
		{
			return string.IsNullOrEmpty(fullPath) ? INHERIT_SYMBOL + entity.Name : fullPath + INHERIT_SYMBOL + entity.Name;
		}

		/// <summary>
		/// 获取指定导航属性的关联子句名称。
		/// </summary>
		/// <param name="complex">指定要关联的导航属性。</param>
		/// <param name="fullPath">指定的导航属性对应的成员完整路径。</param>
		/// <returns>返回关联子句的名称。</returns>
		internal static string GetName(IEntityComplexPropertyMetadata complex, string fullPath)
		{
			return string.IsNullOrEmpty(fullPath) ? complex.Name : fullPath;
		}

		/// <summary>
		/// 创建指定表与它父类（如果有的话）的继承关联子句。
		/// </summary>
		/// <param name="table">指定要创建的关联子句的子表标识。</param>
		/// <param name="fullPath">指定的 <paramref name="table"/> 参数对应的成员完整路径。</param>
		/// <param name="targetCreator">关联目标表的创建器函数。</param>
		/// <returns>返回创建的继承表关联子句，如果指定的表实体没有父实体则返回空(null)。</returns>
		internal static JoinClause Create(TableIdentifier table, string fullPath, Func<IEntityMetadata, TableIdentifier> targetCreator)
		{
			if(table.Entity == null)
				throw new DataException($"The Entity property of the {table} table identifier is null.");

			//获取指定表的父实体
			var super = table.Entity.GetBaseEntity();

			//如果指定的表标识没有父类则返回空
			if(super == null)
				return null;

			//为当前导航属性创建关联子句的表标识
			var target = targetCreator(super);

			//生成当前继承表对应的关联子句（关联名为前缀+完整路径+实体名）
			var joining = new JoinClause(
				GetName(super, fullPath),
				target, JoinType.Left);

			//将关联子句的条件转换为特定的条件表达式
			var conditions = (ConditionExpression)joining.Condition;

			for(int i = 0; i < super.Key.Length; i++)
			{
				conditions.Add(
					Expression.Equal(
						table.CreateField(table.Entity.Key[i]),
						target.CreateField(target.Entity.Key[i])));
			}

			return joining;
		}

		/// <summary>
		/// 创建指定源与实体的继承关联子句。
		/// </summary>
		/// <param name="source">指定要创建关联子句的源表标识。</param>
		/// <param name="target">指定要创建关联子句的目标实体。</param>
		/// <param name="fullPath">指定的 <paramref name="target"/> 参数对应的目标实体关联的成员的完整路径。</param>
		/// <param name="targetFinder">待创建关联子句是否存在的判断函数。</param>
		/// <param name="targetCreator">创建关联子句时目标表标识的生成函数。</param>
		/// <returns>返回创建的继承表关联子句。</returns>
		internal static JoinClause Create(TableIdentifier source, IEntityMetadata target, string fullPath, Func<string, JoinClause> targetFinder, Func<IEntityMetadata, TableIdentifier> targetCreator)
		{
			if(source.Entity == null)
				throw new DataException($"The Entity property of the {source} source table identifier is null.");

			//定义要创建关联的名称
			var name = GetName(target, fullPath);

			if(targetFinder != null)
			{
				var result = targetFinder(name);

				if(result != null)
					return result;
			}

			var targetTable = targetCreator(target);
			var joining = new JoinClause(name, targetTable, JoinType.Left);
			var conditions = (ConditionExpression)joining.Condition;

			for(int i = 0; i < target.Key.Length; i++)
			{
				conditions.Add(
					Expression.Equal(
						targetTable.CreateField(target.Key[i]),
						source.CreateField(source.Entity.Key[i])));
			}

			return joining;
		}

		/// <summary>
		/// 创建导航属性的关联子句。
		/// </summary>
		/// <param name="source">指定要创建关联子句的源。</param>
		/// <param name="complex">指定要创建关联子句对应的导航属性。</param>
		/// <param name="fullPath">指定的 <paramref name="complex"/> 参数对应的成员完整路径。</param>
		/// <param name="targetFinder">待创建关联子句是否存在的判断函数。</param>
		/// <param name="targetCreator">创建关联子句时目标表标识的生成函数。</param>
		/// <returns>返回创建的导航关联子句。</returns>
		internal static JoinClause Create(ISource source, IEntityComplexPropertyMetadata complex, string fullPath, Func<string, JoinClause> targetFinder, Func<IEntityMetadata, TableIdentifier> targetCreator)
		{
			//定义要创建关联的名称
			var name = GetName(complex, fullPath);

			if(targetFinder != null)
			{
				var result = targetFinder(name);

				if(result != null)
					return result;
			}

			//为当前导航属性创建关联子句的表标识
			var target = targetCreator(complex.GetForeignEntity());

			//生成当前导航属性对应的关联子句（关联名为导航属性的完整路径）
			var joining = new JoinClause(name, target,
				(complex.Multiplicity == AssociationMultiplicity.One ? JoinType.Inner : JoinType.Left));

			//将关联子句的条件转换为特定的条件表达式
			var conditions = (ConditionExpression)joining.Condition;

			//将约束键入到关联条件中
			if(complex.HasConstraints())
			{
				foreach(var constraint in complex.Constraints)
				{
					conditions.Add(
						Expression.Equal(
							source.CreateField(constraint.Name),
							complex.GetConstraintValue(constraint)));
				}
			}

			foreach(var link in complex.Links)
			{
				conditions.Add(
					Expression.Equal(
						target.CreateField(link.Role),
						source.CreateField(link.Name)));
			}

			return joining;
		}

		/// <summary>
		/// 创建导航属性的关联子句。
		/// </summary>
		/// <param name="source">指定要创建关联子句的源。</param>
		/// <param name="schema">指定要创建关联子句对应的数据模式成员。</param>
		/// <param name="targetFinder">待创建关联子句是否存在的判断函数。</param>
		/// <param name="targetCreator">创建关联子句时目标表标识的生成函数。</param>
		/// <returns>返回创建的导航关联子句，如果 <paramref name="schema"/> 参数指定的数据模式成员对应的不是导航属性则返回空(null)。</returns>
		internal static JoinClause Create(ISource source, Schema schema, Func<string, JoinClause> targetFinder, Func<IEntityMetadata, TableIdentifier> targetCreator)
		{
			if(schema == null)
				throw new ArgumentNullException(nameof(schema));

			if(schema.Token.Property.IsSimplex)
				return null;

			return Create(source, (IEntityComplexPropertyMetadata)schema.Token.Property, schema.FullPath, targetFinder, targetCreator);
		}
		#endregion
	}
}
