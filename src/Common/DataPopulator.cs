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
using System.Data;
using System.Linq;
using System.Reflection;

namespace Zongsoft.Data.Common
{
	public class DataPopulator : IDataPopulator
	{
		#region 成员字段
		private Type _entityType;
		private IDataEntityCreator _entityCreator;
		#endregion

		#region 私有变量
		private string[] _names;
		private Action<object, IDataRecord, int>[] _setters;
		#endregion

		#region 构造函数
		private DataPopulator(Type entityType)
		{
			_entityType = entityType;
			_entityCreator = DataEntityCreator.Default;
		}

		private DataPopulator(Type entityType, IDataEntityCreator entityCreator)
		{
			_entityType = entityType;
			_entityCreator = entityCreator ?? DataEntityCreator.Default;
		}
		#endregion

		#region 公共属性
		public Type EntityType
		{
			get
			{
				return _entityType;
			}
		}

		public IDataEntityCreator EntityCreator
		{
			get
			{
				return _entityCreator;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_entityCreator = value;
			}
		}
		#endregion

		#region 公共方法
		public System.Collections.IEnumerable Populate(IDataReader reader, DataAccessContextBase context)
		{
			var setters = new Action<object, IDataRecord, int>[reader.FieldCount];
			var metadata = DataAccessEnvironment.Instance.MetadataManager.GetEntity(context.Name);

			for(int i = 0; i < reader.FieldCount; i++)
			{
				//获取字段名对应的属性名
				//var name = context.GetMapper().GetPropertyName(reader.GetName(i));
				var name = metadata.Properties.GetProperty(reader.GetName(i)).Name;

				//从当前实体类的属性名数组中找到对应的下标
				var index = Array.BinarySearch(_names, name, StringComparer.Ordinal);

				if(index >= 0)
					setters[i] = _setters[index];
			}

			while(reader.Read())
			{
				var entity = _entityCreator.Create(_entityType, reader);

				for(var i = 0; i < reader.FieldCount; i++)
				{
					var setter = setters[i];

					if(setter != null)
						setter.Invoke(entity, reader, i);
				}

				yield return entity;
			};
		}
		#endregion

		#region 静态方法
		internal static DataPopulator Build(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException(nameof(entityType));

			var properties = entityType
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanWrite)
				.OrderBy(p => p.Name)
				.ToArray();

			if(properties == null || properties.Length == 0)
				return null;

			var populator = new DataPopulator(entityType)
			{
				_names = new string[properties.Length],
				_setters = new Action<object, IDataRecord, int>[properties.Length],
			};

			for(int i = 0; i < properties.Length; i++)
			{
				populator._names[i] = properties[i].Name;
				populator._setters[i] = DataEmitter.GeneratePropertySetter(entityType, properties[i]);
			}

			return populator;
		}
		#endregion
	}
}
