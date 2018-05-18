---
title: Serialization
category: API Reference
order: 3
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# JsonSerializer

Serializes and deserializes objects and types to and from JSON structures.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- JsonSerializer

## Properties

### [AbstractionMap](API-Serializer#abstractionmap) AbstractionMap { get; set; }

Gets or sets the abstraction map used by this serializer.

### [CustomSerializations](API-Serializer#customserializations) CustomSerializations { get; set; }

Gets or sets the set of custom serializations supported by this serializer.

### [JsonSerializerOptions](API-Serializer-Options#jsonserializeroptions) Options { get; set; }

Gets or sets a set of options for this serializer.

## Methods

### T Deserialize&lt;T&gt;(JsonValue json)

Deserializes a JSON structure to an object of the appropriate type.

**Type Parameter:** T (no constraints)

The type of the object that the JSON structure represents.

**Parameter:** json

The JSON representation of the object.

**Returns:** The deserialized object.

**Exception:** Manatee.Json.Serialization.TypeDoesNotContainPropertyException

Optionally thrown during automatic
deserialization when the JSON contains a property which is not defined by the requested
type.

### void DeserializeType&lt;T&gt;(JsonValue json)

Deserializes a JSON structure to the public static properties of a type.

**Type Parameter:** T (no constraints)

The type to deserialize.

**Parameter:** json

The JSON representation of the type.

**Exception:** Manatee.Json.Serialization.TypeDoesNotContainPropertyException

Optionally thrown during automatic
deserialization when the JSON contains a property which is not defined by the requested
type.

### [JsonValue](API-Json#jsonvalue) GenerateTemplate&lt;T&gt;()

Generates a template JSON inserting default values.

**Type Parameter:** T (no constraints)



**Returns:** 

### [JsonValue](API-Json#jsonvalue) Serialize&lt;T&gt;(T obj)

Serializes an object to a JSON structure.

**Type Parameter:** T (no constraints)

The type of the object to serialize.

**Parameter:** obj

The object to serialize.

**Returns:** The JSON representation of the object.

### [JsonValue](API-Json#jsonvalue) SerializeType&lt;T&gt;()

Serializes the public static properties of a type to a JSON structure.

**Type Parameter:** T (no constraints)

The type to serialize.

**Returns:** The JSON representation of the type.

# AbstractionMap

Provides an interface to map abstract and interface types to
concrete types for object instantiation during deserialization.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- AbstractionMap

## Constructors

### AbstractionMap()

