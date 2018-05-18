---
title: JSON Schema, Common Types
category: API Reference
order: 6
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# StringFormat

Defines various string formatting types used for StringSchema validation.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- StringFormat

## Fields

### static [StringFormat](API-Schema-Misc#stringformat) DateTime

Defines a date/time format via System.DateTime.TryParse(System.String,System.DateTime@)

### static [StringFormat](API-Schema-Misc#stringformat) Email

Defines an email address format.

#### Remarks

From http://www.regular-expressions.info/email.html

### static [StringFormat](API-Schema-Misc#stringformat) HostName

Defines a host name format.

### static [StringFormat](API-Schema-Misc#stringformat) Ipv4

Defines an IPV4 address format.

### static [StringFormat](API-Schema-Misc#stringformat) Ipv6

Defines an IPV6 format.

### static [StringFormat](API-Schema-Misc#stringformat) Regex

Defines a regular expression format.

### static [StringFormat](API-Schema-Misc#stringformat) Uri

Defines a URI format via System.Uri.IsWellFormedUriString(System.String,System.UriKind).

#### Remarks

For draft-06 schema, only use this for absolute URIs.

### static [StringFormat](API-Schema-Misc#stringformat) UriReference

Defines a URI format via System.Uri.IsWellFormedUriString(System.String,System.UriKind)

## Properties

### string Key { get; }

A string key which specifies this string format.

## Methods

### static [StringFormat](API-Schema-Misc#stringformat) GetFormat(string formatKey)

