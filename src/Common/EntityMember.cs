using System;
using System.Data;
using System.Reflection;

namespace Zongsoft.Data.Common
{
	public class EntityMember : Zongsoft.Reflection.MemberToken
	{
		#region 成员字段
		private Action<object, IDataRecord, int> _populate;
		#endregion

		#region 构造函数
		public EntityMember(FieldInfo field, Action<object, IDataRecord, int> populate) : base(field)
		{
			_populate = populate;
		}

		public EntityMember(PropertyInfo property, Action<object, IDataRecord, int> populate) : base(property)
		{
			_populate = populate;
		}
		#endregion

		#region 公共方法
		public void Populate(object entity, IDataRecord record, int ordinal)
		{
			_populate?.Invoke(entity, record, ordinal);
		}
		#endregion
	}
}
