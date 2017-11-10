using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public abstract class EntityPropertyMetadata
	{
		#region 成员字段
		private EntityMetadata _entity;
		private string _name;
		private string _alias;
		private Type _type;
		#endregion

		#region 构造函数
		protected EntityPropertyMetadata(EntityMetadata entity, string name, Type type)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			_entity = entity;
			_name = name.Trim();
			_type = type;
		}
		#endregion

		#region 公共属性
		public EntityMetadata Entity
		{
			get
			{
				return _entity;
			}
			internal set
			{
				_entity = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
			}
		}

		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public abstract bool IsSimplex
		{
			get;
		}

		public abstract bool IsComplex
		{
			get;
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return _name + 
		}
		#endregion
	}
}
