/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text;
using System.Xml;

namespace Zongsoft.Data.Metadata
{
	public class MetadataResolver : IMetadataResolver
	{
		#region 静态字段
		private static IMetadataResolver _default;
		#endregion

		#region 常量定义
		public const string XML_DEFAULT_NAMESPACE_URI = "http://schemas.zongsoft.com/data";
		public const string XML_STORAGE_NAMESPACE_URI = "http://schemas.zongsoft.com/data/storage";
		public const string XML_CONCEPT_NAMESPACE_URI = "http://schemas.zongsoft.com/data/concept";

		private const string XML_SCHEMA_ELEMENT = "schema";
		private const string XML_CONTAINER_ELEMENT = "container";
		private const string XML_ENTITY_ELEMENT = "entity";
		private const string XML_KEY_ELEMENT = "key";
		private const string XML_PROPERTYREF_ELEMENT = "propertyRef";
		private const string XML_PROPERTY_ELEMENT = "property";
		private const string XML_COMPLEXPROPERTY_ELEMENT = "complexProperty";
		private const string XML_COMMAND_ELEMENT = "command";
		private const string XML_PARAMETER_ELEMENT = "parameter";
		private const string XML_TEXT_ELEMENT = "text";
		private const string XML_ASSOCIATION_ELEMENT = "association";
		private const string XML_END_ELEMENT = "end";
		private const string XML_CONSTRAINTS_ELEMENT = "constraints";
		private const string XML_CONSTRAINT_ELEMENT = "constraint";
		private const string XML_MAPPINGS_ELEMENT = "mappings";
		private const string XML_MAPPING_ELEMENT = "mapping";
		private const string XML_SELECT_ELEMENT = "select";
		private const string XML_DELETE_ELEMENT = "delete";
		private const string XML_INSERT_ELEMENT = "insert";
		private const string XML_UPDATE_ELEMENT = "update";

		/// <summary>name: 表示元素的名称。</summary>
		private const string XML_NAME_ATTRIBUTE = "name";
		/// <summary>type: 表示元素的类型。</summary>
		private const string XML_TYPE_ATTRIBUTE = "type";
		/// <summary>inherits: 表示当前实体继承的父实体名。</summary>
		private const string XML_INHERITS_ATTRIBUTE = "inherits";
		/// <summary>length: 表示字符串类型属性的长度。</summary>
		private const string XML_LENGTH_ATTRIBUTE = "length";
		/// <summary>nullable: 表示属性值是否可以为空。</summary>
		private const string XML_NULLABLE_ATTRIBUTE = "nullable";
		/// <summary>precision: 表示数字属性精度。</summary>
		private const string XML_PRECISION_ATTRIBUTE = "precision";
		/// <summary>scale: 表示数字属性的小数位数量。</summary>
		private const string XML_SCALE_ATTRIBUTE = "scale";
		/// <summary>direction: 表示命令参数的传递方向。</summary>
		private const string XML_DIRECTION_ATTRIBUTE = "direction";
		/// <summary>resultType: 表示命令的返回类型。</summary>
		private const string XML_RESULTTYPE_ATTRIBUTE = "resultType";
		/// <summary>relationship: 表示导航(复合)属性的关系名。</summary>
		private const string XML_RELATIONSHIP_ATTRIBUTE = "relationship";
		/// <summary>from: 表示导航(复合)属性对应关系的起始顶点。</summary>
		private const string XML_FROM_ATTRIBUTE = "from";
		/// <summary>to: 表示导航(复合)属性对应关系的结束顶点。</summary>
		private const string XML_TO_ATTRIBUTE = "to";
		/// <summary>entity: 表示关系顶点或映射对应的实体。</summary>
		private const string XML_ENTITY_ATTRIBUTE = "entity";
		/// <summary>command: 表示映射对应的命令。</summary>
		private const string XML_COMMAND_ATTRIBUTE = "command";
		/// <summary>mappedTo: 表示命令参数的映射路径。</summary>
		private const string XML_MAPPEDTO_ATTRIBUTE = "mappedTo";
		/// <summary>multiplicity: 表示关系顶点的多重性。</summary>
		private const string XML_MULTIPLICITY_ATTRIBUTE = "multiplicity";
		/// <summary>propertyName: 表示约束或命令参数映射中对应的属性名。</summary>
		private const string XML_PROPERTYNAME_ATTRIBUTE = "propertyName";
		/// <summary>value: 表示约束的值。</summary>
		private const string XML_VALUE_ATTRIBUTE = "value";
		/// <summary>operator: 表示约束的操作符。</summary>
		private const string XML_OPERATOR_ATTRIBUTE = "operator";
		/// <summary>field: 表示属性映射中对应的字段名。</summary>
		private const string XML_FIELD_ATTRIBUTE = "field";
		#endregion

