using System;
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class ExpressionCollection : Expression, ICollection<IExpression>
	{
		#region 成员字段
		private IList<IExpression> _items;
		#endregion

		#region 构造函数
		public ExpressionCollection()
		{
			_items = new List<IExpression>();
		}

		public ExpressionCollection(IEnumerable<IExpression> items)
		{
			if(items == null)
				_items = new List<IExpression>();
			else
				_items = new List<IExpression>(items);
		}
		#endregion

		#region 公共属性
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
			if(item == null)
				throw new ArgumentNullException(nameof(item));

			_items.Add(item);
		}

		public bool Remove(IExpression item)
		{
			return _items.Remove(item);
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
		#endregion

		#region 枚举遍历
		public IEnumerator<IExpression> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion
	}
}
