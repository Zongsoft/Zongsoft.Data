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
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata.Profiles
{
	public class MetadataFileLoader : IMetadataLoader
	{
		#region 成员字段
		private string _path;
		#endregion

		#region 构造函数
		public MetadataFileLoader()
		{
			_path = Zongsoft.ComponentModel.ApplicationContextBase.Current?.ApplicationDirectory;
		}

		public MetadataFileLoader(string path)
		{
			if(string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			_path = path;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置要加载的目录地址，支持“|”竖线符分隔的多个目录。
		/// </summary>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				_path = value;
			}
		}
		#endregion

		#region 加载方法
		public IEnumerable<IMetadata> Load(string name)
		{
			if(string.IsNullOrEmpty(_path))
				throw new InvalidOperationException("The file or directory path to load is not specified.");

			var directories = _path.Split('|');

			foreach(var directory in directories)
			{
				//如果指定的目录不存在则返回初始化失败
				if(!Directory.Exists(directory))
					throw new InvalidOperationException($"The '{directory}' directory path to load does not exist.");

				//查找指定目录下的所有映射文件
				var files = Directory.GetFiles(directory, "*.mapping", SearchOption.AllDirectories);

				foreach(var file in files)
				{
					//加载指定的映射文件
					var metadata = MetadataFile.Load(file, name);

					//将加载成功的映射文件加入到提供程序集中
					if(metadata != null)
						yield return metadata;
				}
			}
		}
		#endregion
	}
}
