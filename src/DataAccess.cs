using System;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Data.Metadata;
using Zongsoft.Data.Runtime;

namespace Zongsoft.Data
{
	public class DataAccess : IDataAccess
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
		public object Execute(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Execute(name, inParameters, out outParameters);
		}

		public object Execute(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 计数方法
		public int Count(string name, ICondition condition, string scope = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 删除方法
		public int Delete(string name, ICondition condition, string scope = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 插入方法
		public int Insert(string name, object entity, string scope = null)
		{
			throw new NotImplementedException();
		}

		public int Insert<T>(string name, IEnumerable<T> entities, string scope = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 更新方法
		public int Update(string name,
		                  object entity,
						  ICondition condition = null,
						  string scope = null)
		{
			throw new NotImplementedException();
		}

		public int Update<T>(string name,
		                     IEnumerable<T> entities,
							 ICondition condition = null,
							 string scope = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 查询方法
		public IEnumerable<T> Select<T>(string name,
		                                ICondition condition = null,
										string scope = null,
										Paging paging = null,
										params Sorting[] sorting)
		{
			throw new NotImplementedException();
		}

		public IEnumerable Select(string name,
		                          ICondition condition = null,
								  string scope = null,
								  Paging paging = null,
								  params Sorting[] sorting)
		{
			var executor = _executor;

			if(executor == null)
				throw new InvalidOperationException();

			var parameter = new DataSelectParameter(name, condition, scope, paging, sorting);
			var context = new DataExecutorContext(executor, this.MetadataManager, DataAccessAction.Select, parameter);

			return executor.Execute(context) as IEnumerable;
		}
		#endregion
	}
}
