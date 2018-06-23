using System;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Common;

namespace Zongsoft.Data.Dummy
{
	public class DummyConnector : IDataConnector
	{
		#region 单例字段
		public static readonly DummyConnector Instance = new DummyConnector();
		#endregion

		#region 成员字段
		private readonly IDataSource _source;
		private readonly IDataSourceProvider _provider;
		private readonly IDataSourceSelector _selector;
		#endregion

		#region 构造函数
		private DummyConnector()
		{
			_source = new DataSource("local", "nothing", "dummy");
			_provider = new DummySourceProvider(_source);
			_selector = new DummySourceSelector(_source);
		}
		#endregion

		#region 公共属性
		public IDataSourceProvider Provider => _provider;

		public IDataSourceSelector Selector => _selector;
		#endregion

		#region 公共方法
		public IDataSource GetSource(DataAccessContextBase context)
		{
			return _source;
		}
		#endregion

		#region 迭代遍历
		public IEnumerator<IDataSource> GetEnumerator()
		{
			yield return _source;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			yield return _source;
		}
		#endregion

		#region 嵌套子类
		private class DummySourceProvider : IDataSourceProvider
		{
			private IDataSource _source;

			public DummySourceProvider(IDataSource source)
			{
				_source = source;
			}

			public IEnumerable<IDataSource> GetSources(string name)
			{
				yield return _source;
			}
		}

		private class DummySourceSelector : IDataSourceSelector
		{
			private IDataSource _source;

			public DummySourceSelector(IDataSource source)
			{
				_source = source;
			}

			public IDataSource GetSource(DataAccessContextBase context, IReadOnlyList<IDataSource> sources)
			{
				if(sources == null)
					return _source;

				return sources[0];
			}
		}
		#endregion
	}
}
