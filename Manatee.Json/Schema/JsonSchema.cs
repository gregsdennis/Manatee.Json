using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Serialization;
using Manatee.Json.Internal;
using Manatee.Json.Schema.Validators;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides base functionality for the basic <see cref="IJsonSchema"/> implementations.S
	/// </summary>
	public class JsonSchema : IJsonSchema
	{
		/// <summary>
		/// Defines an empty Schema.  Useful for specifying that any schema is valid.
		/// </summary>
		public static readonly JsonSchema Empty = new JsonSchema();
		/// <summary>
		/// Defines the Draft-04 Schema as presented at http://json-schema.org/draft-04/schema#
		/// </summary>
		public static readonly JsonSchema Draft04 = new JsonSchema
			{
				Type = JsonSchemaTypeDefinition.Object,
				Id = "http://json-schema.org/draft-04/schema#",
				Schema = "http://json-schema.org/draft-04/schema#",
				Description = "Core schema meta-schema",
				Definitions = new JsonSchemaTypeDefinitionCollection
					{
						new JsonSchemaTypeDefinition("schemaArray")
							{
								Definition = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Array,
										MinItems = 1,
										Items = JsonSchemaReference.Root
									},
							},
						new JsonSchemaTypeDefinition("positiveInteger")
							{
								Definition = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Integer,
										Minimum = 0
									},
							},
						new JsonSchemaTypeDefinition("positiveIntegerDefault0")
							{
								Definition = new JsonSchema
									{
										AllOf = new List<IJsonSchema>
											{
												new JsonSchemaReference("#/definitions/positiveInteger"),
												new JsonSchema
													{
														Default = 0
													}
											}
									},
							},
						new JsonSchemaTypeDefinition("simpleTypes")
							{
								Definition = new JsonSchema
									{
										Enum = new List<EnumSchemaValue>
											{
												new EnumSchemaValue("array"),
												new EnumSchemaValue("boolean"),
												new EnumSchemaValue("integer"),
												new EnumSchemaValue("null"),
												new EnumSchemaValue("number"),
												new EnumSchemaValue("object"),
												new EnumSchemaValue("string")
											}
									}
							},
						new JsonSchemaTypeDefinition("stringArray")
							{
								Definition = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Array,
										Items = new JsonSchema {Type = JsonSchemaTypeDefinition.String},
										MinItems = 1,
										UniqueItems = true
									}
							}
					},
				Properties = new JsonSchemaPropertyDefinitionCollection
					{
						new JsonSchemaPropertyDefinition("id")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.String,
										Format = StringFormat.Uri
									}
							},
						new JsonSchemaPropertyDefinition("$schema")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.String,
										Format = StringFormat.Uri
									}
							},
						new JsonSchemaPropertyDefinition("title")
							{
								Type = new JsonSchema {Type = JsonSchemaTypeDefinition.String}
							},
						new JsonSchemaPropertyDefinition("description")
							{
								Type = new JsonSchema {Type = JsonSchemaTypeDefinition.String}
							},
						new JsonSchemaPropertyDefinition("default")
							{
								Type = Empty
							},
						new JsonSchemaPropertyDefinition("multipleOf")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Number,
										Minimum = 0,
										ExclusiveMinimum = true
									}
							},
						new JsonSchemaPropertyDefinition("maximum")
							{
								Type = new JsonSchema {Type = JsonSchemaTypeDefinition.Number}
							},
						new JsonSchemaPropertyDefinition("exclusiveMaximum")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Boolean,
										Default = false
									}
							},
						new JsonSchemaPropertyDefinition("minimum")
							{
								Type = new JsonSchema {Type = JsonSchemaTypeDefinition.Number}
							},
						new JsonSchemaPropertyDefinition("exclusiveMinimum")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Boolean,
										Default = false
									}
							},
						new JsonSchemaPropertyDefinition("maxLength")
							{
								Type = new JsonSchemaReference("#/definitions/positiveInteger")
							},
						new JsonSchemaPropertyDefinition("minLength")
							{
								Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")
							},
						new JsonSchemaPropertyDefinition("pattern")
							{
								Type = new JsonSchema()
									{
										Type = JsonSchemaTypeDefinition.String,
										Format = StringFormat.Regex
									}
							},
						new JsonSchemaPropertyDefinition("additionalItems")
							{
								Type = new JsonSchema
									{
										AnyOf = new List<IJsonSchema>
											{
												new JsonSchema
													{
														Type = JsonSchemaTypeDefinition.Boolean
													},
												JsonSchemaReference.Root
											},
										Default = new JsonObject()
									}
							},
						new JsonSchemaPropertyDefinition("items")
							{
								Type = new JsonSchema
									{
										AnyOf = new List<IJsonSchema>
											{
												JsonSchemaReference.Root,
												new JsonSchemaReference("#/definitions/schemaArray")
											},
										Default = new JsonObject()
									}
							},
						new JsonSchemaPropertyDefinition("maxItems")
							{
								Type = new JsonSchemaReference("#/definitions/positiveInteger")
							},
						new JsonSchemaPropertyDefinition("minItems")
							{
								Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")
							},
						new JsonSchemaPropertyDefinition("uniqueItems")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Boolean,
										Default = false
									}
							},
						new JsonSchemaPropertyDefinition("maxProperties")
							{
								Type = new JsonSchemaReference("#/definitions/positiveInteger")
							},
						new JsonSchemaPropertyDefinition("minProperties")
							{
								Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")
							},
						new JsonSchemaPropertyDefinition("required")
							{
								Type = new JsonSchemaReference("#/definitions/stringArray")
							},
						new JsonSchemaPropertyDefinition("additionalProperties")
							{
								Type = new JsonSchema
									{
										AnyOf = new List<IJsonSchema>
											{
												new JsonSchema
													{
														Type = JsonSchemaTypeDefinition.Boolean
													},
												JsonSchemaReference.Root
											},
										Default = new JsonObject()
									}
							},
						new JsonSchemaPropertyDefinition("definitions")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Object,
										AdditionalProperties = new AdditionalProperties
											{
												Definition = JsonSchemaReference.Root
											},
										Default = new JsonObject()
									}
							},
						new JsonSchemaPropertyDefinition("properties")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Object,
										AdditionalProperties = new AdditionalProperties
											{
												Definition = JsonSchemaReference.Root
											},
										Default = new JsonObject()
									}
							},
						new JsonSchemaPropertyDefinition("patternProperties")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Object,
										AdditionalProperties = new AdditionalProperties
											{
												Definition = JsonSchemaReference.Root
											},
										Default = new JsonObject()
									}
							},
						new JsonSchemaPropertyDefinition("dependencies")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Object,
										AdditionalProperties = new AdditionalProperties
											{
												Definition = new JsonSchema
													{
														AnyOf = new List<IJsonSchema>
															{
																JsonSchemaReference.Root,
																new JsonSchemaReference("#/definitions/stringArray")
															}
													}
											}
									}
							},
						new JsonSchemaPropertyDefinition("enum")
							{
								Type = new JsonSchema
									{
										Type = JsonSchemaTypeDefinition.Array,
										MinItems = 1,
										UniqueItems = true
									}
							},
						new JsonSchemaPropertyDefinition("type")
							{
								Type = new JsonSchema
									{
										AnyOf = new List<IJsonSchema>
											{
												new JsonSchemaReference("#/definitions/simpleTypes"),
												new JsonSchema
													{
														Type = JsonSchemaTypeDefinition.Array,
														Items = new JsonSchemaReference("#/definitions/simpleTypes"),
														MinItems = 1,
														UniqueItems = true
													}
											}
									}
							},
						new JsonSchemaPropertyDefinition("allOf")
							{
								Type = new JsonSchemaReference("#/definitions/schemaArray")
							},
						new JsonSchemaPropertyDefinition("anyOf")
							{
								Type = new JsonSchemaReference("#/definitions/schemaArray")
							},
						new JsonSchemaPropertyDefinition("oneOf")
							{
								Type = new JsonSchemaReference("#/definitions/schemaArray")
							},
						new JsonSchemaPropertyDefinition("not")
							{
								Type = JsonSchemaReference.Root
							},
					},
				Dependencies = new List<IJsonSchemaDependency>
					{
						new PropertyDependency("exclusiveMaximum", "maximum"),
						new PropertyDependency("exclusiveMinimum", "minimum"),
					},
				Default = new JsonObject()
			};

		private static readonly IEnumerable<string> _definedProperties =
			Draft04.Properties.Select(p => p.Name)
			       .Union(new[]
				              {
					              "format",
								  "$ref"
				              }).ToList();

		private string _id;
		private string _schema;
		private double? _multipleOf;

		/// <summary>
		/// Used to specify which this schema defines.
		/// </summary>
		public string Id
		{
			get { return _id; }
			set
			{
				if (!string.IsNullOrWhiteSpace(value) && !StringFormat.Uri.Validate(value))
					throw new ArgumentOutOfRangeException(nameof(Id), "'id' property must be a well-formed URI.");
				_id = value;
			}
		}
		/// <summary>
		/// Used to specify a schema which contains the definitions used by this schema.
		/// </summary>
		/// <remarks>
		/// if left null, the default of http://json-schema.org/draft-04/schema# is used.
		/// </remarks>
		public string Schema
		{
			get { return _schema; }
			set
			{
				if (!string.IsNullOrWhiteSpace(value) && !StringFormat.Uri.Validate(value))
					throw new ArgumentOutOfRangeException(nameof(Schema), "'$schema' property must be a well-formed URI.");
				_schema = value;
			}
		}
		/// <summary>
		/// Defines a title for this schema.
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// Defines a description for this schema.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// The default value for this schema.
		/// </summary>
		/// <remarks>
		/// The default value is defined as a JSON value which may need to be deserialized
		/// to a .Net data structure.
		/// </remarks>
		public JsonValue Default { get; set; }
		/// <summary>
		/// Defines a divisor for acceptable values.
		/// </summary>
		public double? MultipleOf
		{
			get { return _multipleOf; }
			set
			{
				if (value <=0) throw new ArgumentOutOfRangeException(nameof(MultipleOf), "'multipleOf' property must be a positive value.");
				_multipleOf = value;
			}
		}
		/// <summary>
		/// Defines a maximum acceptable value.
		/// </summary>
		public double? Maximum { get; set; }
		/// <summary>
		/// Defines whether the maximum value is itself acceptable.
		/// </summary>
		public bool? ExclusiveMaximum { get; set; }
		/// <summary>
		/// Defines a minimum acceptable value.
		/// </summary>
		public double? Minimum { get; set; }
		/// <summary>
		/// Defines whether the minimum value is itself acceptable.
		/// </summary>
		public bool? ExclusiveMinimum { get; set; }
		/// <summary>
		/// Defines a maximum acceptable length.
		/// </summary>
		public uint? MaxLength { get; set; }
		/// <summary>
		/// Defines a minimum acceptable length.
		/// </summary>
		public uint? MinLength { get; set; }
		/// <summary>
		/// Defines a <see cref="Regex"/> pattern for to which the value must adhere.
		/// </summary>
		public string Pattern { get; set; }
		/// <summary>
		/// Defines any additional items to be expected by this schema.
		/// </summary>
		public AdditionalItems AdditionalItems { get; set; }
		/// <summary>
		/// Defines the schema for the items contained in the array.
		/// </summary>
		public IJsonSchema Items { get; set; }
		/// <summary>
		/// Defines a maximum number of items required for the array.
		/// </summary>
		public uint? MaxItems { get; set; }
		/// <summary>
		/// Gets and sets a minimum number of items required for the array.
		/// </summary>
		public uint? MinItems { get; set; }
		/// <summary>
		/// Defines whether the array should contain only unique items.
		/// </summary>
		public bool? UniqueItems { get; set; }
		/// <summary>
		/// Defines a maximum acceptable length.
		/// </summary>
		public uint? MaxProperties { get; set; }
		/// <summary>
		/// Defines a minimum acceptable length.
		/// </summary>
		public uint? MinProperties { get; set; }
		/// <summary>
		/// Defines any additional properties to be expected by this schema.
		/// </summary>
		public AdditionalProperties AdditionalProperties { get; set; }
		/// <summary>
		/// Defines a collection of schema type definitions.
		/// </summary>
		public JsonSchemaTypeDefinitionCollection Definitions { get; set; }
		/// <summary>
		/// Defines a collection of properties expected by this schema.
		/// </summary>
		public JsonSchemaPropertyDefinitionCollection Properties { get; set; }
		/// <summary>
		/// Defines additional properties based on regular expression matching of the property name.
		/// </summary>
		public Dictionary<Regex, IJsonSchema> PatternProperties { get; set; }
		/// <summary>
		/// Defines property dependencies.
		/// </summary>
		public IEnumerable<IJsonSchemaDependency> Dependencies { get; set; }
		/// <summary>
		/// A collection of acceptable values.
		/// </summary>
		public IEnumerable<EnumSchemaValue> Enum { get; set; }
		/// <summary>
		/// The JSON Schema type which defines this schema.
		/// </summary>
		public JsonSchemaTypeDefinition Type { get; set; }
		/// <summary>
		/// A collection of required schema which must be satisfied.
		/// </summary>
		public IEnumerable<IJsonSchema> AllOf { get; set; }
		/// <summary>
		/// A collection of schema options.
		/// </summary>
		public IEnumerable<IJsonSchema> AnyOf { get; set; }
		/// <summary>
		/// A collection of schema options.
		/// </summary>
		public IEnumerable<IJsonSchema> OneOf { get; set; }
		/// <summary>
		/// A collection of schema which must not be satisfied.
		/// </summary>
		public IJsonSchema Not { get; set; }
		/// <summary>
		/// Defines a required format for the string.
		/// </summary>
		public StringFormat Format { get; set; }
		/// <summary>
		/// Gets other, non-schema-defined properties.
		/// </summary>
		public JsonObject ExtraneousDetails { get; set; }
		/// <summary>
		/// Identifies the physical path for the schema document (may be different than the ID).
		/// </summary>
		public Uri DocumentPath { get; set; }

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public virtual SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var jValue = root ?? ToJson(null);
			var validators = JsonSchemaPropertyValidatorFactory.Get(this, json);
			var results = validators.Select(v => v.Validate(this, json, jValue)).ToList();
			return new SchemaValidationResults(results);
		}

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public virtual void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Id = obj.TryGetString("id");
			Uri uri;
			var uriFolder = DocumentPath?.OriginalString.EndsWith("/") ?? true ? DocumentPath : DocumentPath?.GetParentUri();
			if (!string.IsNullOrWhiteSpace(Id) &&
				(Uri.TryCreate(Id, UriKind.Absolute, out uri) || Uri.TryCreate(uriFolder + Id, UriKind.Absolute, out uri)))
			{
				DocumentPath = uri;
				JsonSchemaRegistry.Register(this);
			}
			Schema = obj.TryGetString("$schema");
			Title = obj.TryGetString("title");
			Description = obj.TryGetString("description");
			if (obj.ContainsKey("default"))
				Default = obj["default"];
			MultipleOf = obj.TryGetNumber("multipleOf");
			Maximum = obj.TryGetNumber("maximum");
			ExclusiveMaximum = obj.TryGetBoolean("exclusiveMaximum");
			Minimum = obj.TryGetNumber("minimum");
			ExclusiveMinimum = obj.TryGetBoolean("exclusiveMinimum");
			MaxLength = (uint?) obj.TryGetNumber("maxLength");
			MinLength = (uint?) obj.TryGetNumber("minLength");
			Pattern = obj.TryGetString("pattern");
			if (obj.ContainsKey("additionalItems"))
			{
				if (obj["additionalItems"].Type == JsonValueType.Boolean)
					AdditionalItems = obj["additionalItems"].Boolean ? AdditionalItems.True : AdditionalItems.False;
				else
					AdditionalItems = new AdditionalItems {Definition = JsonSchemaFactory.FromJson(obj["additionalItems"], DocumentPath) };
			}
			MaxItems = (uint?) obj.TryGetNumber("maxItems");
			MinItems = (uint?) obj.TryGetNumber("minItems");
			if (obj.ContainsKey("items"))
				Items = JsonSchemaFactory.FromJson(obj["items"], DocumentPath);
			UniqueItems = obj.TryGetBoolean("uniqueItems");
			MaxProperties = (uint?) obj.TryGetNumber("maxProperties");
			MinProperties = (uint?) obj.TryGetNumber("minProperties");
			// Must deserialize "properties" before "required".
			if (obj.ContainsKey("properties"))
			{
				Properties = new JsonSchemaPropertyDefinitionCollection();
				foreach (var prop in obj["properties"].Object)
				{
					var property = new JsonSchemaPropertyDefinition(prop.Key) { Type = JsonSchemaFactory.FromJson(prop.Value, DocumentPath) };
					Properties.Add(property);
				}
			}
			if (obj.ContainsKey("required"))
			{
				var properties = Properties ?? new JsonSchemaPropertyDefinitionCollection();
				var newProperties = new List<JsonSchemaPropertyDefinition>();
				var requiredProperties = obj["required"].Array.Select(v => v.String);
				foreach (var propertyName in requiredProperties)
				{
					var property = properties.FirstOrDefault(p => p.Name == propertyName);
					if (property != null)
						property.IsRequired = true;
					else
					{
						newProperties.Add(new JsonSchemaPropertyDefinition(propertyName)
							{
								IsHidden = true,
								IsRequired = true
							});
					}
				}
				properties.AddRange(newProperties);
				Properties = properties;
			}
			if (obj.ContainsKey("additionalProperties"))
			{
				if (obj["additionalProperties"].Type == JsonValueType.Boolean)
					AdditionalProperties = obj["additionalProperties"].Boolean ? AdditionalProperties.True : AdditionalProperties.False;
				else
					AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaFactory.FromJson(obj["additionalProperties"], DocumentPath)};
			}
			if (obj.ContainsKey("definitions"))
			{
				Definitions = new JsonSchemaTypeDefinitionCollection();
				foreach (var defn in obj["definitions"].Object)
				{
					var definition = new JsonSchemaTypeDefinition(defn.Key) {Definition = JsonSchemaFactory.FromJson(defn.Value, DocumentPath) };

					Definitions.Add(definition);
				}
			}
			if (obj.ContainsKey("patternProperties"))
			{
				var patterns = obj["patternProperties"].Object;
				PatternProperties = patterns.ToDictionary(kvp => new Regex(kvp.Key), kvp => JsonSchemaFactory.FromJson(kvp.Value, DocumentPath));
			}
			if (obj.ContainsKey("dependencies"))
				Dependencies = obj["dependencies"].Object.Select(v =>
					{
						IJsonSchemaDependency dependency;
						switch (v.Value.Type)
						{
							case JsonValueType.Object:
								dependency = new SchemaDependency(v.Key, JsonSchemaFactory.FromJson(v.Value, DocumentPath));
								break;
							case JsonValueType.Array:
								dependency = new PropertyDependency(v.Key, v.Value.Array.Select(jv => jv.String));
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						return dependency;
					});
			if (obj.ContainsKey("enum"))
				Enum = json.Object["enum"].Array.Select(jv => new EnumSchemaValue(jv));
			if (obj.ContainsKey("type"))
			{
				var typeEntry = obj["type"];
				switch (typeEntry.Type)
				{
					case JsonValueType.String:
						// string implies primitive type
						Type = _GetPrimitiveDefinition(typeEntry.String);
						break;
					case JsonValueType.Array:
						// array implies "oneOf" several primitive types
						Type = new JsonSchemaMultiTypeDefinition(false);
						Type.FromJson(typeEntry, serializer);
						Type.Definition.DocumentPath = DocumentPath;
						break;
				}
			}
			if (obj.ContainsKey("allOf"))
				AllOf = obj["allOf"].Array.Select((j) => JsonSchemaFactory.FromJson(j, DocumentPath));
			if (obj.ContainsKey("anyOf"))
				AnyOf = json.Object["anyOf"].Array.Select((j) => JsonSchemaFactory.FromJson(j, DocumentPath));
			if (obj.ContainsKey("oneOf"))
				OneOf = obj["oneOf"].Array.Select((j) => JsonSchemaFactory.FromJson(j, DocumentPath));
			if (obj.ContainsKey("not"))
				Not = JsonSchemaFactory.FromJson(obj["not"], DocumentPath);
			var formatKey = obj.TryGetString("format");
			Format = StringFormat.GetFormat(formatKey);
			var details = obj.Where(kvp => !_definedProperties.Contains(kvp.Key)).ToJson();
			if (details.Any())
				ExtraneousDetails = details;
		}

		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public virtual JsonValue ToJson(JsonSerializer serializer)
		{
			var requiredProperties = new List<string>();
			if (Properties != null)
				requiredProperties = Properties.Where(p => p.IsRequired).Select(p => p.Name).ToList();
			var json = new JsonObject();
			if (Id != null) json["id"] = Id;
			if (!string.IsNullOrWhiteSpace(Schema)) json["$schema"] = Schema;
			if (Title != null) json["title"] = Title;
			if (!string.IsNullOrWhiteSpace(Description)) json["description"] = Description;
			if (Default != null) json["default"] = Default;
			if (MultipleOf.HasValue) json["multipleOf"] = MultipleOf;
			if (Maximum.HasValue) json["maximum"] = Maximum;
			if (ExclusiveMaximum.HasValue) json["exclusiveMaximum"] = ExclusiveMaximum;
			if (Minimum.HasValue) json["minimum"] = Minimum;
			if (ExclusiveMinimum.HasValue) json["exclusiveMinimum"] = ExclusiveMinimum;
			if (MaxLength.HasValue) json["maxLength"] = MaxLength;
			if (MinLength.HasValue) json["minLength"] = MinLength;
			if (Pattern != null) json["pattern"] = Pattern;
			if (AdditionalItems != null)
				json["additionalItems"] = AdditionalItems.ToJson(serializer);
			if (Items != null)
			{
				var items = Items as JsonSchema;
				var type = items?.Type as JsonSchemaMultiTypeDefinition;
				if (type != null && !type.IsPrimitive)
					json["items"] = items.Type.ToJson(serializer);
				else
					json["items"] = Items.ToJson(serializer);
			}
			if (MaxItems.HasValue) json["maxItems"] = MinItems;
			if (MinItems.HasValue) json["minItems"] = MinItems;
			if (UniqueItems ?? false) json["uniqueItems"] = UniqueItems;
			if (MaxProperties.HasValue) json["maxProperties"] = MaxProperties;
			if (MinProperties.HasValue) json["minProperties"] = MinProperties;
			if (requiredProperties.Any()) json["required"] = requiredProperties.ToJson();
			if (AdditionalProperties != null)
				json["additionalProperties"] = AdditionalProperties.ToJson(serializer);
			if (Definitions != null) json["definitions"] = Definitions.ToDictionary(d => d.Name, d => d.Definition).ToJson(serializer);
			if (Properties != null && Properties.Any(p => !p.IsHidden))
			{
				json["properties"] = Properties.Where(p => !p.IsHidden).ToDictionary(p => p.Name, p => p.Type).ToJson(serializer);
			}
			if (PatternProperties != null && PatternProperties.Any())
				json["patternProperties"] = PatternProperties.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value).ToJson(serializer);
			if ((Dependencies != null) && Dependencies.Any())
			{
				var jsonDependencies = new JsonObject();
				foreach (var dependency in Dependencies)
				{
					jsonDependencies[dependency.PropertyName] = dependency.GetJsonData();
				}
				json["dependencies"] = jsonDependencies;
			}
			if (Enum != null)
				json["enum"] = Enum.ToJson(serializer);
			if (Type != null)
				json["type"] = Type.ToJson(serializer);
			if (AllOf != null) json["allOf"] = AllOf.ToJson(serializer);
			if (AnyOf != null) json["anyOf"] = AnyOf.ToJson(serializer);
			if (OneOf != null) json["oneOf"] = OneOf.ToJson(serializer);
			if (Not != null) json["not"] = Not.ToJson(serializer);
			if (Format != null) json["format"] = Format.Key;
			if (ExtraneousDetails != null)
			{
				foreach (var kvp in ExtraneousDetails.Where(kvp => !_definedProperties.Contains(kvp.Key)))
				{
					json[kvp.Key] = kvp.Value;
				}
			}
			return json;
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public virtual bool Equals(IJsonSchema other)
		{
			var schema = other as JsonSchema;
			if (ReferenceEquals(null, schema)) return false;
			if (ReferenceEquals(this, schema)) return true;
			if (Id != schema.Id) return false;
			if (Schema != schema.Schema) return false;
			if (Title != schema.Title) return false;
			if (Description != schema.Description) return false;
			if (!Equals(Default, schema.Default)) return false;
			if (MultipleOf != schema.MultipleOf) return false;
			if (Maximum != schema.Maximum) return false;
			if (ExclusiveMaximum != schema.ExclusiveMaximum) return false;
			if (Minimum != schema.Minimum) return false;
			if (ExclusiveMinimum != schema.ExclusiveMinimum) return false;
			if (MaxLength != schema.MaxLength) return false;
			if (MinLength != schema.MinLength) return false;
			if (Pattern != schema.Pattern) return false;
			if (!Equals(AdditionalItems, schema.AdditionalItems)) return false;
			if (!Equals(Items, schema.Items)) return false;
			if (MaxItems != schema.MaxItems) return false;
			if (MinItems != schema.MinItems) return false;
			if (UniqueItems != schema.UniqueItems) return false;
			if (!Equals(AdditionalProperties, schema.AdditionalProperties)) return false;
			if (!Definitions.ContentsEqual(schema.Definitions)) return false;
			if (!Properties.ContentsEqual(schema.Properties)) return false;
			if (!PatternProperties.ContentsEqual(PatternProperties)) return false;
			if (!Dependencies.ContentsEqual(schema.Dependencies)) return false;
			if (!Enum.ContentsEqual(schema.Enum)) return false;
			if (!Equals(Type, schema.Type)) return false;
			if (!AllOf.ContentsEqual(schema.AllOf)) return false;
			if (!AnyOf.ContentsEqual(schema.AnyOf)) return false;
			if (!OneOf.ContentsEqual(schema.OneOf)) return false;
			if (!Equals(Not, schema.Not)) return false;
			if (!Equals(Format, schema.Format)) return false;
			return Dependencies.ContentsEqual(schema.Dependencies);
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			return Equals(obj as IJsonSchema);
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Type?.GetHashCode() ?? 0;
				hashCode = (hashCode*397) ^ (Id?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Schema?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Title?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Description?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Default?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ MultipleOf.GetHashCode();
				hashCode = (hashCode*397) ^ Maximum.GetHashCode();
				hashCode = (hashCode*397) ^ ExclusiveMaximum.GetHashCode();
				hashCode = (hashCode*397) ^ Minimum.GetHashCode();
				hashCode = (hashCode*397) ^ ExclusiveMinimum.GetHashCode();
				hashCode = (hashCode*397) ^ (MaxLength?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (MinLength?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Pattern?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (AdditionalItems?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Items?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (MaxItems?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (MinItems?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (UniqueItems?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (AdditionalProperties?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Definitions?.ToList().GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Properties?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (PatternProperties?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Dependencies?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Enum?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Type?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (AllOf?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (AnyOf?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (OneOf?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Not?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Format?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		private static JsonSchemaTypeDefinition _GetPrimitiveDefinition(string type)
		{
			switch (type)
			{
				case "array":
					return JsonSchemaTypeDefinition.Array;
				case "boolean":
					return JsonSchemaTypeDefinition.Boolean;
				case "integer":
					return JsonSchemaTypeDefinition.Integer;
				case "null":
					return JsonSchemaTypeDefinition.Null;
				case "number":
					return JsonSchemaTypeDefinition.Number;
				case "object":
					return JsonSchemaTypeDefinition.Object;
				case "string":
					return JsonSchemaTypeDefinition.String;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}