Gets a [StringFormat](API-Schema-Misc#stringformat) object based on a format key.

**Parameter:** formatKey

The predefined key for the format.

**Returns:** A [StringFormat](API-Schema-Misc#stringformat) object, or null if none exists for the key.

### bool Validate(string value)

Validates a value to the specified format.

**Parameter:** value

The value to validate.

**Returns:** True if the value is valid, otherwise false.

# ContentEncoding

Defines possible values for the draft-07 JSON Schema property [ContentEncoding](API-Schema-7#contentencoding-contentencoding--get-set-).

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- ContentEncoding

## Fields

### SevenBit

Indicates a 7-bit encoding.

### EightBit

Indicates a 8-bit encoding.

### Binary

Indicates a binary encoding.

### QuotedPrintable

Indicates a quoted-printable encoding.

### Base64

Indicates a base-64 encoding.

### IetfToken

Indicates an ietf-token encoding.

### XToken

Indicates an x-token encoding.

# AdditionalItems

Defines additional items for array-type schemas.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- AdditionalItems

## Fields

### static [AdditionalItems](API-Schema-Misc#additionalitems) False

Prohibits additional items in the JSON.

### static [AdditionalItems](API-Schema-Misc#additionalitems) True

Allows any additional items to be added to the JSON.

## Properties

### [IJsonSchema](API-Schema#ijsonschema) Definition { get; set; }

Defines a schema to which any additional properties must validate.

**Exception:** Manatee.Json.ReadOnlyException

Thrown when attempting to set the definition
of one of the static [AdditionalProperties](API-Schema-Misc#additionalproperties) fields.

## Methods

### bool Equals(AdditionalItems other)

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

### string ToString()

Returns a string that represents the current object.

**Returns:** A string that represents the current object.

#### Filterpriority

2

# AdditionalProperties

Defines additional properties for object-specific schemas.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- AdditionalProperties

## Fields

### static [AdditionalProperties](API-Schema-Misc#additionalproperties) False

Prohibits additional properties in the JSON.

### static [AdditionalProperties](API-Schema-Misc#additionalproperties) True

Allows any additional property to be added to the JSON.

## Properties

### [IJsonSchema](API-Schema#ijsonschema) Definition { get; set; }

Defines a schema to which any additional properties must validate.

**Exception:** Manatee.Json.ReadOnlyException

Thrown when attempting to set the definition
of one of the static [AdditionalProperties](API-Schema-Misc#additionalproperties) fields.

## Methods

### bool Equals(AdditionalProperties other)

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

### string ToString()

Returns a string that represents the current object.

**Returns:** A string that represents the current object.

#### Filterpriority

2

# EnumSchemaValue

Defines a single schema enumeration value.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- EnumSchemaValue

## Constructors

### EnumSchemaValue(JsonValue value)

Creates a new instance of the [EnumSchemaValue](API-Schema-Misc#enumschemavalue) class.

**Parameter:** value

The value.

## Methods

### bool Equals(EnumSchemaValue other)

Indicates whether the current object is equal to another object of the same type.

**Parameter:** other

An object to compare with this object.

**Returns:** true if the current object is equal to the *other* parameter; otherwise, false.

### void FromJson(JsonValue json, JsonSerializer serializer)

Builds an object from a [JsonValue](API-Json#jsonvalue).

**Parameter:** json

The [JsonValue](API-Json#jsonvalue) representation of the object.

**Parameter:** serializer

The [JsonSerializer](API-Serializer#jsonserializer) instance to use for additional
serialization of values.

### [JsonValue](API-Json#jsonvalue) ToJson(JsonSerializer serializer)

Converts an object to a [JsonValue](API-Json#jsonvalue).

**Parameter:** serializer

The [JsonSerializer](API-Serializer#jsonserializer) instance to use for additional
serialization of values.

**Returns:** The [JsonValue](API-Json#jsonvalue) representation of the object.

### [SchemaValidationResults](API-Schema#schemavalidationresults) Validate(JsonValue json)

Validates a [JsonValue](API-Json#jsonvalue) against the schema.

**Parameter:** json

A [JsonValue](API-Json#jsonvalue)

**Returns:** The results of the validation.

# IJsonSchemaDependency

Defines properties and methods required to represent dependencies within JSON Schema.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- IJsonSchemaDependency

## Properties

### string PropertyName { get; }

Gets or sets the property with the dependency.

## Methods

### [JsonValue](API-Json#jsonvalue) GetJsonData()

Gets the JSON data to be used as the value portion in the dependency list of the schema.

### [SchemaValidationResults](API-Schema#schemavalidationresults) Validate(JsonValue json, JsonValue root)

Validates a [JsonValue](API-Json#jsonvalue) against the schema.

**Parameter:** json

A [JsonValue](API-Json#jsonvalue)

**Parameter:** root

The root schema serialized to a [JsonValue](API-Json#jsonvalue). Used internally for resolving references.

**Returns:** The results of the validation.

# PropertyDependency

Declares a dependency that is based on the presence of other properties in the JSON.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- PropertyDependency

## Constructors

### PropertyDependency(string propertyName, string firstDependency, String[] otherDependencies)

Creates a new instance of the [PropertyDependency](API-Schema-Misc#propertydependency) class.

**Parameter:** propertyName

The property name.

**Parameter:** firstDependency

A minimal required property dependency.

**Parameter:** otherDependencies

Additional property dependencies.

## Properties

### string PropertyName { get; }

Gets or sets the property with the dependency.

## Methods

### [JsonValue](API-Json#jsonvalue) GetJsonData()

Gets the JSON data to be used as the value portion in the dependency list of the schema.

### [SchemaValidationResults](API-Schema#schemavalidationresults) Validate(JsonValue json, JsonValue root)

Validates a [JsonValue](API-Json#jsonvalue) against the schema.

**Parameter:** json

A [JsonValue](API-Json#jsonvalue)

**Parameter:** root

The root schema serialized to a [JsonValue](API-Json#jsonvalue). Used internally for resolving references.

**Returns:** The results of the validation.

# SchemaDependency

Creates a dependency that is based on a secondary schema.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- SchemaDependency

## Constructors

### SchemaDependency(string propertyName, IJsonSchema schema)

Creates a new instance of the [SchemaDependency](API-Schema-Misc#schemadependency) class.

**Parameter:** propertyName

The property name.

**Parameter:** schema

The schema which must be validated.

## Properties

### string PropertyName { get; }

Gets or sets the property with the dependency.

## Methods

### [JsonValue](API-Json#jsonvalue) GetJsonData()

Gets the JSON data to be used as the value portion in the dependency list of the schema.

### [SchemaValidationResults](API-Schema#schemavalidationresults) Validate(JsonValue json, JsonValue root)

Validates a [JsonValue](API-Json#jsonvalue) against the schema.

**Parameter:** json

A [JsonValue](API-Json#jsonvalue)

**Parameter:** root

The root schema serialized to a [JsonValue](API-Json#jsonvalue). Used internally for resolving references.

**Returns:** The results of the validation.

# JsonSchemaCollection

Represents a collection of schemata.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Schema

**Inheritance hierarchy:**

- Object
- List&lt;IJsonSchema&gt;
- JsonSchemaCollection

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

### bool Equals(IJsonSchema other)

Indicates whether the current object is equal to another object of the same type.

**Parameter:** other

An object to compare with this object.

**Returns:** true if the current object is equal to the *other* parameter; otherwise, false.

### void FromJson(JsonValue json, JsonSerializer serializer)

Builds an object from a [JsonValue](API-Json#jsonvalue).

**Parameter:** json

The [JsonValue](API-Json#jsonvalue) representation of the object.

**Parameter:** serializer

The [JsonSerializer](API-Serializer#jsonserializer) instance to use for additional
serialization of values.

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

**Returns:** The results of the validation.

