using System;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Runtime
{
	public class FromClause
	{
		#region 成员字段
		private string _alias;
		private MetadataEntity _entity;
		private List<FromJoinClause> _joins;
		#endregion

		#region 构造函数
		public FromClause(MetadataEntity entity, int aliasId)
		{
			_entity = entity;
			_alias = "t" + aliasId.ToString();
		}
		#endregion

		#region 公共属性
		public string Alias
		{
			get
			{
				return _alias;
			}
		}

		public MetadataEntity Entity
		{
			get
			{
				return _entity;
			}
		}

		public IList<FromJoinClause> Joins
		{
			get
			{
				if(_joins == null)
					System.Threading.Interlocked.CompareExchange(ref _joins, new List<FromJoinClause>(), null);

				return _joins;
			}
		}
		#endregion
	}
}
