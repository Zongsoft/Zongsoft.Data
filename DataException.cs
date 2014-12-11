using System;

namespace Zongsoft.Data
{
	public class DataException : Exception
	{
		public DataException(string message, Exception innerException = null) : base(message, innerException)
		{
		}
	}
}
