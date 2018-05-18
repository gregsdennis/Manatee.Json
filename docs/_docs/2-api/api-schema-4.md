---
title: JSON Schema, Draft 4
category: API Reference
order: 7
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# JsonSchema04

Provides base functionality for the basic [IJsonSchema](API-Schema#ijsonschema) implementations.S

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- JsonSchema04

## Fields

### static [JsonSchema04](API-Schema-4#jsonschema04) Empty

Defines an empty Schema. Useful for specifying that any schema is valid.

### static [JsonSchema04](API-Schema-4#jsonschema04) MetaSchema

Defines the Draft-04 Schema as presented at http://json-schema.org/draft-04/schema#

### static [JsonSchemaReference](API-Schema#jsonschemareference) Root

Defines the root reference schema for [JsonSchema04](API-Schema-4#jsonschema04).

## Properties

### [AdditionalItems](API-Schema-Misc#additionalitems) AdditionalItems { get; set; }

Defines any additional items to be expected by this schema.

### [AdditionalProperties](API-Schema-Misc#additionalproperties) AdditionalProperties { get; set; }

Defines any additional properties to be expected by this schema.

### IEnumerable&lt;IJsonSchema&gt; AllOf { get; set; }

A collection of required schema which must be satisfied.

### IEnumerable&lt;IJsonSchema&gt; AnyOf { get; set; }

A collection of schema options.

### [JsonValue](API-Json#jsonvalue) Default { get; set; }

The default value for this schema.

#### Remarks

The default value is defined as a JSON value which may need to be deserialized
to a .Net data structure.

### Dictionary&lt;string, IJsonSchema&gt; Definitions { get; set; }

Defines a collection of schema type definitions.

### IEnumerable&lt;IJsonSchemaDependency&gt; Dependencies { get; set; }

Defines property dependencies.

### string Description { get; set; }

Defines a description for this schema.

### System.Uri DocumentPath { get; set; }

Identifies the physical path for the schema document (may be different than the ID).

### IEnumerable&lt;EnumSchemaValue&gt; Enum { get; set; }

A collection of acceptable values.

### bool? ExclusiveMaximum { get; set; }

Defines whether the maximum value is itself acceptable.

### bool? ExclusiveMinimum { get; set; }

Defines whether the minimum value is itself acceptable.

### [JsonObject](API-Json#jsonobject) ExtraneousDetails { get; set; }

Gets other, non-schema-defined properties.

### [StringFormat](API-Schema-Misc#stringformat) Format { get; set; }

Defines a required format for the string.

### string Id { get; set; }

Used to specify which this schema defines.

### [IJsonSchema](API-Schema#ijsonschema) Items { get; set; }

Defines the schema for the items contained in the array.

### double? Maximum { get; set; }

Defines a maximum acceptable value.

### uint? MaxItems { get; set; }

Defines a maximum number of items required for the array.

### uint? MaxLength { get; set; }

Defines a maximum acceptable length.

### uint? MaxProperties { get; set; }

Defines a maximum acceptable length.

### double? Minimum { get; set; }

Defines a minimum acceptable value.

### uint? MinItems { get; set; }

Gets and sets a minimum number of items required for the array.

### uint? MinLength { get; set; }

Defines a minimum acceptable length.

### uint? MinProperties { get; set; }

Defines a minimum acceptable length.

### double? MultipleOf { get; set; }

Defines a divisor for acceptable values.

### [IJsonSchema](API-Schema#ijsonschema) Not { get; set; }

A collection of schema which must not be satisfied.

### IEnumerable&lt;IJsonSchema&gt; OneOf { get; set; }

A collection of schema options.

### string Pattern { get; set; }

Defines a System.Text.RegularExpressions.Regex pattern for to which the value must adhere.

### Dictionary&lt;Regex, IJsonSchema&gt; PatternProperties { get; set; }

Defines additional properties based on regular expression matching of the property name.

### Dictionary&lt;string, IJsonSchema&gt; Properties { get; set; }

Defines a collection of properties expected by this schema.

### IEnumerable&lt;string&gt; Required { get; set; }

A collection of property names that are required.

### string Schema { get; set; }

Used to specify a schema which contains the definitions used by this schema.

#### Remarks

if left null, the default of http://json-schema.org/draft-04/schema# is used.

### string Title { get; set; }

Defines a title for this schema.

### [JsonSchemaType](API-Schema#jsonschematype) Type { get; set; }

The JSON Schema type which defines this schema.

### bool? UniqueItems { get; set; }

Defines whether the array should contain only unique items.

## Methods

### bool Equals(IJsonSchema other)

Indicates whether the current object is equal to another object of the same type.

**Parameter:** other

An object to compare with this object.

**Returns:** true if the current object is equal to the *other* parameter; otherwise, false.

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The object to compare with the current object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The object to compare with the current object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### void FromJson(JsonValue json, JsonSerializer serializer)

Builds an object from a [JsonValue](API-Json#jsonvalue).

**Parameter:** json

The [JsonValue](API-Json#jsonvalue) representation of the object.

**Parameter:** serializer

The [JsonSerializer](API-Serializer#jsonserializer) instance to use for additional
serialization of values.

### int GetHashCode()

Serves as a hash function for a particular type.

**Returns:** A hash code for the current System.Object.

#### Filterpriority

2

### [JsonValue](API-Json#jsonvalue) ToJson(JsonSerializer serializer)

Converts an object to a [JsonValue](API-Json#jsonvalue).

**Parameter:** serializer

The [JsonSerializer](API-Serializer#jsonserializer) instance to use for additional
serialization of values.

**Returns:** The [JsonValue](API-Json#jsonvalue) representation of the object.

### [SchemaValidationResults](API-Schema#schemavalidationresults) Validate(JsonValue json, JsonValue root)

Validates a [JsonValue](API-Json#jsonvalue) against the schema.

**Parameter:** json

A [JsonValue](API-Json#jsonvalue)

**Parameter:** root

The root schema serialized to a [JsonValue](API-Json#jsonvalue). Used internally for resolving references.

**Returns:** True if the [JsonValue](API-Json#jsonvalue) passes validation; otherwise false.

