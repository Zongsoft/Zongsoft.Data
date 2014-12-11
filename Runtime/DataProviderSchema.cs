using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Data.Runtime
{
	public class DataProviderSchema
	{
		#region 成员字段
		private string _namespace;
		private DataProviderContainerCollection _containers;
		#endregion

		#region 构造函数
		public DataProviderSchema(string @namespace)
		{
			if(string.IsNullOrWhiteSpace(@namespace))
				throw new ArgumentNullException("namespace");

			_namespace = @namespace.Trim();
			_containers = new DataProviderContainerCollection();
		}
		#endregion

		#region 公共属性
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}

		public DataProviderContainerCollection Containers
		{
			get
			{
				return _containers;
			}
		}
		#endregion
	}
}
