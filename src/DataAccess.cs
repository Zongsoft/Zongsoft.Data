using System;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Runtime;

namespace Zongsoft.Data
{
	public class DataAccess : DataAccessBase
	{
		#region 成员字段
		private DataExecutor _executor;
		private MetadataManager _modelManager;
		#endregion

		#region 构造函数
		public DataAccess()
		{
		}
		#endregion

		#region 公共属性
		public DataExecutor Executor
		{
			get
			{
				if(_executor == null)
					System.Threading.Interlocked.CompareExchange(ref _executor, new DataExecutor(this), null);

				return _executor;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_executor = value;
			}
		}

		public MetadataManager MetadataManager
		{
			get
			{
				if(_modelManager == null)
					_modelManager = MetadataManager.Default;

				return _modelManager;
			}
		}
		#endregion

		#region 执行方法
		protected override IEnumerable<T> OnExecute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			throw new NotImplementedException();
		}

		protected override object OnExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 计数方法
		protected override int OnCount(string name, ICondition condition, string[] includes)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 删除方法
		protected override int OnDelete(string name, ICondition condition, string[] cascades)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 插入方法
		protected override int OnInsert(string name, DataDictionary data, string scope)
		{
			return base.OnInsert(name, data, scope);
		}

		protected override int OnInsertMany(string name, IEnumerable<DataDictionary> items, string scope)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 更新方法
		protected override int OnUpdate(string name, DataDictionary data, ICondition condition, string scope)
		{
			return base.OnUpdate(name, data, condition, scope);
		}

		protected override int OnUpdateMany(string name, IEnumerable<DataDictionary> items, ICondition condition, string scope)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 查询方法
		protected override IEnumerable<T> OnSelect<T>(string name, ICondition condition, Grouping grouping, string scope, Paging paging, Sorting[] sortings)
		{
			var executor = _executor;

			if(executor == null)
				throw new InvalidOperationException();

			var parameter = new DataSelectParameter(name, condition, scope, paging, sortings);
			var context = new DataExecutorContext(executor, this.MetadataManager, DataAccessAction.Select, parameter);

			return executor.Execute(context) as IEnumerable<T>;
		}
		#endregion

		#region 重写方法
		public override string[] GetKey(string name)
		{
			throw new NotImplementedException();
		}

		public override bool Exists(string name, ICondition condition)
		{
			throw new NotImplementedException();
		}

		public override long Increment(string name, string member, ICondition condition, int interval = 1)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
