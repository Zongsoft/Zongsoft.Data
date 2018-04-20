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
		public static string GetFieldName(this IEntityProperty property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			return string.IsNullOrEmpty(property.Alias) ? property.Name : property.Alias;
		}
	}
}
