using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public abstract class MetadataElementCollectionBase<TElement> : Zongsoft.Collections.NamedCollectionBase<TElement> where TElement : MetadataElementBase
	{
		#region 成员字段
		private object _owner;
		#endregion

		#region 构造函数
		protected MetadataElementCollectionBase(object owner)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");

			_owner = owner;
		}
		#endregion

		#region 保护属性
		/// <summary>
		/// 获取当前集合的所有者对象。
		/// </summary>
		protected object Owner
		{
			get
			{
				return _owner;
			}
		}
		#endregion

		#region 重写方法
		protected override void InsertItems(int index, IEnumerable<TElement> items)
		{
			if(items == null)
				throw new ArgumentNullException("items");

			foreach(var item in items)
			{
				if(item.Owner != null && !object.ReferenceEquals(_owner, item.Owner))
					throw new InvalidOperationException("The element is invalid.");

				if(item.Owner == null)
					item.Owner = _owner;
			}

			//调用基类同名方法
			base.InsertItems(index, items);
		}
		#endregion
	}
}
