---
title: JSON Schema
category: API Reference
order: 5
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# IJsonSchema

Defines a type for all schema to implement.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- IJsonSchema

## Properties

### System.Uri DocumentPath { get; set; }

Identifies the physical path for the schema document (may be different than the ID).

### string Id { get; set; }

Used to specify which this schema defines.

### string Schema { get; set; }

Used to specify a schema which contains the definitions used by this schema.

#### Remarks

if left null, the default of http://json-schema.org/draft-04/schema# is used.

## Methods

### [SchemaValidationResults](API-Schema#schemavalidationresults) Validate(JsonValue json, JsonValue root)

Validates a [JsonValue](API-Json#jsonvalue) against the schema.

**Parameter:** json

A [JsonValue](API-Json#jsonvalue)

**Parameter:** root

The root schema serialized to a [JsonValue](API-Json#jsonvalue). Used internally for resolving references.

**Returns:** The results of the validation.

# SchemaValidationResults

Contains the results of schema validation.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- SchemaValidationResults

## Constructors

### SchemaValidationResults(string propertyName, string message)

Creates an instance of [SchemaValidationResults](API-Schema#schemavalidationresults).

**Parameter:** propertyName

The name of the property that failed.

**Parameter:** message

A message explaining the error.

## Properties

### IEnumerable&lt;SchemaValidationError&gt; Errors { get; }

Gets a collection of any errors which may have occurred during validation.

### bool Valid { get; }

Gets whether the validation was successful.

# SchemaValidationError

Represents a single schema validation error.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- SchemaValidationError

## Properties

### string Message { get; }

A message indicating the failure.

### string PropertyName { get; }

The property or property path which failed validation.

## Methods

### bool Equals(SchemaValidationError other)

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

### int GetHashCode()

Serves as a hash function for a particular type.

**Returns:** A hash code for the current System.Object.

#### Filterpriority

2

### string ToString()

Returns a string that represents the current object.

**Returns:** A string that represents the current object.

#### Filterpriority

2

# JsonSchemaOptions

Defines options associated with JSON Schema.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- JsonSchemaOptions

## Properties

### static Func&lt;string, string&gt; Download { get; set; }

Gets and sets a method used to download online schema.

### static bool EnforceReadOnly { get; set; }

Gets or sets whether the &quot;readOnly&quot; schema keyword should be enforced. The default is true.

### static bool ValidateFormat { get; set; }

Gets or sets whether the &quot;format&quot; schema keyword should be validated. The default is true.

# JsonSchemaFactory

Defines methods to build schema objects.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- JsonSchemaFactory

## Methods

### static [IJsonSchema](API-Schema#ijsonschema) FromJson(JsonValue json, Uri documentPath)

Creates a schema object from its JSON representation.

**Parameter:** json

A JSON object.

**Parameter:** documentPath

The path to the physical location to this document

**Returns:** A schema object

### static [IJsonSchema](API-Schema#ijsonschema) FromJson&lt;T&gt;(JsonValue json, Uri documentPath)

Creates a schema object from its JSON representation, allowing a specific schema version to be used..

**Type Parameter:** T (no constraints)

The type representing the schema version to create.

**Parameter:** json

A JSON object.

**Parameter:** documentPath

The path to the physical location to this document

**Returns:** A schema object

### static [IJsonSchema](API-Schema#ijsonschema) FromJson(JsonValue json, Type type, Uri documentPath)

Creates a schema object from its JSON representation, allowing a specific schema version to be used..

**Parameter:** json

A JSON object.

**Parameter:** type

The type representing the schema version to create.

**Parameter:** documentPath

The path to the physical location to this document

**Returns:** A schema object

### static void SetDefaultSchemaVersion&lt;T&gt;()

Sets the default schema to use when deserializing a schema that doesn&#39;t define its version.

**Type Parameter:** T : [IJsonSchema](API-Schema#ijsonschema)

The schema type.

# JsonSchemaRegistry

Provides a registry in which JSON schema can be saved to be referenced by the
system.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- JsonSchemaRegistry

## Methods

### static void Clear()

Clears the registry of all types.

### static [IJsonSchema](API-Schema#ijsonschema) Get(string uri)

Downloads and registers a schema at the specified URI.

**Parameter:** uri

### static void Register(IJsonSchema schema)

Explicitly registers an existing schema.

**Parameter:** schema

### static void Unregister(IJsonSchema schema)

Removes a schema from the registry.

**Parameter:** schema

### static void Unregister(string uri)

Removes a schema from the registry.

**Parameter:** uri

# SchemaLoadException

Thrown when a schema could not be loaded.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- Exception
- SchemaLoadException

# JsonSchemaReference

Defines a reference to a schema.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- JsonSchemaReference

## Constructors

### JsonSchemaReference(string reference, IJsonSchema baseSchema)

Creates a new instance of the [JsonSchemaReference](API-Schema#jsonschemareference) class that supports additional schema properties.

**Parameter:** reference

The relative (internal) or absolute (URI) path to the referenced type definition.

**Parameter:** baseSchema

An instance of the base schema to use (either [JsonSchema04](API-Schema-4#jsonschema04) or [JsonSchema06](API-Schema-6#jsonschema06)).

**Exception:** System.ArgumentNullException

Thrown when *reference* or *baseSchema* is null.

**Exception:** System.ArgumentException

Thrown when *reference* is empty or whitespace or
if *baseSchema* is not of type [JsonSchema04](API-Schema-4#jsonschema04) or [JsonSchema06](API-Schema-6#jsonschema06).

### JsonSchemaReference(string reference, Type baseSchemaType)

Creates a new instance of the [JsonSchemaReference](API-Schema#jsonschemareference) class.

**Parameter:** reference

The relative (internal) or absolute (URI) path to the referenced type definition.

**Parameter:** baseSchemaType

The draft version of schema to use as a base when resolving if not defined in the resolved schema.
Must be either [JsonSchema04](API-Schema-4#jsonschema04) or [JsonSchema06](API-Schema-6#jsonschema06).

**Exception:** System.ArgumentNullException

Thrown when *reference* is null.

**Exception:** System.ArgumentException

Thrown when *reference* is empty or whitespace or
when *baseSchemaType* is not [JsonSchema04](API-Schema-4#jsonschema04) or [JsonSchema06](API-Schema-6#jsonschema06).

## Properties

### [IJsonSchema](API-Schema#ijsonschema) Base { get; set; }

Provides a mechanism to include sibling keywords alongside $ref.

### System.Uri DocumentPath { get; set; }

Identifies the physical path for the schema document (may be different than the ID).

### string Id { get; set; }

Used to specify which this schema defines.

### string Reference { get; }

Defines the reference in respect to the root schema.

### [IJsonSchema](API-Schema#ijsonschema) Resolved { get; }

Exposes the schema at the references location.

#### Remarks

The Manatee.Json.Schema.JsonSchemaReference._Resolve(Manatee.Json.JsonValue) method must first be called.

### string Schema { get; set; }

Used to specify a schema which contains the definitions used by this schema.

#### Remarks

if left null, the default of http://json-schema.org/draft-04/schema# is used.

## Methods

### bool Equals(IJsonSchema other)

Indicates whether the current object is equal to another object of the same type.

**Parameter:** other

An object to compare with this object.

**Returns:** true if the current object is equal to the *other* parameter; otherwise, false.

### bool Equals(Object obj)

Determines whether the specified object is equal to the current object.

**Parameter:** obj

The object to compare with the current object.

**Returns:** true if the specified object is equal to the current object; otherwise, false.

#### Filterpriority

2

### bool Equals(Object obj)

Determines whether the specified object is equal to the current object.

**Parameter:** obj

The object to compare with the current object.

**Returns:** true if the specified object is equal to the current object; otherwise, false.

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

# JsonSchemaType

Defines the recognized schema data types.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- JsonSchemaType

## Fields

### NotDefined

Provides a default value so that type cannot be assumed.

### Array

Indicates the array type.

### Boolean

Indicates the boolean type.

### Integer

Indicates the integer type.

### Null

Indicates the null type.

### Number

Indicates the number type.

### Object

Indicates the object type.

### String

Indicates the string type.

# JsonSchemaPropertyValidatorFactory

Provides the validation system with validators.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- JsonSchemaPropertyValidatorFactory

## Methods

### static void RegisterValidator(IJsonSchemaPropertyValidator validator)

Registers a new validator to be executed during schema validation.

**Parameter:** validator



# IJsonSchemaPropertyValidator

Performs validations for a single schema property.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- IJsonSchemaPropertyValidator

## Methods

### bool Applies(IJsonSchema schema, JsonValue json)

Determines whether the validator should execute for a particular schema and JSON instance.

**Parameter:** schema

The schema.

**Parameter:** json

The JSON instance.

**Returns:** true if the validatory should execute; otherwise false.

### [SchemaValidationResults](API-Schema#schemavalidationresults) Validate(IJsonSchema schema, JsonValue json, JsonValue root)

Performs validations of a schema property on a JSON instance.

**Parameter:** schema

The schema.

**Parameter:** json

The JSON instance.

**Parameter:** root

The root schema serialized to a [JsonValue](API-Json#jsonvalue). Used resolving references.
Pass this to any subschema validations that need to be performed.

**Returns:** A [SchemaValidationResults](API-Schema#schemavalidationresults) instance.

