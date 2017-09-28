/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public static class DataAccessContextExtension
	{
		private const string KEY_ENTITYMAPPER_STATE = "__EntityMapper__";

		#region 公共方法
		public static IDataEntityMapper GetMapper(this DataAccessContextBase context)
		{
			if(context.HasStates && context.States.TryGetValue(KEY_ENTITYMAPPER_STATE, out var mapper))
				return (IDataEntityMapper)mapper;

			foreach(var mapping in DataAccessEnvironment.Instance.Mappings)
			{
				var found = mapping.GetEntityMapper(context.Name);

				if(found != null)
				{
					context.States[KEY_ENTITYMAPPER_STATE] = found;
					return found;
				}
			}

			return null;
		}
		#endregion
	}
}
