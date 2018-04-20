using System;
using System.Linq;
using System.Collections.Generic;

namespace Zongsoft.Data.Common
{
	internal static class Utility
	{
		public static IEnumerable<string> ResolveScope(string scope, Metadata.IEntity entity, Collections.INamedCollection<Reflection.MemberToken> members)
		{
			IEnumerable<string> Resolve(string wildcard)
			{
				foreach(var property in entity.Properties.Where(p => p.IsSimplex && (members == null || members.Contains(p.Name))))
				{
					yield return property.Name;
				}

				var baseName = entity.BaseName;

				while(!string.IsNullOrEmpty(baseName) &&
				      DataEnvironment.Metadata.Entities.TryGet(baseName, out var baseEntity))
				{
					foreach(var property in baseEntity.Properties.Where(p => p.IsSimplex && (members == null || members.Contains(p.Name))))
					{
						//忽略父表中的主键
						if(!property.IsPrimaryKey)
							yield return property.Name;
					}

					baseName = baseEntity.BaseName;
				}
			}

			return Scoping.Parse(scope).Map(Resolve);
		}
	}
}
