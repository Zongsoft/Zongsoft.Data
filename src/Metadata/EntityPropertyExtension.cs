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

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Metadata
{
	public static class EntityPropertyExtension
	{
		/// <summary>
		/// 获取指定实体属性对应的字段名以及返回的别名。
		/// </summary>
		/// <param name="property">当前的实体属性。</param>
		/// <param name="alias">输出参数，对应的返回别名。详细说明请参考该方法的备注说明。</param>
		/// <returns>当前属性对应的字段名。</returns>
		/// <remarks>
		///		<para>注意：如果当前实体属性的字段名不同于属性名，则<paramref name="alias"/>输出参数值即为属性名，必须确保查询返回的字段标识都为对应的属性名，以便后续实体组装时进行字段与属性的匹配。</para>
		/// </remarks>
		public static string GetFieldName(this IEntityPropertyMetadata property, out string alias)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			if(string.IsNullOrEmpty(property.Alias))
			{
				alias = null;
				return property.Name;
			}
			else
			{
				alias = property.Name;
				return property.Alias;
			}
		}

		public static bool HasConstraints(this IEntityComplexPropertyMetadata property)
		{
			return property != null && property.Constraints != null && property.Constraints.Length > 0;
		}

		/// <summary>
		/// 获取指定导航属性约束项值的常量表达式。
		/// </summary>
		/// <param name="property">指定的导航属性。</param>
		/// <param name="constraint">指定的导航属性的约束项。</param>
		/// <returns>返回的约束项值对应关联属性数据类型的常量表达式。</returns>
		public static ConstantExpression GetConstraintValue(this IEntityComplexPropertyMetadata property, AssociationConstraint constraint)
		{
			if(constraint.Value == null)
				return ConstantExpression.Null;

			//获取指定导航属性的关联属性
			var associatedProperty = property.Entity.Properties.Get(constraint.Name);

			//返回约束项值转换成关联属性数据类型的常量表达式
			return Expression.Constant(Zongsoft.Common.Convert.ConvertValue(constraint.Value, Utility.FromDbType(associatedProperty.Type)));
		}

		/// <summary>
		/// 获取导航属性关联的目标实体对象。
		/// </summary>
		/// <param name="property">指定的导航属性。</param>
		/// <returns>返回关联的目标实体对象。</returns>
		public static IEntityMetadata GetForeignEntity(this IEntityComplexPropertyMetadata property, out IEntityPropertyMetadata foreignProperty)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			//设置返回参数默认值
			foreignProperty = null;

			var index = property.Role.IndexOf(':');

			if(index < 0)
				return property.Entity.Metadata.Manager.Entities.Get(property.Role);

			var entity = property.Entity.Metadata.Manager.Entities.Get(property.Role.Substring(0, index));
			foreignProperty = entity.Properties.Get(property.Role.Substring(index + 1));

			return entity;
		}

		/// <summary>
		/// 尝试获取导航属性关联的目标实体成员路径。
		/// </summary>
		/// <param name="property">指定的导航属性。</param>
		/// <param name="memberPath">输出属性，对应导航属性关联的目标实体成员路径。</param>
		/// <returns>如果指定的导航属性定义了关联的目标成员，则返回真(True)否则返回假(False)。</returns>
		public static bool TryGetForeignMemberPath(this IEntityComplexPropertyMetadata property, out string memberPath)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			//设置输出参数默认值
			memberPath = null;

			//获取分隔符的位置
			var index = property.Role.IndexOf(':');

			if(index > 0)
			{
				memberPath = property.Role.Substring(index + 1);
				return true;
			}

			return false;
		}

		/// <summary>
		/// 获取导航属性关联的目标实体中特定属性。
		/// </summary>
		/// <param name="property">指定的导航属性。</param>
		/// <returns>返回关联的目标实体中的特定属性。</returns>
		public static IEntityPropertyMetadata GetForeignProperty(this IEntityComplexPropertyMetadata property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var index = property.Role.IndexOf(':');

			if(index < 0)
				return null;

			var entity = property.Entity.Metadata.Manager
						.Entities
						.Get(property.Role.Substring(0, index));

			return entity.Properties.Get(property.Role.Substring(index + 1));
		}
	}
}
