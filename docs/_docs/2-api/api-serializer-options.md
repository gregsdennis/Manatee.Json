---
title: Serialization Options
category: API Reference
order: 4
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# JsonSerializerOptions

Represents a set of behavior options for the [JsonSerializer](API-Serializer#jsonserializer) object.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- JsonSerializerOptions

## Constructors

### JsonSerializerOptions()

Creates a new instance of [JsonSerializerOptions](API-Serializer-Options#jsonserializeroptions) with default options.

### JsonSerializerOptions(JsonSerializerOptions options)

Creates a new instance of [JsonSerializerOptions](API-Serializer-Options#jsonserializeroptions) by copying an existing
[JsonSerializerOptions](API-Serializer-Options#jsonserializeroptions) instance.

**Parameter:** options

The [JsonSerializerOptions](API-Serializer-Options#jsonserializeroptions) instance to copy.

## Fields

### static [JsonSerializerOptions](API-Serializer-Options#jsonserializeroptions) Default

Default options used by the serializer.

## Properties

### bool AutoSerializeFields { get; set; }

Gets and sets whether public fields should be serialized during autoserialization.

### bool CaseSensitiveDeserialization { get; set; }

Gets and sets whether the serializer considers case for properties while deserializing.

#### Remarks

This only affects automatic serialization.

### string CustomDateTimeSerializationFormat { get; set; }

Gets and sets a custom serialization format for System.DateTime.

### [DateTimeSerializationFormat](API-Serializer-Options#datetimeserializationformat) DateTimeSerializationFormat { get; set; }

Gets and sets the format for System.DateTime serialization using the default serializer methods.

#### Remarks

If the Manatee.Json.Serialization.JsonSerializationTypeRegistry entry for DateTime has been changed to custom
methods, this property will have no effect.

### Func&lt;string, string&gt; DeserializationNameTransform { get; set; }

Gets and sets a transformation function for property names during deserialization. Default is no transformation.

### bool EncodeDefaultValues { get; set; }

Gets and sets whether the serializer encodes default values for properties.

#### Remarks

Setting to &#39;true&#39; may significantly increase the size of the JSON structure.

### [EnumSerializationFormat](API-Serializer-Options#enumserializationformat) EnumSerializationFormat { get; set; }

Gets and sets the format for enumeration serialization using the default serializer methods.

#### Remarks

If an entry has been made in Manatee.Json.Serialization.JsonSerializationTypeRegistry for the specific type,
this property will have no effect.

### string FlagsEnumSeparator { get; set; }

Gets and sets a separator to be used when serializing enumerations marked with the System.FlagsAttribute.

### [InvalidPropertyKeyBehavior](API-Serializer-Options#invalidpropertykeybehavior) InvalidPropertyKeyBehavior { get; set; }

Gets and sets the behavior of the deserializer when a JSON structure is passed which
contains invalid property keys.

### bool OnlyExplicitProperties { get; set; }

Gets and sets whether the serializer will serialize only the properties defined by the
type given as the generic parameter.

### [PropertySelectionStrategy](API-Serializer-Options#propertyselectionstrategy) PropertySelectionStrategy { get; set; }

Gets and sets which properties are automatically included while serializing.

### [IResolver](API-Serializer-Options#iresolver) Resolver { get; set; }

Gets and sets an [IResolver](API-Serializer-Options#iresolver) implementation for instantiating objects while deserializing.

### Func&lt;string, string&gt; SerializationNameTransform { get; set; }

Gets and sets a transformation function for property names during serialization. Default is no transformation.

### [TypeNameSerializationBehavior](API-Serializer-Options#typenameserializationbehavior) TypeNameSerializationBehavior { get; set; }

Gets and sets whether the serializer always includes the type name while serializing.

#### Remarks

This only affects automatic serialization.

# InvalidPropertyKeyBehavior

Enumeration of behavior options for the deserializer when a JSON structure is passed which
contains invalid property keys.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- InvalidPropertyKeyBehavior

## Fields

### DoNothing

Deserializer ignores the invalid property keys.

### ThrowException

Deserializer will throw an exception when an invalid property key is found.

# DateTimeSerializationFormat

Available formatting options for serializing DateTime objects.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- DateTimeSerializationFormat

## Fields

### Iso8601

Output conforms to ISO 8601 formatting: YYYY-MM-DDThh:mm:ss.sTZD (e.g. 1997-07-16T19:20:30.45+01:00)

### JavaConstructor

Output is a string in the format &quot;/Date([ms])/&quot;, where [ms] is the number of milliseconds
since January 1, 1970 UTC.

### Milliseconds

Output is a numeric value representing the number of milliseconds since January 1, 1970 UTC.

### Custom

Output is formatted using the [CustomDateTimeSerializationFormat](API-Serializer-Options#string-customdatetimeserializationformat--get-set-) property.

# EnumSerializationFormat

Enumerates serialization formats for enumerations.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- EnumSerializationFormat

## Fields

### AsInteger

Instructs the serializer to convert enumeration values to their numeric
counterparts.

### AsName

Instructs the serializer to convert enumeration values to their string
counterparts.

#### Remarks

This option will use the Description attribute if it is present. If the
enumeration is marked with the flags attribute, the string representation
will consist of a comma-delimited list of names. Whenever a value is
passed which does not have a named counterpart, the numeric value will
be used.

# TypeNameSerializationBehavior

Enumerates serialization behaviors for saving type names.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- TypeNameSerializationBehavior

## Fields

### Auto

Serializes the type name as necessary.

### OnlyForAbstractions

Serializes the type name only for abstract and interface types.

### Always

Always serializes the type name.

### Never

Never serializes the type name.

# PropertySelectionStrategy

Enumerates the types of properties which are automatically serialized.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- PropertySelectionStrategy

## Fields

### ReadWriteOnly

Indicates that read/write properties will be serialized.

### ReadOnly

Indicates that read-only properties will be serialized.

### ReadAndWrite

Indicates that both read-only and read/write properties will be serialized.

# IResolver

Defines methods required to resolved instances for deserialization.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Serialization

**Inheritance hierarchy:**

- IResolver

## Methods

### T Resolve&lt;T&gt;()

Resolves an instance of the given type.

**Type Parameter:** T (no constraints)

The type to resolve.

**Returns:** An instance of the type requested.

### Object Resolve(Type type)

Resolves an instance of the given type.

**Parameter:** type

The type to resolve.

**Returns:** An instance of the type requested.

