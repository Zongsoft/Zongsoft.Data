using System;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.Runtime
{
	public class FromJoinClause
	{
		#region 成员字段
		private string _alias;
		private MetadataEntity _entity;
		private IList<MetadataEntityProperty> _refs;
		private IList<MetadataEntityProperty> _dependentRefs;
		private string _navigationPropertyName;
		private FromClause _from;
		private FromJoinClause _parent;
		private FromJoinClauseCollection _children;
		#endregion

		#region 构造函数
		public FromJoinClause(int aliasId, MetadataEntity entity, IList<MetadataEntityProperty> refs, IList<MetadataEntityProperty> dependentRefs)
		{
			_alias = "t" + aliasId.ToString();
			_entity = entity;
			_refs = refs;
			_dependentRefs = dependentRefs;
		}

		public FromJoinClause(string alias, MetadataEntity entity, IList<MetadataEntityProperty> refs, IList<MetadataEntityProperty> dependentRefs)
		{
			_alias = alias;
			_entity = entity;
			_refs = refs;
			_dependentRefs = dependentRefs;
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

		public IList<MetadataEntityProperty> Refs
		{
			get
			{
				return _refs;
			}
		}

		public IList<MetadataEntityProperty> DependentRefs
		{
			get
			{
				return _dependentRefs;
			}
		}

		public string NavigationPropertyName
		{
			get
			{
				return _navigationPropertyName;
			}
			set
			{
				_navigationPropertyName = value;
			}
		}

		public FromJoinClauseCollection Children
		{
			get
			{
				if(_children == null)
					System.Threading.Interlocked.CompareExchange(ref _children, new FromJoinClauseCollection(this), null);

				return _children;
			}
		}
		#endregion
	}
}
