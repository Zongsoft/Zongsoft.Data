using System;
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class ConditionExpression : ICollection<IExpression>, IExpression
	{
		#region 成员字段
		private IList<IExpression> _items;
		#endregion

		#region 构造函数
		public ConditionExpression(ConditionCombination combination, IEnumerable<IExpression> items)
		{
			this.ConditionCombination = combination;

			if(items == null)
				_items = new List<IExpression>();
			else
				_items = new List<IExpression>(items);
		}
		#endregion

		#region 公共属性
		public ConditionCombination ConditionCombination
		{
			get;
		}

		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region 公共方法
		public void Add(IExpression item)
		{
			if(item != null)
				_items.Add(item);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool Contains(IExpression item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(IExpression[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		public bool Remove(IExpression item)
		{
			return _items.Remove(item);
		}

		public IEnumerator<IExpression> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion

		#region 静态方法
		public static ConditionExpression And(IEnumerable<IExpression> items)
		{
			return new ConditionExpression(ConditionCombination.And, items);
		}

		public static ConditionExpression And(params IExpression[] items)
		{
			return new ConditionExpression(ConditionCombination.And, items);
		}

		public static ConditionExpression Or(IEnumerable<IExpression> items)
		{
			return new ConditionExpression(ConditionCombination.Or, items);
		}

		public static ConditionExpression Or(params IExpression[] items)
		{
			return new ConditionExpression(ConditionCombination.Or, items);
		}
		#endregion
	}
}
