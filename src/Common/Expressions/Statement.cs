using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public class Statement : Expression, IStatement
	{
		#region 成员字段
		private Collections.INamedCollection<ParameterExpression> _parameters;
		#endregion

		#region 公共属性
		public virtual bool HasParameters
		{
			get
			{
				return _parameters != null && _parameters.Count > 0;
			}
		}

		public virtual Collections.INamedCollection<ParameterExpression> Parameters
		{
			get
			{
				if(_parameters == null)
				{
					lock(this)
					{
						if(_parameters == null)
							_parameters = this.CreateParameters();
					}
				}

				return _parameters;
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual Collections.INamedCollection<ParameterExpression> CreateParameters()
		{
			return new ParameterCollection();
		}
		#endregion

		#region 嵌套子类
		private class ParameterCollection : Collections.NamedCollectionBase<ParameterExpression>
		{
			private int _index;

			protected override string GetKeyForItem(ParameterExpression item)
			{
				return item.Name;
			}

			protected override void AddItem(ParameterExpression item)
			{
				if(string.IsNullOrEmpty(item.Name) || item.Name == "?")
				{
					var index = System.Threading.Interlocked.Increment(ref _index);
					item.Name = "p" + index.ToString();
				}

				base.AddItem(item);
			}
		}
		#endregion
	}
}
