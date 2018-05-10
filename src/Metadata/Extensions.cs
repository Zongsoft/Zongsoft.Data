using System;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Common.Expressions;

namespace Zongsoft.Data.Metadata
{
	public static class EntityExtension
	{
		public static IEntity GetBaseEntity(this IEntity entity)
		{
			if(entity != null && !string.IsNullOrEmpty(entity.BaseName))
				return DataEnvironment.Metadata.Entities.Get(entity.BaseName);

			return null;
		}

		public static string GetTableName(this IEntity entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			return string.IsNullOrEmpty(entity.Alias) ? entity.Name : entity.Alias;
		}
	}

	public static class EntityPropertyExtension
	{
		public static string GetFieldName(this IEntityProperty property, out string alias)
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

		/// <summary>
		/// 获取指定导航属性约束项值的常量表达式。
		/// </summary>
		/// <param name="property">指定的导航属性。</param>
		/// <param name="constraint">指定的导航属性的约束项。</param>
		/// <returns>返回的约束项值对应关联属性数据类型的常量表达式。</returns>
		public static ConstantExpression GetConstraintValue(this IEntityComplexProperty property, AssociationConstraint constraint)
		{
			if(constraint.Value == null)
				return ConstantExpression.Null;

			//获取指定导航属性的关联属性
			var associatedProperty = property.Entity.Properties.Get(constraint.Name);

			//返回约束项值转换成关联属性数据类型的常量表达式
			return Expression.Constant(Zongsoft.Common.Convert.ConvertValue(constraint.Value, associatedProperty.Type), associatedProperty.Type);
		}

		/// <summary>
		/// 获取导航属性关联的目标实体对象。
		/// </summary>
		/// <param name="property">指定的导航属性。</param>
		/// <returns>返回关联的目标实体对象。</returns>
		public static IEntity GetForeignEntity(this IEntityComplexProperty property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var index = property.Role.IndexOf(':');

			if(index < 0)
				return DataEnvironment.Metadata.Entities.Get(property.Role);
			else
				return DataEnvironment.Metadata.Entities.Get(property.Role.Substring(0, index));
		}

		public static bool TryGetForeignMemberPath(this IEntityComplexProperty property, out string memberPath)
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
		public static IEntityProperty GetForeignProperty(this IEntityComplexProperty property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var index = property.Role.IndexOf(':');

			if(index < 0)
				return null;

			var entity = DataEnvironment.Metadata.Entities.Get(property.Role.Substring(0, index));
			return entity.Properties.Get(property.Role.Substring(index + 1));
		}
	}

	public static class EntityPorpertyCollectionExtension
	{
		/// <summary>
		///		<para>查找属性集合中指定成员路径对应的属性。</para>
		///		<para>注：查找范围包括父实体的属性集。</para>
		/// </summary>
		/// <param name="properties">指定要查找的属性集合。</param>
		/// <param name="path">指定要查找的成员路径，支持多级导航属性路径。</param>
		/// <param name="match">属性匹配成功后的回调函数，其各参数表示：
		///		<list type="number">
		///			<item>第一个参数，表示当前匹配属性的成员路径（注意：不含当前属性名，即不是全路径）；</item>
		///			<item>第二个参数，表示当前匹配到的属性。</item>
		///		</list>
		///		<para>
		///		回调函数的返回值为空(null)，表示查找方法继续后续的匹配；
		///		如果为真(true)则当前查找方法立即退出，并返回当前匹配到的属性；
		///		如果为假(False)则当前查找方法立即退出，并返回空(null)，即查找失败。
		///		</para>
		/// </param>
		/// <returns>返回找到的属性。</returns>
		public static IEntityProperty Find(this IEntityPropertyCollection properties, string path, Action<string, IEntityProperty> match = null)
		{
			if(string.IsNullOrEmpty(path))
				return null;

			IEntityProperty property = null;
			var parts = path.Split('.');

			for(int i = 0; i < parts.Length; i++)
			{
				if(properties == null)
					return null;

				if(properties.TryGet(parts[i], out property))
				{
					if(property.IsComplex)
						properties = ((IEntityComplexProperty)property).GetForeignEntity().Properties;
					else
						properties = null;

					match?.Invoke(string.Join(".", parts, 0, i), property);
				}
				else
				{
					property = FindBaseProperty(ref properties, parts[i]);

					if(property == null)
						return null;

					match?.Invoke(string.Join(".", parts, 0, i), property);
				}
			}

			return property;
		}

		#region 私有方法
		private static IEntityProperty FindBaseProperty(ref IEntityPropertyCollection properties, string name)
		{
			if(properties == null)
				return null;

			while(!string.IsNullOrEmpty(properties.Entity.BaseName) &&
				  DataEnvironment.Metadata.Entities.TryGet(properties.Entity.BaseName, out var baseEntity))
			{
				properties = baseEntity.Properties;

				if(properties.TryGet(name, out var property))
					return property;
			}

			return null;
		}
		#endregion
	}
}
