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

using Zongsoft.Common;
using Zongsoft.Data.Common;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public class DataAccess : DataAccessBase
	{
		#region 成员字段
		private IDataProvider _provider;
		#endregion

		#region 构造函数
		public DataAccess(string name) : base(name)
		{
		}

		public DataAccess(string name, IEnumerable<IDataAccessFilter> filters) : base(name)
		{
			if(filters != null)
			{
				foreach(var filter in filters)
				{
					if(filter != null)
						this.Filters.Add(filter);
				}
			}
		}
		#endregion

		#region 公共属性
		public override IDataMetadataContainer Metadata
		{
			get => this.Provider.Metadata;
		}

		public IDataProvider Provider
		{
			get
			{
				if(_provider == null)
				{
					_provider = DataEnvironment.Providers.GetProvider(this.Name);

					if(_provider != null)
						_provider.Error += this.Provider_Error;
				}

				return _provider;
			}
		}

		private void Provider_Error(object sender, DataAccessErrorEventArgs args)
		{
			this.OnError(args);
		}
		#endregion

		#region 获取主键
		public override string[] GetKey(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//获取指定名称的数据实体定义
			var entity = this.Provider.Metadata.Entities.Get(name);

			if(entity == null || entity.Key == null || entity.Key.Length == 0)
				return null;

			//创建返回的主键成员名的数组
			var members = new string[entity.Key.Length];

			for(var i = 0; i < members.Length; i++)
			{
				members[i] = entity.Key[i].Name;
			}

			return members;
		}
		#endregion

		#region 执行方法
		protected override void OnExecute(DataExecuteContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 存在方法
		protected override void OnExists(DataExistContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 计数方法
		protected override void OnCount(DataCountContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 递增方法
		protected override void OnIncrement(DataIncrementContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 删除方法
		protected override void OnDelete(DataDeleteContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 插入方法
		protected override void OnInsert(DataInsertContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 复写方法
		protected override void OnUpsert(DataUpsertContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 更新方法
		protected override void OnUpdate(DataUpdateContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 查询方法
		protected override void OnSelect(DataSelectContextBase context)
		{
			this.Provider.Execute((IDataAccessContext)context);
		}
		#endregion

		#region 模式解析
		protected override ISchemaParser CreateSchema()
		{
			return new SchemaParser(this.Provider);
		}
		#endregion

		#region 序号构建
		protected override ISequence CreateSequence(ISequence sequence)
		{
			if(sequence == null)
				return null;

			return new DataSequenceProvider(this.Provider, sequence);
		}
		#endregion

		#region 调用过滤
		protected override void OnFiltered(IDataAccessContextBase context)
		{
			//首先调用本数据访问器的过滤器后趋部分
			base.OnFiltered(context);

			//最后调用全局过滤器的前趋部分
			foreach(var filter in DataEnvironment.Filters)
			{
				if(filter == null)
					continue;

				var predication = filter as Zongsoft.Services.IPredication;

				if(predication == null || predication.Predicate(context))
				{
					filter.OnFiltered(context);
				}
			}
		}

		protected override void OnFiltering(IDataAccessContextBase context)
		{
			//首先调用全局过滤器的前趋部分
			foreach(var filter in DataEnvironment.Filters)
			{
				if(filter == null)
					continue;

				var predication = filter as Zongsoft.Services.IPredication;

				if(predication == null || predication.Predicate(context))
				{
					filter.OnFiltering(context);
				}
			}

			//最后调用本数据访问器的过滤器前趋部分
			base.OnFiltering(context);
		}
		#endregion

		#region 上下文法
		protected override DataCountContextBase CreateCountContext(string name, ICondition condition, string member, object state)
		{
			return new DataCountContext(this, name, condition, member, state);
		}

		protected override DataExistContextBase CreateExistContext(string name, ICondition condition, object state)
		{
			return new DataExistContext(this, name, condition, state);
		}

		protected override DataExecuteContextBase CreateExecuteContext(string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, object state)
		{
			return new DataExecuteContext(this, name, isScalar, resultType, inParameters, null, state);
		}

		protected override DataIncrementContextBase CreateIncrementContext(string name, string member, ICondition condition, int interval, object state)
		{
			return new DataIncrementContext(this, name, member, condition, interval, state);
		}

		protected override DataDeleteContextBase CreateDeleteContext(string name, ICondition condition, ISchema schema, object state)
		{
			return new DataDeleteContext(this, name, condition, schema, state);
		}

		protected override DataInsertContextBase CreateInsertContext(string name, bool isMultiple, object data, ISchema schema, object state)
		{
			return new DataInsertContext(this, name, isMultiple, data, schema, state);
		}

		protected override DataUpsertContextBase CreateUpsertContext(string name, bool isMultiple, object data, ISchema schema, object state)
		{
			return new DataUpsertContext(this, name, isMultiple, data, schema, state);
		}

		protected override DataUpdateContextBase CreateUpdateContext(string name, bool isMultiple, object data, ICondition condition, ISchema schema, object state)
		{
			return new DataUpdateContext(this, name, isMultiple, data, condition, schema, state);
		}

		protected override DataSelectContextBase CreateSelectContext(string name, Type entityType, ICondition condition, Grouping grouping, ISchema schema, Paging paging, Sorting[] sortings, object state)
		{
			return new DataSelectContext(this, name, entityType, grouping, condition, schema, paging, sortings, state);
		}
		#endregion

		#region 内部方法
		internal long Increase(Metadata.IDataSequence sequence, object data)
		{
			if(this.Sequence == null)
				throw new InvalidOperationException($"Missing required sequence of the '{this.Name}' DataAccess.");

			return ((DataSequenceProvider)this.Sequence).Increase(sequence, data);
		}
		#endregion

		#region 嵌套子类
		private class DataSequenceProvider : ISequence
		{
			#region 常量定义
			private const string SEQUENCE_KEY = "Zongsoft.Sequence:";
			#endregion

			#region 成员字段
			private readonly ISequence _sequence;
			private readonly IDataProvider _provider;
			#endregion

			#region 构造函数
			public DataSequenceProvider(IDataProvider provider, ISequence sequence)
			{
				_provider = provider ?? throw new ArgumentNullException(nameof(provider));
				_sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
			}
			#endregion

			#region 公共方法
			public long Increase(Metadata.IDataSequence sequence, object data)
			{
				if(sequence == null)
					throw new ArgumentNullException(nameof(sequence));

				return _sequence.Increment(this.GetSequenceKey(sequence, data), sequence.Interval, sequence.Seed);
			}
			#endregion

			#region 显式实现
			long ISequence.Increment(string key, int interval, int seed)
			{
				key = this.GetSequenceKey(key, out var sequence);

				return _sequence.Increment(key,
					interval == 1 ? sequence.Interval : interval,
					seed == 0 ? sequence.Seed : seed);
			}

			long ISequence.Decrement(string key, int interval, int seed)
			{
				key = this.GetSequenceKey(key, out var sequence);

				return _sequence.Decrement(key,
					interval == 1 ? sequence.Interval : interval,
					seed == 0 ? sequence.Seed : seed);
			}

			void ISequence.Reset(string key, int value)
			{
				key = this.GetSequenceKey(key, out var sequence);
				_sequence.Reset(key, value == 0 ? sequence.Seed : value);
			}
			#endregion

			#region 私有方法
			private string GetSequenceKey(string key, out Metadata.IDataSequence sequence)
			{
				sequence = null;

				if(string.IsNullOrEmpty(key))
					throw new ArgumentNullException(nameof(key));

				var index = key.LastIndexOfAny(new[] { ':', '.', '@' });
				object data = null;

				if(index > 0 && key[index] == '@')
				{
					data = key.Substring(index + 1).Split(',', '|', '-');
					index = key.LastIndexOfAny(new[] { ':', '.' }, index);
				}

				if(index < 0)
					throw new ArgumentException($"Invalid sequence key, the sequence key must separate the entity name and property name with a colon or a dot.");

				if(!_provider.Metadata.Entities.TryGet(key.Substring(0, index), out var entity))
					throw new ArgumentException($"The '{key.Substring(0, index)}' entity specified in the sequence key does not exist.");

				if(!entity.Properties.TryGet(key.Substring(index + 1), out var found) || found.IsComplex)
					throw new ArgumentException($"The '{key.Substring(index + 1)}' property specified in the sequence key does not exist or is not a simplex property.");

				sequence = ((IDataEntitySimplexProperty)found).Sequence;

				if(sequence == null)
					throw new ArgumentException($"The '{found.Name}' property specified in the sequence key is undefined.");

				return this.GetSequenceKey(sequence, data);
			}

			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private string GetSequenceKey(Metadata.IDataSequence sequence, object data)
			{
				var key = SEQUENCE_KEY + sequence.Property.Entity.Name + "." + sequence.Property.Name;

				if(sequence.References != null && sequence.References.Length > 0)
				{
					if(data == null)
						throw new InvalidOperationException($"Missing required references data for the '{sequence.Name}' sequence.");

					var index = 0;
					object value = null;

					foreach(var reference in sequence.References)
					{
						switch(data)
						{
							case IModel model:
								if(!model.TryGetValue(reference.Name, out value) || value == null)
									throw new InvalidOperationException($"The required '{reference.Name}' reference of sequence is not included in the data.");

								break;
							case IDictionary<string, object> genericDictionary:
								if(!genericDictionary.TryGetValue(reference.Name, out value) || value == null)
									throw new InvalidOperationException($"The required '{reference.Name}' reference of sequence is not included in the data.");

								break;
							case IDictionary classicDictionary:
								if(!classicDictionary.Contains(reference.Name) || value == null)
									throw new InvalidOperationException($"The required '{reference.Name}' reference of sequence is not included in the data.");

								break;
							default:
								if(Zongsoft.Common.TypeExtension.IsScalarType(data.GetType()))
								{
									if(data.GetType().IsArray)
										value = ((Array)data).GetValue(index) ?? throw new InvalidOperationException($"The required '{reference.Name}' reference of sequence is not included in the data.");
									else
										value = data.ToString();
								}
								else
								{
									if(Reflection.Reflector.GetValue(data, reference.Name) == null)
										throw new InvalidOperationException($"The required '{reference.Name}' reference of sequence is not included in the data.");
								}

								break;
						}

						if(index++ == 0)
							key += ":";
						else
							key += "-";

						key += value.ToString().Trim();
					}
				}

				return key;
			}
			#endregion
		}
		#endregion
	}
}
