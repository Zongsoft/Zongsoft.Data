/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data.MySql
{
	public class MySqlSelectBuilder : IDataBuilder<DataSelectionContext>
	{
		public IDataOperation Build(DataSelectionContext context)
		{
			var provider = DataEnvironment.Providers.GetProvider(context);
			var entity = provider.Metadata.Entities.Get(context.Name, () => new DataException());

			var scoping = Scoping.Parse(context.Scope);
			var members = scoping.ToArray(() => provider.Metadata.Entities.Get(context.Name).Properties.Where(p => p.IsSimplex).Select(p => p.Name));
			var selection = new List<IEntityProperty>(members.Length);

			foreach(var member in members)
			{
				var property = entity.Properties.Get(member, () => new DataAccessException($"Specified '{member}' of select member is undefined."));

				selection.Add(property);
			}

			throw new NotImplementedException();
		}
	}
}
