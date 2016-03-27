using System;
using System.IO;
using System.Xml;

namespace Zongsoft.Data.Metadata
{
	public interface IMetadataResolver
	{
		MetadataFile Resolve(string filePath);
		MetadataFile Resolve(Stream stream);
		MetadataFile Resolve(TextReader reader);
		MetadataFile Resolve(XmlReader reader);
	}
}
