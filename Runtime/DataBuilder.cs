using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Data.Runtime
{
	public class DataBuilder : IDataBuilder
	{
		#region 私有变量
		private readonly ConcurrentDictionary<DataAccessAction, IDataBuilder> _builders;
		#endregion

		#region 构造函数
		protected DataBuilder()
		{
			_builders = new ConcurrentDictionary<DataAccessAction, IDataBuilder>();
		}
		#endregion

		#region 公共方法
		public DataOperation Build(DataPipelineContext context)
		{
			var builder = this.GetBuilder(context);

			if(builder == null)
				throw new InvalidOperationException("The data builder can not supports the action.");

			return this.OnBuild(builder, context);
		}
		#endregion

		#region 注册方法
		public void Register(DataAccessAction action, IDataBuilder builder)
		{
			if(builder == null)
				((IDictionary<DataAccessAction, IDataBuilder>)_builders).Remove(action);

			_builders.AddOrUpdate(action, builder, (_, __) => builder);
		}
		#endregion

		#region 虚拟方法
		protected virtual IDataBuilder GetBuilder(DataPipelineContext context)
		{
			IDataBuilder result;

			if(_builders.TryGetValue(context.DataExecutorContext.Action, out result))
				return result;

			return null;
		}

		protected virtual DataOperation OnBuild(IDataBuilder builder, DataPipelineContext context)
		{
			return builder.Build(context);
		}
		#endregion
	}
}
