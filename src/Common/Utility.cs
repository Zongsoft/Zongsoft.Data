using System;
using System.Linq;
using System.Collections.Generic;

namespace Zongsoft.Data.Common
{
	internal static class Utility
	{
		public static ISet<string> RinseScope(string scope, Metadata.IEntity entity, Collections.INamedCollection<Reflection.MemberToken> members)
		{
			IList<string> removes = null;
			var parts = Scoping.Parse(scope).Resolve(_ => entity.Properties.Where(p => p.IsSimplex).Select(p => p.Name));

			foreach(var part in parts)
			{
				var key = part;
				var index = part.IndexOf('.');

				if(index > 0)
					key = part.Substring(0, index);

				//如果实体成员不包含范围元素则将它移除
				if(!members.Contains(key) || !entity.Properties.Contains(key))
				{
					if(removes == null)
						removes = new List<string>();

					removes.Add(part);
				}
			}

			//从集合中剔除不在成员列表中的项
			parts.ExceptWith(removes);

			//返回处理过集
			return parts;
		}
	}
}
