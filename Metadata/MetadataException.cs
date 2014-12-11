using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zongsoft.Data.Metadata
{
	public class MetadataException : Exception, ISerializable
	{
		#region 构造函数
		public MetadataException()
		{
		}

		public MetadataException(string message) : base(message)
		{
		}

		public MetadataException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MetadataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