		#region 构造函数
		protected MetadataResolver()
		{
		}
		#endregion

		#region 静态属性
		public static IMetadataResolver Default
		{
			get
			{
				if(_default == null)
					System.Threading.Interlocked.CompareExchange(ref _default, new MetadataResolver(), null);

				return _default;
			}
			set
			{
				_default = value;
			}
		}
		#endregion

		#region 公共方法
		public MetadataFile Resolve(string filePath)
		{
			using(var reader = this.CreateReader((settings, context) => XmlReader.Create(filePath, settings, context)))
			{
				return this.Resolve(reader, filePath);
			}
		}

		public MetadataFile Resolve(Stream stream)
		{
			using(var reader = this.CreateReader((settings, context) => XmlReader.Create(stream, settings, context)))
			{
				return this.Resolve(reader);
			}
		}

		public MetadataFile Resolve(TextReader reader)
		{
			using(var xmlReader = this.CreateReader((settings, context) => XmlReader.Create(reader, settings, context)))
			{
				return this.Resolve(xmlReader);
			}
		}

		public MetadataFile Resolve(XmlReader reader)
		{
			return this.Resolve(reader, null);
		}

		public MetadataFile Resolve(XmlReader reader, string url)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			if(string.IsNullOrWhiteSpace(url))
				url = reader.BaseURI;

			if(reader.MoveToContent() == XmlNodeType.Element)
			{
				if(reader.Name != XML_SCHEMA_ELEMENT)
					throw new MetadataException(string.Format("The root element must be '<{0}>' in this '{1}' file.", XML_SCHEMA_ELEMENT, url));
			}

			var file = new MetadataFile(url);

			while(reader.Read() && reader.NodeType == XmlNodeType.Element)
			{
				switch(reader.LocalName)
				{
					case XML_CONTAINER_ELEMENT:
						this.ResolveContainer(reader, file);
						break;
					case XML_MAPPINGS_ELEMENT:
						this.ResolveMappings(reader, file);
						break;
					default:
						this.ProcessUnrecognizedElement(reader, file, null);
						break;
				}
			}

			return file;
		}
		#endregion

		#region 解析方法
		private void ResolveContainer(XmlReader reader, MetadataFile file)
		{
			if(reader.ReadState == ReadState.Initial)
				reader.Read();

			if(reader.NodeType != XmlNodeType.Element)
				throw new InvalidOperationException("The reader position is invalid.");

			switch(reader.NamespaceURI.ToLowerInvariant())
			{
				case XML_STORAGE_NAMESPACE_URI:
					var storageContainer = new MetadataStorageContainer(reader.GetAttribute(XML_NAME_ATTRIBUTE), reader.GetAttribute("provider"), file);
					file.Storages.Add(storageContainer);

					using(reader = reader.ReadSubtree())
					{
						this.ResolveContainer(reader, storageContainer, MetadataElementKind.Storage);
					}

					break;
				case XML_CONCEPT_NAMESPACE_URI:
					var conceptContainer = new MetadataConceptContainer(reader.GetAttribute(XML_NAME_ATTRIBUTE), file);
					file.Concepts.Add(conceptContainer);

					using(reader = reader.ReadSubtree())
					{
						this.ResolveContainer(reader, conceptContainer, MetadataElementKind.Concept);
					}

					break;
				default:
					using(reader = reader.ReadSubtree())
					{
						this.ProcessUnrecognizedElement(reader, file, null);
					}

					break;
			}
		}

