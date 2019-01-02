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
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Collections;
using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问上下文的接口。
	/// </summary>
	public interface IDataAccessContext : IDataAccessContextBase
	{
		/// <summary>
		/// 获取当前上下文对应的数据源。
		/// </summary>
		IDataSource Source
		{
			get;
		}

		/// <summary>
		/// 获取数据提供程序。
		/// </summary>
		IDataProvider Provider
		{
			get;
		}
	}

	public class DataCountContext : DataCountContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		#endregion

		#region 构造函数
		public DataCountContext(IDataAccess dataAccess, string name, ICondition condition, string includes, object state = null) : base(dataAccess, name, condition, includes, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}
		#endregion
	}

	public class DataExistContext : DataExistContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		#endregion

		#region 构造函数
		public DataExistContext(IDataAccess dataAccess, string name, ICondition condition, object state = null) : base(dataAccess, name, condition, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}
		#endregion
	}

	public class DataExecuteContext : DataExecuteContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly ICommandMetadata _command;
		#endregion

		#region 构造函数
		public DataExecuteContext(IDataAccess dataAccess, string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters, object state = null) : base(dataAccess, name, isScalar, resultType, inParameters, outParameters, state)
		{
			_provider = DataEnvironment.Providers.GetProvider(dataAccess.Name);

			if(!_provider.Metadata.Commands.TryGet(name, out _command))
				throw new DataException($"The specified '{name}' command mapping does not exist.");
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public ICommandMetadata Command
		{
			get
			{
				return _command;
			}
		}
		#endregion
	}

	public class DataIncrementContext : DataIncrementContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		#endregion

		#region 构造函数
		public DataIncrementContext(IDataAccess dataAccess, string name, string member, ICondition condition, int interval, object state = null) : base(dataAccess, name, member, condition, interval, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}
		#endregion
	}

	public class DataDeleteContext : DataDeleteContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		private volatile IReadOnlyNamedCollection<Schema> _schemas;
		#endregion

		#region 构造函数
		public DataDeleteContext(IDataAccess dataAccess, string name, ICondition condition, string schema, object state = null) : base(dataAccess, name, condition, schema, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}

		public IReadOnlyNamedCollection<Schema> Schemas
		{
			get
			{
				if(_schemas == null && !string.IsNullOrEmpty(this.Schema))
					_schemas = Zongsoft.Data.Schema.Parse(this.Schema, _entity, typeof(object));

				return _schemas;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnPropertyChanged(string propertyName)
		{
			if(propertyName == nameof(this.Schema))
				_schemas = null;

			//调用基类同名方法
			base.OnPropertyChanged(propertyName);
		}
		#endregion
	}

	public class DataInsertContext : DataInsertContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		private volatile IReadOnlyNamedCollection<Schema> _schemas;
		#endregion

		#region 构造函数
		public DataInsertContext(IDataAccess dataAccess, string name, bool isMultiple, object data, string schema, object state = null) : base(dataAccess, name, isMultiple, data, schema, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}

		public IReadOnlyNamedCollection<Schema> Schemas
		{
			get
			{
				if(_schemas == null)
					_schemas = Zongsoft.Data.Schema.Parse(string.IsNullOrEmpty(this.Schema) ? "*" : this.Schema, _entity, this.EntityType);

				return _schemas;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnPropertyChanged(string propertyName)
		{
			if(propertyName == nameof(this.Schema))
				_schemas = null;

			//调用基类同名方法
			base.OnPropertyChanged(propertyName);
		}
		#endregion
	}

	public class DataUpsertContext : DataUpsertContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		private volatile IReadOnlyNamedCollection<Schema> _schemas;
		#endregion

		#region 构造函数
		public DataUpsertContext(IDataAccess dataAccess, string name, bool isMultiple, object data, string schema, object state = null) : base(dataAccess, name, isMultiple, data, schema, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}

		public IReadOnlyNamedCollection<Schema> Schemas
		{
			get
			{
				if(_schemas == null)
					_schemas = Zongsoft.Data.Schema.Parse(string.IsNullOrEmpty(this.Schema) ? "*" : this.Schema, _entity, this.EntityType);

				return _schemas;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnPropertyChanged(string propertyName)
		{
			if(propertyName == nameof(this.Schema))
				_schemas = null;

			//调用基类同名方法
			base.OnPropertyChanged(propertyName);
		}
		#endregion
	}

	public class DataUpdateContext : DataUpdateContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		private volatile IReadOnlyNamedCollection<Schema> _schemas;
		#endregion

		#region 构造函数
		public DataUpdateContext(IDataAccess dataAccess, string name, bool isMultiple, object data, ICondition condition, string schema, object state = null) : base(dataAccess, name, isMultiple, data, condition, schema, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}

		public IReadOnlyNamedCollection<Schema> Schemas
		{
			get
			{
				if(_schemas == null)
					_schemas = Zongsoft.Data.Schema.Parse(string.IsNullOrEmpty(this.Schema) ? "*" : this.Schema, _entity, this.EntityType);

				return _schemas;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnPropertyChanged(string propertyName)
		{
			if(propertyName == nameof(this.Schema))
				_schemas = null;

			//调用基类同名方法
			base.OnPropertyChanged(propertyName);
		}
		#endregion
	}

	public class DataSelectContext : DataSelectContextBase, IDataAccessContext
	{
		#region 成员字段
		private IDataSource _source;
		private readonly IDataProvider _provider;
		private readonly IEntityMetadata _entity;
		private volatile IReadOnlyNamedCollection<Schema> _schemas;
		#endregion

		#region 构造函数
		public DataSelectContext(IDataAccess dataAccess, string name, Type entityType, Grouping grouping, ICondition condition, string schema, Paging paging, Sorting[] sortings, object state = null) : base(dataAccess, name, entityType, grouping, condition, schema, paging, sortings, state)
		{
			DataAccessContextUtility.Initialize(dataAccess.Name, name, out _provider, out _entity);
		}
		#endregion

		#region 公共属性
		public IDataSource Source
		{
			get
			{
				if(_source == null)
				{
					lock(this)
					{
						if(_source == null)
							_source = _provider.Multiplexer.GetSource(this);
					}
				}

				return _source;
			}
		}

		public IDataProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public IEntityMetadata Entity
		{
			get
			{
				return _entity;
			}
		}

		public IReadOnlyNamedCollection<Schema> Schemas
		{
			get
			{
				if(_schemas == null)
					_schemas = Zongsoft.Data.Schema.Parse(string.IsNullOrEmpty(this.Schema) ? "*" : this.Schema, _entity, this.EntityType);

				return _schemas;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnPropertyChanged(string propertyName)
		{
			if(propertyName == nameof(this.Schema))
				_schemas = null;

			//调用基类同名方法
			base.OnPropertyChanged(propertyName);
		}
		#endregion
	}

	internal static class DataAccessContextUtility
	{
		internal static void Initialize(string applicationName, string accessName, out IDataProvider provider, out IEntityMetadata entity)
		{
			provider = DataEnvironment.Providers.GetProvider(applicationName);

			if(!provider.Metadata.Entities.TryGet(accessName, out entity))
				throw new DataException($"The specified '{accessName}' entity mapping does not exist.");
		}
	}
}