Creates a new [AbstractionMap](API-Serializer#abstractionmap) instance.

### AbstractionMap(AbstractionMap other)

Creates a new [AbstractionMap](API-Serializer#abstractionmap) instance using another as a basis.

**Parameter:** other

## Properties

### static [AbstractionMap](API-Serializer#abstractionmap) Default { get; }

Provides a default abstraction map for new [JsonSerializer](API-Serializer#jsonserializer) instances.

## Methods

### Type GetMap(Type type)

Retrieves the map setting for an abstraction type.

**Parameter:** type

The abstraction type.

**Returns:** The mapped type if a mapping exists; otherwise the abstraction type.

### void Map&lt;TAbstract, TConcrete&gt;(MapBaseAbstractionBehavior mappingBehavior)

Applies a mapping from an abstraction to a concrete type.

**Type Parameter:** TAbstract (no constraints)

The abstract type.

**Type Parameter:** TConcrete : class, TAbstract, new()

The concrete type.

**Parameter:** mappingBehavior

The mapping behavior.

**Exception:** Manatee.Json.Serialization.JsonTypeMapException`2

Thrown if TConcrete is an
abstract class or an interface.

### void MapGeneric(Type tAbstract, Type tConcrete, MapBaseAbstractionBehavior mappingBehavior)

Applies a mapping from an open generic abstraction to an open generic concrete type.

**Parameter:** tAbstract

The abstract type.

**Parameter:** tConcrete

The concrete type.

**Parameter:** mappingBehavior

The mapping behavior.

**Exception:** Manatee.Json.Serialization.JsonTypeMapException

Thrown if *tConcrete* is an
abstract class or an interface or if *tConcrete* does not inherit
from *tAbstract*.

### void RemoveMap&lt;TAbstract&gt;(bool removeRelated)

Removes a previously-assigned mapping.

**Type Parameter:** TAbstract (no constraints)

The type to remove.

**Parameter:** removeRelated

Optionally removes mappings of base and related interface types.

# CustomSerializations

Manages methods for serializing object types which do not implement [IJsonSerializable](API-Serializer#ijsonserializable) and
cannot be automatically serialized.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- CustomSerializations

## Constructors

### CustomSerializations()

Creates a new [CustomSerializations](API-Serializer#customserializations) instance.

### CustomSerializations(CustomSerializations other)

Creates a new [CustomSerializations](API-Serializer#customserializations) instance using another as a basis.

**Parameter:** other

## Properties

### static [CustomSerializations](API-Serializer#customserializations) Default { get; }

Provides a default custom serialization set for new [JsonSerializer](API-Serializer#jsonserializer) instances.

## Methods

### bool IsRegistered&lt;T&gt;()

Gets whether a given type has been entered into the registry.

**Type Parameter:** T (no constraints)

The type.

**Returns:** True if an entry exists for the type; otherwise false.

### bool IsRegistered(Type type)

Gets whether a given type has been entered into the registry.

**Parameter:** type

The type.

**Returns:** True if an entry exists for the type; otherwise false.

# IJsonSerializable

Provides implementers the option to set a preferred method for serialization.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- IJsonSerializable

## Methods

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

# JsonIgnoreAttribute

Applied to properties to indicate that they are not to be serialized.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- Attribute
- JsonIgnoreAttribute

# JsonMapToAttribute

Allows the user to specify how a property is mapped during serialization.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- Attribute
- JsonMapToAttribute

## Constructors

### JsonMapToAttribute(string key)

Creates a new instance fo the [JsonMapToAttribute](API-Serializer#jsonmaptoattribute) class.

**Parameter:** key

The JSON object key.

## Properties

### string MapToKey { get; }

Specifies the key in the JSON object which maps to the property to which
this attribute is applied.

# SchemaAttribute

Indicates that a type should be validated by a JSON Schema before deserializing.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- Attribute
- SchemaAttribute

## Constructors

### SchemaAttribute(string source)

Creates a new instance of the [SchemaAttribute](API-Serializer#schemaattribute).

**Parameter:** source

The source of the schema.

## Properties

### string Source { get; }

The source of the schema. May be an absolute URI or the name of a static property defined on the type.

# JsonSerializationException

Thrown when an error occurs during serialization or deserialization.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- Exception
- JsonSerializationException

## Constructors

### JsonSerializationException(string message, Exception innerException)

Creates a new instance of the [JsonSerializationException](API-Serializer#jsonserializationexception) class.

**Parameter:** message

**Parameter:** innerException

### Exception(string message, Exception innerException)

Creates a new instance of the [JsonSerializationException](API-Serializer#jsonserializationexception) class.

**Parameter:** message

**Parameter:** innerException

# TypeDoesNotContainPropertyException

Optionally thrown when deserializing and the JSON structure contains property names
which are not valid for the type requested.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- Exception
- TypeDoesNotContainPropertyException

## Properties

### [JsonValue](API-Json#jsonvalue) Json { get; }

Gets the portion of the JSON structure which contain the invalid properties.

### Type Type { get; }

Gets the type.

# TypeInstantiationException

Thrown when a type cannot be instantiated.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- Exception
- TypeInstantiationException

## Constructors

### TypeInstantiationException(Type type)

Creates a new instance of the [TypeInstantiationException](API-Serializer#typeinstantiationexception) class.

**Parameter:** type

The type which could not be instantiated.

