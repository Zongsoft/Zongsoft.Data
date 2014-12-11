using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;

using Zongsoft.Data;
using Zongsoft.Data.Metadata;

namespace Zongsoft.Data
{
	public static class DataUtility
	{
		internal class DataEntry
		{
			#region 成员字段
			private string _tableName;
			private MetadataEntity _conceptEntity;
			private ICollection<string> _conceptProperties;
			private MetadataMappingEntity _mapping;
			#endregion

			#region 构造函数
			public DataEntry(MetadataEntity conceptEntity)
			{
				if(conceptEntity == null)
					throw new ArgumentNullException("conceptEntity");

				if(conceptEntity.Kind != MetadataElementKind.Concept)
					throw new ArgumentException("The kind of entity is not Concept.");

				_conceptEntity = conceptEntity;
				_mapping = MetadataManager.Default.GetMapping(conceptEntity.QualifiedName) as MetadataMappingEntity;

				if(_mapping == null)
					throw new DataException("Missing mapping for concept entity.");
			}
			#endregion

			#region 公共属性
			public MetadataEntity ConceptEntity
			{
				get
				{
					return _conceptEntity;
				}
			}

			public ICollection<string> ConceptProperties
			{
				get
				{
					return _conceptProperties;
				}
				set
				{
					_conceptProperties = value;
				}
			}

			public MetadataMappingEntity Mapping
			{
				get
				{
					return _mapping;
				}
			}

			public MetadataEntity StorageEntity
			{
				get
				{
					return Mapping.StorageEntity;
				}
			}

			public string TableName
			{
				get
				{
					if(string.IsNullOrEmpty(_tableName))
					{
						var entity = this.StorageEntity;

						if(entity == null)
							throw new DataException("The storeage entity is null.");

						_tableName = entity.GetAttributeValue("name", MetadataUri.Storage);

						if(string.IsNullOrWhiteSpace(_tableName))
							_tableName = entity.Name;
					}

					return _tableName;
				}
			}
			#endregion
		}

		internal static IList<DataEntry> GetInherits(string qualifiedName, string scope)
		{
			if(string.IsNullOrWhiteSpace(qualifiedName))
				throw new ArgumentNullException("qualifiedName");

			var currentEntity = MetadataManager.Default.GetConceptElement<MetadataEntity>(qualifiedName);

			if(currentEntity == null)
				throw new DataException(string.Format("Not found the concept entity with '{0}'.", qualifiedName));

			var entries = new Stack<DataEntry>();

			while(currentEntity != null)
			{
				if(entries.Any(entry => entry.ConceptEntity == currentEntity))
					break;

				entries.Push(new DataEntry(currentEntity)
				{
					ConceptProperties = ResolveScope(currentEntity, scope)
				});

				currentEntity = currentEntity.BaseEntity;
			}

			return entries.ToArray();
		}

		internal static IList<DataEntry> GetOnetoOne(MetadataEntity entity, IEnumerable<string> members)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			var entries = new List<DataEntry>();

			foreach(var member in members)
			{
				string propertyName;

				if(member.IndexOf('.') > 0)
					propertyName = member.Substring(0, member.IndexOf('.'));
				else
					propertyName = member;

				var property = entity.Properties[propertyName] as MetadataEntityComplexProperty;

				if(property != null)
				{
					var association = property.Relationship.Association;

					if(association != null && association.IsOneToOne(property.Relationship.From, property.Relationship.To))
					{
						var dependentEntity = association.Members[property.Relationship.To].Entity;

						entries.Add(new DataEntry(dependentEntity)
						{
							//ConceptProperties = ResolveScope(dependentEntity, scope)
						});
					}
				}
			}

			return entries;
		}

		public static ICollection<string> ResolveScope(MetadataEntity entity, string scope)
		{
			if(entity == null)
				return null;

			var properties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var members = scope.Split(',');

			//初始化所有单值属性到哈希集中
			ResetProperties(entity, properties, false);

			if(string.IsNullOrWhiteSpace(scope))
				return properties;

			for(int i = 0; i < members.Length; i++)
			{
				var member = members[i].Trim();

				if(member.Length == 0)
					continue;

				if(member == "*") //包含所有成员
				{
					ResetProperties(entity, properties, true);
				}
				else if(member == "!" || member == "-") //排除所有成员
				{
					properties.Clear();
				}
				else if(member.Length > 1)
				{
					if(member[0] == '-') //排除指定成员
						properties.Remove(member.Substring(1));
					else //包含指定成员
					{
						if(!member.Contains(".") && entity.Properties.Contains(member))
							properties.Add(member);
					}
				}
			}

			return properties;
		}

		private static void ResetProperties(MetadataEntity entity, HashSet<string> properties, bool containsAll)
		{
			foreach(var property in entity.Properties)
			{
				if(containsAll)
					properties.Add(property.Name.Trim());
				else
				{
					if(property is MetadataEntitySimplexProperty)
						properties.Add(property.Name.Trim());
				}
			}
		}
	}
}
