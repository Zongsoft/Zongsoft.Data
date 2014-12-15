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

		internal FromClause GetSelectFromClause(string qualifiedName, string scope)
		{
			if(string.IsNullOrWhiteSpace(qualifiedName))
				throw new ArgumentNullException("qualifiedName");

			var entity = MetadataManager.Default.GetConceptElement<MetadataEntity>(qualifiedName);

			if(entity == null)
				throw new DataException(string.Format("Not found the concept entity with '{0}'.", qualifiedName));

			var index = 1;
			var inherits = DataUtility.GetInherits(entity);
			var from = new FromClause(inherits.Pop(), index++);

			while(inherits.Count > 0)
			{
				var current = inherits.Pop();
				from.Joins.Add(new JoinClauseEx(index++, current, current.Key, from.Entity.Key));
			}

			var members = DataUtility.ResolveScope(entity, scope);

			foreach(var member in members)
			{
				var parts = member.Split('.');

				for(int i = 0; i < parts.Length; i++)
				{
					var property = entity.GetProperty(parts[i]) as MetadataEntityComplexProperty;

					if(property != null && (property.Relationship.IsOneToOne() || property.Relationship.IsManyToOne()))
					{
						var memberName = string.Join(".", parts, 0, i);
						var join = this.FindJoinClause(from.Joins, jc => jc.NavigationPropertyName == memberName);

						if(join == null)
						{
							join = new JoinClauseEx(index++,
								property.Relationship.GetToEntity(),
								property.Relationship.GetToEntityReferences(),
								property.Relationship.GetFromEntityReferences())
							{
								NavigationPropertyName = memberName,
							};

							if(from.Entity == property.Relationship.GetFromEntity())
							{
								from.Joins.Add(join);
							}
							else
							{
								var parent = this.FindJoinClause(from.Joins, jc => jc.Entity == property.Relationship.GetFromEntity());

								if(parent == null)
									parent.Children.Add(join);
							}
						}
					}
				}
			}

			return from;
		}

		private JoinClauseEx FindJoinClause(IEnumerable<JoinClauseEx> joins, Predicate<JoinClauseEx> predicate)
		{
			if(predicate == null)
				return null;

			foreach(var join in joins)
			{
				if(predicate(join))
					return join;

				var result = FindJoinClause(join.Children, predicate);

				if(result != null)
					return result;
			}

			return null;
		}

		public class FromClause
		{
			#region 成员字段
			private string _alias;
			private MetadataEntity _entity;
			private List<JoinClauseEx> _joins;
			#endregion

			#region 构造函数
			public FromClause(MetadataEntity entity, int aliasId)
			{
				_entity = entity;
				_alias = "t" + aliasId.ToString();
			}
			#endregion

			#region 公共属性
			public string Alias
			{
				get
				{
					return _alias;
				}
			}

			public MetadataEntity Entity
			{
				get
				{
					return _entity;
				}
			}

			public IList<JoinClauseEx> Joins
			{
				get
				{
					if(_joins == null)
						System.Threading.Interlocked.CompareExchange(ref _joins, new List<JoinClauseEx>(), null);

					return _joins;
				}
			}
			#endregion
		}

		public class NavigationPropertyJoinClause
		{
			private MetadataEntityComplexProperty _property;
			private MetadataAssociation _association;
			private MetadataAssociationEnd _from;
			private MetadataAssociationEnd _to;
		}

		public class JoinClauseEx
		{
			#region 成员字段
			private string _alias;
			private MetadataEntity _entity;
			private IList<MetadataEntityProperty> _refs;
			private IList<MetadataEntityProperty> _dependentRefs;
			private string _navigationPropertyName;
			private IList<JoinClauseEx> _children;
			#endregion

			#region 构造函数
			public JoinClauseEx(int aliasId, MetadataEntity entity, IList<MetadataEntityProperty> refs, IList<MetadataEntityProperty> dependentRefs)
			{
				_alias = "t" + aliasId.ToString();
				_entity = entity;
				_refs = refs;
				_dependentRefs = dependentRefs;
			}

			public JoinClauseEx(string alias, MetadataEntity entity, IList<MetadataEntityProperty> refs, IList<MetadataEntityProperty> dependentRefs)
			{
				_alias = alias;
				_entity = entity;
				_refs = refs;
				_dependentRefs = dependentRefs;
			}
			#endregion

			public string Alias
			{
				get
				{
					return _alias;
				}
			}

			public MetadataEntity Entity
			{
				get
				{
					return _entity;
				}
			}

			public IList<MetadataEntityProperty> Refs
			{
				get
				{
					return _refs;
				}
			}

			public IList<MetadataEntityProperty> DependentRefs
			{
				get
				{
					return _dependentRefs;
				}
			}

			public string NavigationPropertyName
			{
				get
				{
					return _navigationPropertyName;
				}
				set
				{
					_navigationPropertyName = value;
				}
			}

			public IList<JoinClauseEx> Children
			{
				get
				{
					if(_children == null)
						System.Threading.Interlocked.CompareExchange(ref _children, new List<JoinClauseEx>(), null);

					return _children;
				}
			}
		}

		public class JoinClauseNew
		{
			public JoinClauseNew(string principalAlias, string dependentAlias, IEnumerable<string> principalKeys, IEnumerable<string> dependentKeys)
			{
			}

			public string PrincipalAlias
			{
				get;
				set;
			}

			public string DependentAlias
			{
				get;
				set;
			}

			public IEnumerable<string> PrincipalKeys
			{
				get;
				set;
			}

			public IEnumerable<string> DependentKeys
			{
				get;
				set;
			}
		}

		public class JoinClause
		{
			#region 成员字段
			private string _principalAlias;
			private string _dependentAlias;
			private MetadataEntity _principalEntity;
			private MetadataEntity _dependentEntity;
			private IEnumerable<MetadataEntityProperty> _principalProperties;
			private IEnumerable<MetadataEntityProperty> _dependentProperties;
			#endregion

			#region 构造函数
			internal JoinClause(string principalAlias, MetadataEntity principalEntity,
			                    IEnumerable<MetadataEntityProperty> principalProperties,
			                    string dependentAlias, MetadataEntity dependentEntity,
			                    IEnumerable<MetadataEntityProperty> dependentProperties)
			{
				_principalAlias = principalAlias;
				_dependentAlias = dependentAlias;
				_principalEntity = principalEntity;
				_dependentEntity = dependentEntity;
				_principalProperties = principalProperties;
				_dependentProperties = dependentProperties;
			}
			#endregion

			#region 公共属性
			public string PrincipalAlias
			{
				get
				{
					return _principalAlias;
				}
			}

			public string DependentAlias
			{
				get
				{
					return _dependentAlias;
				}
			}

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

		private void GenerateSelectFromClause(StringBuilder text, FromClause from)
		{
			if(from == null)
				return;

			var mapping = MetadataManager.Default.GetMapping(from.Entity.QualifiedName) as MetadataMappingEntity;
			var tableName = mapping.StorageEntity.GetAttributeValue("name");

			text.AppendFormat("FROM {0} AS {1}", tableName, from.Alias);
			text.AppendLine();

			foreach(var join in from.Joins)
			{
				text.AppendFormat(@"\tLEFT JOIN {0} AS {1} ON", join.Entity, join.Alias);
				text.AppendLine();

				for(int i = 0; i < join.DependentRefs.Count; i++)
				{
					text.AppendFormat("", join.Alias, join.DependentRefs[i].Name, join.Refs[i].Name);
				}
			}
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
