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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class Schema : ISchema, ISchema<SchemaMember>
	{
		#region 成员字段
		private SchemaParser _parser;
		private Collections.INamedCollection<SchemaMember> _members;
		#endregion

		#region 构造函数
		internal Schema(SchemaParser parser, Metadata.IEntityMetadata entity, Type entityType, Collections.INamedCollection<SchemaMember> entries)
		{
			_parser = parser ?? throw new ArgumentNullException(nameof(parser));
			this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.EntityType = entityType;
			_members = entries ?? new Collections.NamedCollection<SchemaMember>(entry => entry.Name, StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get => this.Entity.Name;
		}

		public Metadata.IEntityMetadata Entity
		{
			get;
		}

		public Type EntityType
		{
			get;
		}

		public bool IsEmpty
		{
			get => _members == null || _members.Count == 0;
		}

		public Collections.INamedCollection<SchemaMember> Members
		{
			get => _members;
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			if(_members != null)
				_members.Clear();
		}

		public bool Contains(string path)
		{
			if(string.IsNullOrEmpty(path) || this.IsEmpty)
				return false;

			var parts = path.Split('.', '/');
			var members = _members;

			for(int i = 0; i < parts.Length; i++)
			{
				if(members == null)
					return false;

				if(string.IsNullOrEmpty(parts[i]))
					continue;

				if(members.TryGet(parts[i], out var member))
					members = member.Children;
				else
					return false;
			}

			return true;
		}

		public SchemaMember Find(string path)
		{
			if(string.IsNullOrEmpty(path) || this.IsEmpty)
				return null;

			var parts = path.Split('.', '/');
			var members = _members;
			var member = (SchemaMember)null;

			for(int i = 0; i < parts.Length; i++)
			{
				if(members == null)
					return null;

				if(string.IsNullOrEmpty(parts[i]))
					continue;

				if(members.TryGet(parts[i], out member))
					members = member.Children;
				else
					return null;
			}

			return member;
		}

		public ISchema<SchemaMember> Include(string path)
		{
			if(string.IsNullOrEmpty(path))
				return this;

			var count = 0;
			var chars = new char[path.Length];

			for(int i = 0; i < chars.Length; i++)
			{
				if(path[i] == '.' || path[i] == '/')
				{
					chars[i] = '{';
					count++;
				}
				else
				{
					chars[i] = path[i];
				}
			}

			//由解析器统一进行解析处理
			_parser.Append(this, count == 0 ? path : new string(chars) + new string('}', count));

			return this;
		}

		public ISchema<SchemaMember> Exclude(string path)
		{
			if(string.IsNullOrEmpty(path))
				return this;

			var count = 0;
			var index = 0;
			var chars = new char[path.Length];

			for(int i = 0; i < chars.Length; i++)
			{
				if(path[i] == '.' || path[i] == '/')
				{
					chars[i] = '{';
					count++;
					index = i;
				}
				else
				{
					chars[i] = path[i];
				}
			}

			string expression;

			if(index > 0)
				expression = new string(chars, 0, index + 1) + '!' + new string(chars, index + 1, chars.Length - index - 1) + new string('}', count);
			else
				expression = "!" + path;

			//由解析器统一进行解析处理
			_parser.Append(this, expression);

			return this;
		}
		#endregion

		#region 显式实现
		SchemaMemberBase ISchema.Find(string path)
		{
			return this.Find(path);
		}

		ISchema ISchema.Include(string path)
		{
			return this.Include(path);
		}

		ISchema ISchema.Exclude(string path)
		{
			return this.Exclude(path);
		}
		#endregion
	}
}
