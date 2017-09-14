using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Serialization;
using Manatee.Json.Internal;
using Manatee.Json.Schema.Validators;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides base functionality for the basic <see cref="IJsonSchema"/> implementations.
	/// </summary>
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
	public class JsonSchema06 : IJsonSchema
	{
		/// <summary>
		/// Defines an empty Schema.  Useful for specifying that any schema is valid.
		/// </summary>
		public static readonly JsonSchema06 Empty = new JsonSchema06();
		public static readonly JsonSchema06 True = new JsonSchema06();
		public static readonly JsonSchema06 False = new JsonSchema06();
		/// <summary>
		/// Defines the Draft-04 Schema as presented at http://json-schema.org/draft-06/schema#
		/// </summary>
		public static readonly JsonSchema06 MetaSchema = new JsonSchema06
			{
				Id = "http://json-schema.org/draft-06/schema#",
				Schema = "http://json-schema.org/draft-06/schema#",
				Title = "Core schema meta-schema",
				Definitions = new JsonSchemaTypeDefinitionCollection
					{
						["schemaArray"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Array,
								MinItems = 1,
								Items = JsonSchemaReference.Root
							},
						["nonNegativeInteger"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Integer,
								Minimum = 0
							},
						["nonNegativeIntegerDefault0"] = new JsonSchema06
							{
								AllOf = new List<IJsonSchema>
									{
										new JsonSchemaReference("#/definitions/nonNegativeInteger"),
										new JsonSchema06 {Default = 0}
									}
							},
						["simpleTypes"] = new JsonSchema06
							{
								Enum = new List<EnumSchemaValue>
									{
										"array",
										"boolean",
										"integer",
										"null",
										"number",
										"object",
										"string"
									}
							},
						["stringArray"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Array,
								Items = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
								UniqueItems = true,
								Default = new JsonArray()
							}
					},
				Type = new JsonSchemaMultiTypeDefinition(JsonSchemaTypeDefinition.Object, JsonSchemaTypeDefinition.Boolean),
				Properties = new JsonSchemaPropertyDefinitionCollection
					{
						["$id"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.String,
								Format = StringFormat.UriReference
							},
						["$schema"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.String,
								Format = StringFormat.Uri
							},
						["title"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
						["description"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
						["default"] = Empty,
						["multipleOf"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Number,
								ExclusiveMinimum = 0
							},
						["maximum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
						["exclusiveMaximum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
						["minimum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
						["exclusiveMinimum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
						["maxLength"] = new JsonSchemaReference("#/definitions/nonNegativeInteger"),
						["minLength"] = new JsonSchemaReference("#/definitions/nonNegativeIntegerDefault0"),
						["pattern"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.String,
								Format = StringFormat.Regex
							},
						["additionalItems"] = JsonSchemaReference.Root,
						["items"] = new JsonSchema06
							{
								AnyOf = new List<IJsonSchema>
									{
										JsonSchemaReference.Root,
										new JsonSchemaReference("#/definitions/schemaArray")
									},
								Default = new JsonObject()
							},
						["maxItems"] = new JsonSchemaReference("#/definitions/nonNegativeInteger"),
						["minItems"] = new JsonSchemaReference("#/definitions/nonNegativeIntegerDefault0"),
						["uniqueItems"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Boolean,
								Default = false
							},
						["contains"] = JsonSchemaReference.Root,
						["maxProperties"] = new JsonSchemaReference("#/definitions/nonNegativeInteger"),
						["minProperties"] = new JsonSchemaReference("#/definitions/nonNegativeIntegerDefault0"),
						["required"] = new JsonSchemaReference("#/definitions/stringArray"),
						["additionalProperties"] = JsonSchemaReference.Root,
						["definitions"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Object,
								AdditionalProperties = JsonSchemaReference.Root,
								Default = new JsonObject()
							},
						["properties"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Object,
								AdditionalProperties = JsonSchemaReference.Root,
								Default = new JsonObject()
							},
						["patternProperties"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Object,
								AdditionalProperties = JsonSchemaReference.Root,
								Default = new JsonObject()
							},
						["dependencies"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Object,
								AdditionalProperties = new JsonSchema06
									{
										AnyOf = new List<IJsonSchema>
											{
												JsonSchemaReference.Root,
												new JsonSchemaReference("#/definitions/stringArray")
											}
									}
							},
						["propertyNames"] = JsonSchemaReference.Root,
						["const"] = Empty,
						["enum"] = new JsonSchema06
							{
								Type = JsonSchemaTypeDefinition.Array,
								MinItems = 1,
								UniqueItems = true
							},
						["type"] = new JsonSchema06
							{
								AnyOf = new List<IJsonSchema>
									{
										new JsonSchemaReference("#/definitions/simpleTypes"),
										new JsonSchema06
											{
												Type = JsonSchemaTypeDefinition.Array,
												Items = new JsonSchemaReference("#/definitions/simpleTypes"),
												MinItems = 1,
												UniqueItems = true
											}
									}
							},
						["format"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
						["allOf"] = new JsonSchemaReference("#/definitions/schemaArray"),
						["anyOf"] = new JsonSchemaReference("#/definitions/schemaArray"),
						["oneOf"] = new JsonSchemaReference("#/definitions/schemaArray"),
						["not"] = JsonSchemaReference.Root,
					},
				Default = new JsonObject()
			};

		private static readonly IEnumerable<string> _definedProperties =
			MetaSchema.Properties.Select(p => p.Name).ToList();

		private string _id;
		private string _schema;
		private double? _multipleOf;
		private bool? _booleanSchemaDefinition;
		private StringFormat _format;

		/// <summary>
		/// Used to specify which this schema defines.
		/// </summary>
		public string Id
		{
			get { return _id; }
			set
			{
				if (!string.IsNullOrWhiteSpace(value) && !StringFormat.UriReference.Validate(value))
					throw new ArgumentOutOfRangeException(nameof(Id), "'$id' property must be a well-formed URI.");
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
		public double? ExclusiveMaximum { get; set; }
		/// <summary>
		/// Defines a minimum acceptable value.
		/// </summary>
		public double? Minimum { get; set; }
		/// <summary>
		/// Defines whether the minimum value is itself acceptable.
		/// </summary>
		public double? ExclusiveMinimum { get; set; }
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
		/// Defines a schema that must be contained within an array.
		/// </summary>
		public IJsonSchema Contains { get; set; }
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
		public IJsonSchema AdditionalProperties { get; set; }
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
		/// Defines an expected constant value.
		/// </summary>
		public JsonValue Const { get; set; }
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
		public StringFormat Format
		{
			get { return _format; }
			set
			{
				value?.ValidateForDraft<JsonSchema06>();
				_format = value;
			}
		}
		/// <summary>
		/// Gets other, non-schema-defined properties.
		/// </summary>
		public JsonObject ExtraneousDetails { get; set; }
		/// <summary>
		/// Identifies the physical path for the schema document (may be different than the ID).
		/// </summary>
		public Uri DocumentPath { get; set; }

		private string DebuggerDisplay => ToJson(null).ToString();

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public virtual SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			if (_booleanSchemaDefinition == true)
				return new SchemaValidationResults();
			if (_booleanSchemaDefinition == false)
				return new SchemaValidationResults(new[] {new SchemaValidationError(string.Empty, "All schemata are invalid")});

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
			if (json.Type == JsonValueType.Boolean)
			{
				_booleanSchemaDefinition = json.Boolean;
				return;
			}
			
			var obj = json.Object;
			Id = obj.TryGetString("$id");
			var uriFolder = DocumentPath?.OriginalString.EndsWith("/") ?? true ? DocumentPath : DocumentPath?.GetParentUri();
			if (!string.IsNullOrWhiteSpace(Id) &&
				(Uri.TryCreate(Id, UriKind.Absolute, out Uri uri) || Uri.TryCreate(uriFolder + Id, UriKind.Absolute, out uri)))
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
			ExclusiveMaximum = obj.TryGetNumber("exclusiveMaximum");
			Minimum = obj.TryGetNumber("minimum");
			ExclusiveMinimum = obj.TryGetNumber("exclusiveMinimum");
			MaxLength = (uint?) obj.TryGetNumber("maxLength");
			MinLength = (uint?) obj.TryGetNumber("minLength");
			Pattern = obj.TryGetString("pattern");
			if (obj.ContainsKey("additionalItems"))
			{
				if (obj["additionalItems"].Type == JsonValueType.Boolean)
					AdditionalItems = obj["additionalItems"].Boolean ? AdditionalItems.True : AdditionalItems.False;
				else
					AdditionalItems = new AdditionalItems {Definition = _ReadSchema(obj["additionalItems"]) };
			}
			MaxItems = (uint?) obj.TryGetNumber("maxItems");
			MinItems = (uint?) obj.TryGetNumber("minItems");
			if (obj.ContainsKey("items"))
				Items = _ReadSchema(obj["items"]);
			UniqueItems = obj.TryGetBoolean("uniqueItems");
			if (obj.ContainsKey("contains"))
				Contains = _ReadSchema(obj["contains"]);
			MaxProperties = (uint?) obj.TryGetNumber("maxProperties");
			MinProperties = (uint?) obj.TryGetNumber("minProperties");
			// Must deserialize "properties" before "required".
			if (obj.ContainsKey("properties"))
			{
				Properties = new JsonSchemaPropertyDefinitionCollection();
				foreach (var prop in obj["properties"].Object)
				{
					var property = new JsonSchemaPropertyDefinition(prop.Key) {Type = _ReadSchema(prop.Value)};
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
				AdditionalProperties = _ReadSchema(obj["additionalProperties"]);
			if (obj.ContainsKey("definitions"))
			{
				Definitions = new JsonSchemaTypeDefinitionCollection();
				foreach (var defn in obj["definitions"].Object)
				{
					var definition = new JsonSchemaTypeDefinition(defn.Key) {Definition = _ReadSchema(defn.Value)};

					Definitions.Add(definition);
				}
			}
			if (obj.ContainsKey("patternProperties"))
			{
				var patterns = obj["patternProperties"].Object;
				PatternProperties = patterns.ToDictionary(kvp => new Regex(kvp.Key), kvp => _ReadSchema(kvp.Value));
			}
			if (obj.ContainsKey("dependencies"))
				Dependencies = obj["dependencies"].Object.Select(v =>
					{
						IJsonSchemaDependency dependency;
						switch (v.Value.Type)
						{
							case JsonValueType.Boolean:
							case JsonValueType.Object:
								dependency = new SchemaDependency(v.Key, _ReadSchema(v.Value));
								break;
							case JsonValueType.Array:
								dependency = new PropertyDependency(v.Key, v.Value.Array.Select(jv => jv.String));
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						return dependency;
					});
			if (obj.ContainsKey("const"))
				Const = obj["const"];
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
				AllOf = obj["allOf"].Array.Select(_ReadSchema);
			if (obj.ContainsKey("anyOf"))
				AnyOf = json.Object["anyOf"].Array.Select(_ReadSchema);
			if (obj.ContainsKey("oneOf"))
				OneOf = obj["oneOf"].Array.Select(_ReadSchema);
			if (obj.ContainsKey("not"))
				Not = _ReadSchema(obj["not"]);
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
			if (_booleanSchemaDefinition.HasValue) return _booleanSchemaDefinition;

			var requiredProperties = new List<string>();
			if (Properties != null)
				requiredProperties = Properties.Where(p => p.IsRequired).Select(p => p.Name).ToList();
			var json = new JsonObject();
			if (Id != null) json["$id"] = Id;
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
				var items = Items as JsonSchema06;
				if (items?.Type is JsonSchemaMultiTypeDefinition type && !type.IsPrimitive)
					json["items"] = items.Type.ToJson(serializer);
				else
					json["items"] = Items.ToJson(serializer);
			}
			if (MaxItems.HasValue) json["maxItems"] = MinItems;
			if (MinItems.HasValue) json["minItems"] = MinItems;
			if (UniqueItems ?? false) json["uniqueItems"] = UniqueItems;
			if (Contains != null) json["contains"] = Contains.ToJson(serializer);
			if (MaxProperties.HasValue) json["maxProperties"] = MaxProperties;
			if (MinProperties.HasValue) json["minProperties"] = MinProperties;
			if (requiredProperties.Any()) json["required"] = requiredProperties.ToJson();
			if (AdditionalProperties != null)
				json["additionalProperties"] = AdditionalProperties.ToJson(serializer);
			if (Definitions != null) json["definitions"] = Definitions.ToDictionary(d => d.Name, d => d.Definition).ToJson(serializer);
			if (Properties != null && Properties.Any(p => !p.IsHidden))
				// TODO: Type can be null if the schema was created in code.
				json["properties"] = Properties.Where(p => !p.IsHidden).ToDictionary(p => p.Name, p => p.Type).ToJson(serializer);
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
			if (Const != null)
				json["const"] = Const;
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
			var schema = other as JsonSchema06;
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
			if (!Equals(Contains, schema.Contains)) return false;
			if (!Equals(AdditionalProperties, schema.AdditionalProperties)) return false;
			if (!Definitions.ContentsEqual(schema.Definitions)) return false;
			if (!Properties.ContentsEqual(schema.Properties)) return false;
			if (!PatternProperties.ContentsEqual(PatternProperties)) return false;
			if (!Dependencies.ContentsEqual(schema.Dependencies)) return false;
			if (Const != schema.Const) return false;
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
				hashCode = (hashCode*397) ^ (Contains?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (AdditionalProperties?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Definitions?.GetCollectionHashCode().GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Properties?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (PatternProperties?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Dependencies?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Const?.GetHashCode() ?? 0);
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

		private IJsonSchema _ReadSchema(JsonValue json)
		{
			return JsonSchemaFactory.FromJson(json, () => new JsonSchema06(), DocumentPath);
		}
	}
}