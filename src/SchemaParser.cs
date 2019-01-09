/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;
using System.Collections.Generic;

using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public class SchemaParser : SchemaParserBase<SchemaEntry>
	{
		#region 成员字段
		private IDataProvider _provider;
		#endregion

		#region 构造函数
		public SchemaParser(IDataProvider provider)
		{
			_provider = provider ?? throw new ArgumentNullException(nameof(provider));
		}
		#endregion

		#region 解析方法
		public override ISchema<SchemaEntry> Parse(string name, string expression, Type entityType)
		{
			var entity = _provider.Metadata.Entities.Get(name);

			if(string.IsNullOrEmpty(expression))
				return new Schema(this, entity, entityType, null);
			else
				return new Schema(this, entity, entityType, base.Parse(expression, token => this.Resolve(token), new SchemaData(entity, entityType)));
		}

		private IEnumerable<SchemaEntry> Resolve(SchemaEntryToken token)
		{
			var data = (SchemaData)token.Data;

			if(token.Parent != null)
			{
				var parent = token.Parent;

				if(parent.Token.Property.IsSimplex)
					throw new DataException($"The specified {parent} schema does not correspond to a complex property, so its child elements cannot be defined.");

				data.Entity = ((IEntityComplexPropertyMetadata)parent.Token.Property).GetForeignEntity(out var foreignProperty);

				while(foreignProperty != null && foreignProperty.IsComplex)
				{
					data.Entity = ((IEntityComplexPropertyMetadata)foreignProperty).GetForeignEntity(out foreignProperty);
				}

				if(parent.Token.Member != null)
				{
					switch(parent.Token.Member.MemberType)
					{
						case MemberTypes.Field:
							data.EntityType = Zongsoft.Common.TypeExtension.GetElementType(((FieldInfo)parent.Token.Member).FieldType) ??
							                  ((FieldInfo)parent.Token.Member).FieldType;
							break;
						case MemberTypes.Property:
							data.EntityType = Zongsoft.Common.TypeExtension.GetElementType(((PropertyInfo)parent.Token.Member).PropertyType) ??
							                  ((PropertyInfo)parent.Token.Member).PropertyType;
							break;
						case MemberTypes.Method:
							data.EntityType = Zongsoft.Common.TypeExtension.GetElementType(((MethodInfo)parent.Token.Member).ReturnType) ??
							                  ((MethodInfo)parent.Token.Member).ReturnType;
							break;
						default:
							throw new DataException($"Invalid kind of '{parent.Token.Member}' member.");
					}
				}
			}

			if(token.Name == "*")
				return data.Entity.GetTokens(data.EntityType)
				                  .Where(p => p.Property.IsSimplex)
				                  .Select(p => new SchemaEntry(p));

			var current = data.Entity;
			ICollection<IEntityMetadata> ancestors = null;

			while(current != null)
			{
				if(current.GetTokens(data.EntityType).TryGet(token.Name, out var stub))
					return new []{ new SchemaEntry(stub, ancestors) };

				if(ancestors == null)
					ancestors = new List<IEntityMetadata>();

				current = current.GetBaseEntity();
				ancestors.Add(current);
			}

			throw new DataException($"The specified '{token.Name}' property does not exist in the '{data.Entity.Name}' entity.");
		}
		#endregion

		#region 内部方法
		internal void Append(Schema schema, string expression)
		{
			var entries = base.Parse(expression, token => this.Resolve(token), new SchemaData(schema.Entity, schema.EntityType), schema.Entries);
		}
		#endregion

		#region 嵌套结构
		private struct SchemaData
		{
			public IEntityMetadata Entity;
			public Type EntityType;

			public SchemaData(IEntityMetadata entity, Type entityType)
			{
				this.Entity = entity;
				this.EntityType = entityType ?? typeof(object);
			}
		}
		#endregion
	}
}
