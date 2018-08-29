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
using System.Reflection;

using Zongsoft.Collections;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public class Schema : SchemaBase
	{
		#region 成员字段
		private Schema _parent;
		private INamedCollection<Schema> _children;
		#endregion

		#region 构造函数
		private Schema(EntityPropertyToken token)
		{
			this.Token = token;
		}
		#endregion

		#region 公共属性
		public override string Name
		{
			get
			{
				return this.Token.Property.Name;
			}
		}

		public new EntityPropertyToken Token
		{
			get;
		}

		public Schema Parent
		{
			get
			{
				return _parent;
			}
		}

		public override bool HasChildren
		{
			get
			{
				return _children != null && _children.Count > 0;
			}
		}

		public IReadOnlyNamedCollection<Schema> Children
		{
			get
			{
				return (IReadOnlyNamedCollection<Schema>)_children;
			}
		}
		#endregion

		#region 重写方法
		protected override SchemaBase GetParent()
		{
			return _parent;
		}

		protected override void SetParent(SchemaBase parent)
		{
			_parent = (parent as Schema) ?? throw new ArgumentException();
		}

		protected override bool TryGetChild(string name, out SchemaBase child)
		{
			child = null;

			if(_children != null && _children.TryGet(name, out var schema))
			{
				child = schema;
				return true;
			}

			return false;
		}

		protected override void AddChild(SchemaBase child)
		{
			if(!(child is Schema schema))
				throw new ArgumentNullException();

			if(_children == null)
				System.Threading.Interlocked.CompareExchange(ref _children, new NamedCollection<Schema>(item => item.Name), null);

			_children.Add(schema);
			schema._parent = this;
		}

		protected override void RemoveChild(string name)
		{
			_children?.Remove(name);
		}

		protected override void ClearChildren()
		{
			_children?.Clear();
		}
		#endregion

		#region 解析方法
		public static IReadOnlyNamedCollection<Schema> Parse(string text, IEntityMetadata entity, Type entityType)
		{
			return SchemaBase.Parse(text, token =>
			{
				var element = entity;
				var elementType = entityType;

				if(token.Parent != null)
				{
					var parent = (Schema)token.Parent;

					if(parent.Token.Property.IsSimplex)
						throw new DataException($"The specified {parent} schema does not correspond to a complex property, so its child elements cannot be defined.");

					element = ((IEntityComplexPropertyMetadata)parent.Token.Property).GetForeignEntity();

					switch(parent.Token.Member.MemberType)
					{
						case MemberTypes.Field:
							elementType = Zongsoft.Common.TypeExtension.GetElementType(((FieldInfo)parent.Token.Member).FieldType) ??
							              ((FieldInfo)parent.Token.Member).FieldType;
							break;
						case MemberTypes.Property:
							elementType = Zongsoft.Common.TypeExtension.GetElementType(((PropertyInfo)parent.Token.Member).PropertyType) ??
							              ((PropertyInfo)parent.Token.Member).PropertyType;
							break;
						case MemberTypes.Method:
							elementType = Zongsoft.Common.TypeExtension.GetElementType(((MethodInfo)parent.Token.Member).ReturnType) ??
							              ((MethodInfo)parent.Token.Member).ReturnType;
							break;
						default:
							throw new DataException($"Invalid kind of '{parent.Token.Member}' member.");
					}
				}

				if(token.Name == "*")
					return element.GetTokens(elementType)
								.Where(p => p.Property.IsSimplex)
								.Select(p => new Schema(p));

				if(element.GetTokens(elementType).TryGet(token.Name, out var stub))
					return new Schema[] { new Schema(stub) };
				else
					throw new DataException($"The specified '{token.Name}' property does not exist in the '{element.Name}' entity.");
			});
		}
		#endregion
	}
}
