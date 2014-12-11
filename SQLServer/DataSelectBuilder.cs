using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zongsoft.Data;
using Zongsoft.Data.Metadata;
using Zongsoft.Data.Runtime;

namespace Zongsoft.Data.SQLServer
{
	public class DataSelectBuilder : IDataBuilder
	{
		public DataOperation Build(DataPipelineContext context)
		{
			if(context.DataExecutorContext.Action != DataAccessAction.Select)
				throw new NotSupportedException("The data builder was supports 'Select' action only.");

			var dataHandler = context.Pipeline.Handler as IDataHandler;
			var manager = context.DataExecutorContext.MetadataManager;
			var parameter = (DataSelectParameter)context.DataExecutorContext.Parameter;

			var concept = manager.GetConceptContainer(parameter.ContainerName, parameter.Namespace);
			var entity = manager.GetConceptElement<MetadataEntity>(parameter.QualifiedName);
			var mapping = manager.GetMapping(parameter.QualifiedName) as MetadataMappingEntity;
			var mappedEntity = mapping.StorageEntity;
			var mappedEntityBase = mappedEntity.BaseEntity;

			return null;
		}

		internal IList<Join> GetSelectJoins(string qualifiedName, string scope)
		{
			if(string.IsNullOrWhiteSpace(qualifiedName))
				throw new ArgumentNullException("qualifiedName");

			var currentEntity = MetadataManager.Default.GetConceptElement<MetadataEntity>(qualifiedName);

			if(currentEntity == null)
				throw new DataException(string.Format("Not found the concept entity with '{0}'.", qualifiedName));

			var stack = new Stack<MetadataEntity>();

			while(currentEntity != null)
			{
				if(stack.Contains(currentEntity))
					break;

				stack.Push(currentEntity);
				currentEntity = currentEntity.BaseEntity;
			}

			var joins = new List<Join>();
			var principalEntity = stack.Pop();

			while(stack.Count > 0)
			{
				var dependentEntity = stack.Pop();
				joins.Add(new Join(principalEntity, dependentEntity, principalEntity.Key, dependentEntity.Key));
			}

			return joins;
		}

		public class Join
		{
			#region 成员字段
			private MetadataEntity _principalEntity;
			private MetadataEntity _dependentEntity;
			private IEnumerable<MetadataEntityProperty> _principalProperties;
			private IEnumerable<MetadataEntityProperty> _dependentProperties;
			#endregion

			#region 构造函数
			internal Join(MetadataEntity principalEntity, MetadataEntity dependentEntity, IEnumerable<MetadataEntityProperty> principalProperties, IEnumerable<MetadataEntityProperty> dependentProperties)
			{
				_principalEntity = principalEntity;
				_dependentEntity = dependentEntity;
				_principalProperties = principalProperties;
				_dependentProperties = dependentProperties;
			}
			#endregion

			#region 公共属性
			public MetadataEntity PrincipalEntity
			{
				get
				{
					return _principalEntity;
				}
			}

			public MetadataEntity DependentEntity
			{
				get
				{
					return _dependentEntity;
				}
			}

			public IEnumerable<MetadataEntityProperty> PrincipalProperties
			{
				get
				{
					return _principalProperties;
				}
			}

			public IEnumerable<MetadataEntityProperty> DependentProperties
			{
				get
				{
					return _dependentProperties;
				}
			}
			#endregion
		}

		private void GenerateSelectFromClause(StringBuilder text, IList<DataUtility.DataEntry> entries)
		{
			for(int i = 0; i < entries.Count; i++)
			{
				var currentEntry = entries[i];

				if(i > 0)
					text.AppendFormat(Environment.NewLine + "\tLEFT JOIN ");

				text.AppendFormat("{0} AS t{1}", currentEntry.TableName, i + 1);

				if(i > 0)
				{
					text.AppendLine(" ON");

					var previousEntry = entries[i - 1];
					int count = previousEntry.StorageEntity.Key.Length;

					for(int j = 0; j < count; j++)
					{
						text.AppendFormat("\t\tt{0}.[{1}] = t{2}.[{3}]",
										  i, previousEntry.StorageEntity.Key[j].Name,
										  i + 1, currentEntry.StorageEntity.Key[j].Name);

						if(j < count - 1)
							text.AppendLine(" AND");
						else
							text.AppendLine();
					}
				}
			}
		}

		public string BuildCore(DataExecutorContext context)
		{
			var text = new StringBuilder();
			var parameter = context.Parameter as DataSelectParameter;
			var entries = DataUtility.GetInherits(parameter.QualifiedName, parameter.Scope).ToArray();
			var list = new List<DataUtility.DataEntry>(entries);

			foreach(var entry in entries)
			{
				var joins = DataUtility.GetOnetoOne(entry.ConceptEntity, entry.ConceptProperties);
				list.AddRange(joins);
			}

			this.GenerateSelectFromClause(text, list);

			return text.ToString();
		}
	}
}