		private void ResolveContainer(XmlReader reader, MetadataConceptContainer container, MetadataElementKind kind)
		{
			if(reader.ReadState == ReadState.Initial)
				reader.Read();

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				switch(reader.Name)
				{
					case XML_ENTITY_ELEMENT:
						this.ResolveEntity(reader, container);
						break;
					case XML_ASSOCIATION_ELEMENT:
						if(container is MetadataConceptContainer conceptContainer)
							this.ResolveAssociation(reader, conceptContainer);
						else
							this.ProcessUnrecognizedElement(reader, container.File, container);
						break;
					case XML_COMMAND_ELEMENT:
						this.ResolveCommand(reader, container);
						break;
					default:
						this.ProcessUnrecognizedElement(reader, container.File, container);
						break;
				}
			}
		}

		private void ResolveEntity(XmlReader reader, MetadataConceptContainer container)
		{
			//创建实体元素对象
			var entity = new MetadataEntity(reader.GetAttribute(XML_NAME_ATTRIBUTE))
			{
				BaseEntityName = reader.GetAttribute(XML_INHERITS_ATTRIBUTE)
			};

			//处理实体元素的所有其他未知属性
			this.ProcessAttributes(reader, entity, (element, prefix, localName, value, namespaceUri) =>
			{
				return (localName == XML_NAME_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri)) ||
				       (localName == XML_INHERITS_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri));
			});

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				switch(reader.Name)
				{
					case XML_KEY_ELEMENT:
						while(reader.Read() && reader.Depth > depth + 1)
						{
							if(reader.NodeType != XmlNodeType.Element)
								continue;

							if(reader.Name == XML_PROPERTYREF_ELEMENT)
								entity.SetKey(reader.GetAttribute(XML_NAME_ATTRIBUTE));
							else
								this.ProcessUnrecognizedElement(reader, null, container);
						}

						break;
					case XML_PROPERTY_ELEMENT:
						var property = new MetadataEntitySimplexProperty(reader.GetAttribute(XML_NAME_ATTRIBUTE), reader.GetAttribute(XML_TYPE_ATTRIBUTE))
						{
							Length = this.GetAttributeValue<int>(reader, XML_LENGTH_ATTRIBUTE),
							Nullable = this.GetAttributeValue<bool>(reader, XML_NULLABLE_ATTRIBUTE),
							Precision = this.GetAttributeValue<byte>(reader, XML_PRECISION_ATTRIBUTE),
							Scale = this.GetAttributeValue<byte>(reader, XML_SCALE_ATTRIBUTE),
						};

						//处理属性元素的所有其他未知属性
						this.ProcessAttributes(reader, property, (element, prefix, localName, value, namespaceUri) =>
						{
							return (localName == XML_LENGTH_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri)) ||
							       (localName == XML_NULLABLE_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri)) ||
							       (localName == XML_PRECISION_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri)) ||
							       (localName == XML_SCALE_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri));
						});

						//将解析成功的属性元素加入到实体的属性集合
						entity.Properties.Add(property);

						break;
					case XML_COMPLEXPROPERTY_ELEMENT:
						var complexProperty = new MetadataEntityComplexProperty(
						                          reader.GetAttribute(XML_NAME_ATTRIBUTE),
						                          reader.GetAttribute(XML_RELATIONSHIP_ATTRIBUTE),
						                          reader.GetAttribute(XML_FROM_ATTRIBUTE),
						                          reader.GetAttribute(XML_TO_ATTRIBUTE));

						//将解析成功的属性元素加入到实体的属性集合
						entity.Properties.Add(complexProperty);

						break;
					default:
						this.ProcessUnrecognizedElement(reader, null, container);
						break;
				}
			}

			container.Entities.Add(entity);
		}

		private void ResolveCommand(XmlReader reader, MetadataConceptContainer container)
		{
			//创建命令元素对象
			var command = new MetadataCommand(reader.GetAttribute(XML_NAME_ATTRIBUTE));

			//获取返回类型的名称
			var resultType = this.GetAttributeValue<string>(reader, XML_RESULTTYPE_ATTRIBUTE);

			//如果指定的返回类型名不为空则解析它
			if(!string.IsNullOrWhiteSpace(resultType))
				command.ResultType = Zongsoft.Common.TypeExtension.GetType(resultType);

			//处理命令元素的所有其他未知属性
			this.ProcessAttributes(reader, command, (element, prefix, localName, value, namespaceUri) =>
			{
				return ((localName == XML_NAME_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri)) ||
				       (localName == XML_RESULTTYPE_ATTRIBUTE && string.IsNullOrEmpty(namespaceUri)));
			});

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				switch(reader.Name)
				{
					case XML_PARAMETER_ELEMENT:
						var parameter = new MetadataCommandParameter(reader.GetAttribute(XML_NAME_ATTRIBUTE), reader.GetAttribute(XML_TYPE_ATTRIBUTE))
						{
							Direction = this.GetAttributeValue<MetadataCommandParameterDirection>(reader, XML_DIRECTION_ATTRIBUTE, MetadataCommandParameterDirection.In),
						};

						//将解析成功的命令参数元素加入到命令的参数集合
						command.Parameters.Add(parameter);

						break;
					case XML_TEXT_ELEMENT:
						if(reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
							command.Text = reader.Value;

						break;
					default:
						this.ProcessUnrecognizedElement(reader, null, container);
						break;
				}
			}

			container.Commands.Add(command);
		}

		private void ResolveAssociation(XmlReader reader, MetadataConceptContainer container)
		{
			//创建关系元素对象
			var association = new MetadataAssociation(reader.GetAttribute(XML_NAME_ATTRIBUTE));

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				if(reader.Name == XML_END_ELEMENT)
				{
					var member = new MetadataAssociationEnd(reader.GetAttribute(XML_NAME_ATTRIBUTE), reader.GetAttribute(XML_ENTITY_ATTRIBUTE))
					{
						Multiplicity = this.GetAttributeValue<MetadataAssociationMultiplicity>(reader, XML_MULTIPLICITY_ATTRIBUTE, MetadataAssociationMultiplicity.One),
					};

					//解析当前关系顶点的内部元素
					this.ResolveAssociationEnd(reader, member);

					//将解析成功的命令参数元素加入到命令的参数集合
					association.Members.Add(member);
				}
				else
				{
					this.ProcessUnrecognizedElement(reader, null, container);
				}
			}

			container.Associations.Add(association);
		}

		private void ResolveAssociationEnd(XmlReader reader, MetadataAssociationEnd member)
		{
			int depth = reader.Depth;

			var properties = new List<string>();

			//解析关系顶点下的成员元素
			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				switch(reader.Name)
				{
					case XML_PROPERTYREF_ELEMENT:
						var propertyName = this.GetAttributeValue<string>(reader, XML_NAME_ATTRIBUTE, null);

						if(!string.IsNullOrWhiteSpace(propertyName))
							properties.Add(propertyName);
						break;
					case XML_CONSTRAINTS_ELEMENT:
						while(reader.Read() && reader.Depth > depth + 1)
						{
							if(reader.NodeType != XmlNodeType.Element)
								continue;

							if(reader.Name == XML_CONSTRAINT_ELEMENT)
							{
								var constraint = new MetadataAssociationEndConstraint(reader.GetAttribute(XML_PROPERTYNAME_ATTRIBUTE),
								                                                     reader.GetAttribute(XML_VALUE_ATTRIBUTE),
								                                                     this.GetAttributeValue<ConditionOperator>(reader, XML_OPERATOR_ATTRIBUTE, ConditionOperator.Equal));

								member.Constraints.Add(constraint);
							}
							else
							{
								this.ProcessUnrecognizedElement(reader, null, member);
							}
						}

						break;
					default:
						this.ProcessUnrecognizedElement(reader, null, member);
						break;
				}
			}

			if(properties.Count > 0)
				member.Properties = properties.ToArray();
		}

		private void ResolveMappings(XmlReader reader, MetadataFile file)
		{
			if(reader.ReadState == ReadState.Initial)
				reader.Read();

			if(reader.NodeType != XmlNodeType.Element)
				throw new InvalidOperationException("The reader position is invalid.");

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				MetadataMapping mapping = null;

				var conceptName = reader.GetAttribute(XML_ENTITY_ATTRIBUTE, XML_CONCEPT_NAMESPACE_URI);
				var storageName = reader.GetAttribute(XML_ENTITY_ATTRIBUTE, XML_STORAGE_NAMESPACE_URI);

				if(conceptName != null && storageName != null)
				{
					mapping = new MetadataMappingEntity(file, conceptName, storageName);
					this.ResolveEntityMapping(reader, (MetadataMappingEntity)mapping);
				}
				else
				{
					conceptName = reader.GetAttribute(XML_COMMAND_ATTRIBUTE, XML_CONCEPT_NAMESPACE_URI);
					storageName = reader.GetAttribute(XML_COMMAND_ATTRIBUTE, XML_STORAGE_NAMESPACE_URI);

					if(conceptName != null && storageName != null)
					{
						mapping = new MetadataMappingCommand(file, conceptName, storageName);
						this.ResolveCommandMapping(reader, (MetadataMappingCommand)mapping);
					}
				}

				if(mapping == null)
					throw new MetadataException(string.Format("Invalid mapping element in this '{0}' file.", file.Url));

				file.Mappings.Add(mapping);
			}
		}

		private void ResolveEntityMapping(XmlReader reader, MetadataMappingEntity mapping)
		{
			if(reader.ReadState == ReadState.Initial)
				reader.Read();

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				switch(reader.Name)
				{
					case XML_PROPERTY_ELEMENT:
						var property = new MetadataMappingEntityProperty(
						                   reader.GetAttribute(XML_NAME_ATTRIBUTE),
						                   reader.GetAttribute(XML_FIELD_ATTRIBUTE));

						mapping.Properties.Add(property);
						break;
					case XML_DELETE_ELEMENT:
						mapping.DeleteCommand = this.ResolveEntityCommand(reader, mapping);
						break;
					case XML_INSERT_ELEMENT:
						mapping.InsertCommand = this.ResolveEntityCommand(reader, mapping);
						break;
					case XML_UPDATE_ELEMENT:
						mapping.UpdateCommand = this.ResolveEntityCommand(reader, mapping);
						break;
					case XML_SELECT_ELEMENT:
						mapping.SelectCommand = this.ResolveEntityCommand(reader, mapping);
						break;
					default:
						this.ProcessUnrecognizedElement(reader, null, mapping);
						break;
				}
			}
		}

		private MetadataMappingEntity.CommandElement ResolveEntityCommand(XmlReader reader, MetadataMappingEntity mapping)
		{
			var commandElement = mapping.CreateCommandElement(reader.GetAttribute(XML_NAME_ATTRIBUTE));

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				if(reader.Name == XML_PARAMETER_ELEMENT)
				{
					commandElement.Parameters.Add(new MetadataMappingEntity.CommandParameterElement(reader.GetAttribute(XML_NAME_ATTRIBUTE), reader.GetAttribute(XML_MAPPEDTO_ATTRIBUTE)));
				}
				else
				{
					this.ProcessUnrecognizedElement(reader, null, commandElement);
				}
			}

			return commandElement;
		}

		private void ResolveCommandMapping(XmlReader reader, MetadataMappingCommand mapping)
		{
			if(reader.ReadState == ReadState.Initial)
				reader.Read();

			int depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				if(reader.Name == XML_PARAMETER_ELEMENT)
				{
					var parameter = new MetadataMappingCommandParameter(
					                    reader.GetAttribute(XML_NAME_ATTRIBUTE),
					                    reader.GetAttribute(XML_MAPPEDTO_ATTRIBUTE));

					mapping.Parameters.Add(parameter);
				}
				else
				{
					this.ProcessUnrecognizedElement(reader, null, mapping);
				}
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual XmlReader CreateReader(Func<XmlReaderSettings, XmlParserContext, XmlReader> tunk)
		{
			var nameTable = this.CreateXmlNameTable();

			var settings = new XmlReaderSettings()
			{
				CloseInput = false,
				IgnoreComments = true,
				IgnoreWhitespace = true,
				ValidationType = ValidationType.None,
				NameTable = nameTable,
			};

			var namespaceManager = new XmlNamespaceManager(nameTable);

			namespaceManager.AddNamespace(string.Empty, XML_DEFAULT_NAMESPACE_URI);
			namespaceManager.AddNamespace("concept", XML_CONCEPT_NAMESPACE_URI);
			namespaceManager.AddNamespace("storage", XML_STORAGE_NAMESPACE_URI);

			var context = new XmlParserContext(nameTable, namespaceManager, XML_SCHEMA_ELEMENT, XmlSpace.None);

			return tunk(settings, context);
		}

		protected virtual bool OnUnrecognizedElement(XmlReader reader, MetadataFile file, object container)
		{
			return false;
		}
		#endregion

		#region 私有方法
		private T GetAttributeValue<T>(XmlReader reader, string name, T defaultValue = default(T))
		{
			string elementName = reader.NodeType == XmlNodeType.Element ? reader.Name : string.Empty;

			if(reader.MoveToAttribute(name))
			{
				//首先获取当前特性的文本值
				object attributeValue = reader.Value.Trim();

				//将读取器的指针移到当前特性所属的元素
				reader.MoveToElement();

				if(typeof(T) == typeof(string))
					return (T)attributeValue;

				T result;

				//为指定名称的特性值做类型转换，如果转换失败则抛出异常
				if(!Zongsoft.Common.Convert.TryConvertValue<T>(attributeValue, out result))
					throw new MetadataException(string.Format("Invalid value '{0}' of '{1}' attribute in '{2}' element.", attributeValue, name, elementName));

				return result;
			}

			//返回默认值
			return defaultValue;
		}

		private void ProcessAttributes<TElement>(XmlReader reader, TElement element, Func<TElement, string, string, string, string, bool> predicate) where TElement : MetadataElementBase
		{
			if(reader.NodeType != XmlNodeType.Element || reader.HasAttributes == false)
				return;

			for(int i = 0; i < reader.AttributeCount; i++)
			{
				reader.MoveToAttribute(i);

				if(!predicate(element, reader.Prefix, reader.LocalName, reader.Value, reader.NamespaceURI))
					element.Attributes.Add(new MetadataElementAttribute(reader.Prefix, reader.LocalName, reader.Value, reader.NamespaceURI));
			}

			reader.MoveToElement();
		}

		private void ProcessUnrecognizedElement(XmlReader reader, MetadataFile file, object container)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			string elementName = null;

			if(reader.ReadState == ReadState.Initial && reader.Read())
				elementName = reader.Name;
			else if(reader.NodeType == XmlNodeType.Element)
				elementName = reader.Name;

			if(!this.OnUnrecognizedElement(reader, file, container))
			{
				var filePath = file != null ? file.Url : string.Empty;

				if(string.IsNullOrWhiteSpace(elementName))
					throw new MetadataException(string.Format("Contains unrecognized element(s) in the '{0}' file.", filePath));

				throw new MetadataException(string.Format("Found a unrecognized '{0}' element in the '{1}' file.", elementName, filePath));
			}
		}

		private XmlNameTable CreateXmlNameTable()
		{
			var nameTable = new NameTable();

			nameTable.Add(XML_SCHEMA_ELEMENT);
			nameTable.Add(XML_CONTAINER_ELEMENT);
			nameTable.Add(XML_ENTITY_ELEMENT);
			nameTable.Add(XML_KEY_ELEMENT);
			nameTable.Add(XML_PROPERTYREF_ELEMENT);
			nameTable.Add(XML_PROPERTY_ELEMENT);
			nameTable.Add(XML_COMPLEXPROPERTY_ELEMENT);
			nameTable.Add(XML_COMMAND_ELEMENT);
			nameTable.Add(XML_PARAMETER_ELEMENT);
			nameTable.Add(XML_TEXT_ELEMENT);
			nameTable.Add(XML_ASSOCIATION_ELEMENT);
			nameTable.Add(XML_END_ELEMENT);
			nameTable.Add(XML_CONSTRAINTS_ELEMENT);
			nameTable.Add(XML_CONSTRAINT_ELEMENT);
			nameTable.Add(XML_MAPPINGS_ELEMENT);
			nameTable.Add(XML_MAPPING_ELEMENT);
			nameTable.Add(XML_SELECT_ELEMENT);
			nameTable.Add(XML_DELETE_ELEMENT);
			nameTable.Add(XML_INSERT_ELEMENT);
			nameTable.Add(XML_UPDATE_ELEMENT);

			nameTable.Add(XML_NAME_ATTRIBUTE);
			nameTable.Add(XML_TYPE_ATTRIBUTE);
			nameTable.Add(XML_INHERITS_ATTRIBUTE);
			nameTable.Add(XML_LENGTH_ATTRIBUTE);
			nameTable.Add(XML_NULLABLE_ATTRIBUTE);
			nameTable.Add(XML_DIRECTION_ATTRIBUTE);
			nameTable.Add(XML_RESULTTYPE_ATTRIBUTE);
			nameTable.Add(XML_RELATIONSHIP_ATTRIBUTE);
			nameTable.Add(XML_FROM_ATTRIBUTE);
			nameTable.Add(XML_TO_ATTRIBUTE);
			nameTable.Add(XML_ENTITY_ATTRIBUTE);
			nameTable.Add(XML_COMMAND_ATTRIBUTE);
			nameTable.Add(XML_MAPPEDTO_ATTRIBUTE);
			nameTable.Add(XML_MULTIPLICITY_ATTRIBUTE);
			nameTable.Add(XML_PROPERTYNAME_ATTRIBUTE);
			nameTable.Add(XML_VALUE_ATTRIBUTE);
			nameTable.Add(XML_OPERATOR_ATTRIBUTE);
			nameTable.Add(XML_FIELD_ATTRIBUTE);

			return nameTable;
		}
		#endregion
	}
}
