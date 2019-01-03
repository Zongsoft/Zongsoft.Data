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
using System.Collections.Generic;

namespace Zongsoft.Data.Common.Expressions
{
	public abstract class StatementBuilderBase : IStatementBuilder
	{
		#region 私有变量
		private readonly object _syncRoot;
		#endregion

		#region 成员字段
		private IStatementBuilder _count;
		private IStatementBuilder _exist;
		private IStatementBuilder _execution;
		private IStatementBuilder _increment;

		private IStatementBuilder _select;
		private IStatementBuilder _delete;
		private IStatementBuilder _insert;
		private IStatementBuilder _update;
		private IStatementBuilder _upsert;
		#endregion

		#region 构造函数
		protected StatementBuilderBase()
		{
			_syncRoot = new object();
		}
		#endregion

		#region 公共方法
		public virtual IEnumerable<IStatement> Build(IDataAccessContextBase context, IDataSource source)
		{
			IStatementBuilder builder = null;

			switch(context.Method)
			{
				case DataAccessMethod.Select:
					builder = this.GetBuilder(ref _select, () => this.CreateSelectStatementBuilder());
					break;
				case DataAccessMethod.Delete:
					builder = this.GetBuilder(ref _delete, () => this.CreateDeleteStatementBuilder());
					break;
				case DataAccessMethod.Insert:
					builder = this.GetBuilder(ref _insert, () => this.CreateInsertStatementBuilder());
					break;
				case DataAccessMethod.Update:
					builder = this.GetBuilder(ref _update, () => this.CreateUpdateStatementBuilder());
					break;
				case DataAccessMethod.Upsert:
					builder = this.GetBuilder(ref _upsert, () => this.CreateUpsertStatementBuilder());
					break;
				case DataAccessMethod.Count:
					builder = this.GetBuilder(ref _count, () => this.CreateCountStatementBuilder());
					break;
				case DataAccessMethod.Exists:
					builder = this.GetBuilder(ref _exist, () => this.CreateExistStatementBuilder());
					break;
				case DataAccessMethod.Execute:
					builder = this.GetBuilder(ref _execution, () => this.CreateExecutionStatementBuilder());
					break;
				case DataAccessMethod.Increment:
					builder = this.GetBuilder(ref _increment, () => this.CreateIncrementStatementBuilder());
					break;
			}

			if(builder == null)
				throw new DataException("Can not get the statement builder from the context.");

			return builder.Build(context, source);
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private IStatementBuilder GetBuilder(ref IStatementBuilder builder, Func<IStatementBuilder> factory)
		{
			if(builder == null)
			{
				lock(_syncRoot)
				{
					if(builder == null)
						builder = factory();
				}
			}

			return builder;
		}
		#endregion

		#region 抽象方法
		protected abstract IStatementBuilder CreateSelectStatementBuilder();
		protected abstract IStatementBuilder CreateDeleteStatementBuilder();
		protected abstract IStatementBuilder CreateInsertStatementBuilder();
		protected abstract IStatementBuilder CreateUpdateStatementBuilder();
		protected abstract IStatementBuilder CreateUpsertStatementBuilder();

		protected abstract IStatementBuilder CreateCountStatementBuilder();
		protected abstract IStatementBuilder CreateExistStatementBuilder();
		protected abstract IStatementBuilder CreateExecutionStatementBuilder();
		protected abstract IStatementBuilder CreateIncrementStatementBuilder();
		#endregion
	}
}
