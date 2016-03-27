using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataMappingCommandParameter : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _mappedTo;
		#endregion

		#region 构造函数
		public MetadataMappingCommandParameter(string name, string mappedTo)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(string.IsNullOrWhiteSpace(mappedTo))
				throw new ArgumentNullException("mappedTo");

			_name = name.Trim();
			_mappedTo = mappedTo.Trim();
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		public string MappedTo
		{
			get
			{
				return _mappedTo;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_mappedTo = value.Trim();
			}
		}
		#endregion
	}
}
