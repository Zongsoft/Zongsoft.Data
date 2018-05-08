using System;
using System.Collections.Generic;

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
