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
using System.IO;
using System.Xml;

using Zongsoft.Collections;

namespace Zongsoft.Data.Metadata.Profiles
{
	public class MetadataFile : IMetadataProvider
	{
		#region 成员字段
		private string _filePath;
		private Version _version;
		private INamedCollection<IEntity> _entities;
		private INamedCollection<ICommand> _commands;
		#endregion

		#region 构造函数
		public MetadataFile(string filePath, Version version)
		{
			_filePath = filePath;
			_version = version ?? new Version(1, 0);
			_entities = new NamedCollection<IEntity>(p => p.Name);
			_commands = new NamedCollection<ICommand>(p => p.Name);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取映射文件的完整路径。
		/// </summary>
		public string FilePath
		{
			get
			{
				return _filePath;
			}
		}

		/// <summary>
		/// 获取映射文件中的实体元素集。
		/// </summary>
		public INamedCollection<IEntity> Entities
		{
			get
			{
				return _entities;
			}
		}

		/// <summary>
		/// 获取映射文件中的命令元素集。
		/// </summary>
		public INamedCollection<ICommand> Commands
		{
			get
			{
				return _commands;
			}
		}
		#endregion

		#region 加载方法
		public static MetadataFile Load(string filePath)
		{
			return MetadataFileResolver.Default.Resolve(filePath);
		}

		public static MetadataFile Load(Stream stream)
		{
			return MetadataFileResolver.Default.Resolve(stream);
		}

		public static MetadataFile Load(TextReader reader)
		{
			return MetadataFileResolver.Default.Resolve(reader);
		}

		public static MetadataFile Load(XmlReader reader)
		{
			return MetadataFileResolver.Default.Resolve(reader);
		}
		#endregion
	}
}
