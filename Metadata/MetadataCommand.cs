using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public class MetadataCommand : MetadataElementBase
	{
		#region 成员字段
		private string _name;
		private string _text;
		private Type _resultType;
		private MetadataCommandParameterCollection _parameters;
		#endregion

		#region 构造函数
		public MetadataCommand(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_parameters = new MetadataCommandParameterCollection(this);
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

		/// <summary>
		/// 获取命令元素的全称，即为“容器名.元素名”。
		/// </summary>
		public string FullName
		{
			get
			{
				var container = this.Container;

				if(container == null || string.IsNullOrWhiteSpace(container.Name))
					return _name;

				return container.Name + "." + _name;
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public Type ResultType
		{
			get
			{
				return _resultType;
			}
			set
			{
				_resultType = value;
			}
		}

		public MetadataCommandParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// 获取关联元素所属的容器元素。
		/// </summary>
		public MetadataContainer Container
		{
			get
			{
				return (MetadataContainer)base.Owner;
			}
		}
		#endregion
	}
}
