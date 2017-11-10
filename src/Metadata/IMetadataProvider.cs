using System;
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	public interface IMetadataProvider
	{
		ICollection<EntityMetadata> Entities
		{
			get;
		}

		ICollection<CommandMetadata> Commands
		{
			get;
		}

		EntityMetadata GetEntity(string name);
		CommandMetadata GetCommand(string name);
	}
}
