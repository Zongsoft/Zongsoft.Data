using System;
using System.Collections.Concurrent;

namespace Zongsoft.Data.Common
{
	internal class AliasProvider : IDisposable
	{
		#region 私有变量
		private int _index;
		private ConcurrentDictionary<string, string> _dictionary;
		#endregion

		#region 构造函数
		public AliasProvider()
		{
			_dictionary = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共方法
		public string GetAlias(string key)
		{
			if(string.IsNullOrEmpty(key))
				return "t";

			return _dictionary.GetOrAdd(key, _ => "t" + (++_index).ToString());
		}
		#endregion

		#region 处置方法
		void IDisposable.Dispose()
		{
			var dictionary = _dictionary;

			if(dictionary != null)
				dictionary.Clear();

			_dictionary = null;
		}
		#endregion
	}
}
