---
title: Basic JSON
category: API Reference
order: 1
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# JsonValue

Represents a JSON value.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- JsonValue

#### Remarks

A value can consist of a string, a numerical value, a boolean (true or false), a null
placeholder, a JSON array of values, or a nested JSON object.

## Constructors

### JsonValue()

Creates a null [JsonValue](API-Json#jsonvalue).

### JsonValue(string s)

Creates a [JsonValue](API-Json#jsonvalue) from a string.

**Parameter:** s

### JsonValue(JsonObject o)

Creates a [JsonValue](API-Json#jsonvalue) from a JSON object.

**Parameter:** o

### JsonValue(JsonArray a)

Creates a [JsonValue](API-Json#jsonvalue) from a JSON array.

**Parameter:** a

### JsonValue(JsonValue other)

Creates a copy of a [JsonValue](API-Json#jsonvalue).

**Parameter:** other

## Fields

### static [JsonValue](API-Json#jsonvalue) Null

Globally defined null-valued JSON value.

#### Remarks

Use this when initializing a JSON object or array.

## Properties

### [JsonArray](API-Json#jsonarray) Array { get; }

Accesses the [JsonValue](API-Json#jsonvalue) as a JSON array.

**Exception:** Manatee.Json.JsonValueIncorrectTypeException

Thrown when this [JsonValue](API-Json#jsonvalue) does not contain a Json array.

#### Remarks

Setting the value as a JSON array will automatically change the [JsonValue](API-Json#jsonvalue)&#39;s type and
discard the old data.

### bool Boolean { get; }

Accesses the [JsonValue](API-Json#jsonvalue) as a boolean.

**Exception:** Manatee.Json.JsonValueIncorrectTypeException

Thrown when this [JsonValue](API-Json#jsonvalue) does not contain a boolean.

#### Remarks

Setting the value as a boolean will automatically change the [JsonValue](API-Json#jsonvalue)&#39;s type and
discard the old data.

### double Number { get; }

Accesses the [JsonValue](API-Json#jsonvalue) as a numeric value.

**Exception:** Manatee.Json.JsonValueIncorrectTypeException

Thrown when this [JsonValue](API-Json#jsonvalue) does not contain a numeric value.

#### Remarks

Setting the value as a numeric value will automatically change the [JsonValue](API-Json#jsonvalue)&#39;s type and
discard the old data.

### [JsonObject](API-Json#jsonobject) Object { get; }

Accesses the [JsonValue](API-Json#jsonvalue) as a JSON object.

**Exception:** Manatee.Json.JsonValueIncorrectTypeException

Thrown when this [JsonValue](API-Json#jsonvalue) does not contain a Json object.

#### Remarks

Setting the value as a JSON object will automatically change the [JsonValue](API-Json#jsonvalue)&#39;s type and
discard the old data.

### string String { get; }

Accesses the [JsonValue](API-Json#jsonvalue) as a string.

**Exception:** Manatee.Json.JsonValueIncorrectTypeException

Thrown when this [JsonValue](API-Json#jsonvalue) does not contain a string.

#### Remarks

Setting the value as a string will automatically change the [JsonValue](API-Json#jsonvalue)&#39;s type and
discard the old data.

### [JsonValueType](API-Json#jsonvaluetype) Type { get; }

Gets the value type of the existing data.

## Methods

### static bool op_Equality(JsonValue a, JsonValue b)



**Parameter:** a



**Parameter:** b



**Returns:** 

### static bool op_Inequality(JsonValue a, JsonValue b)



**Parameter:** a



**Parameter:** b



**Returns:** 

### static [JsonValue](API-Json#jsonvalue) Parse(string source)

Parses a System.String containing a JSON value.

**Parameter:** source

the System.String to parse.

**Returns:** The JSON value represented by the System.String.

**Exception:** System.ArgumentNullException

Thrown if *source* is null.

**Exception:** System.ArgumentException

Thrown if *source* is empty or whitespace.

**Exception:** Manatee.Json.JsonSyntaxException

Thrown if *source* contains invalid JSON syntax.

### static [JsonValue](API-Json#jsonvalue) Parse(TextReader stream)

Parses data from a System.IO.StreamReader containing a JSON value.

**Parameter:** stream

the System.IO.StreamReader to parse.

**Returns:** The JSON value represented by the System.String.

**Exception:** System.ArgumentNullException

Thrown if *stream* is null.

**Exception:** System.ArgumentException

Thrown if *stream* is at the end.

**Exception:** Manatee.Json.JsonSyntaxException

Thrown if *stream* contains invalid JSON syntax.

### static Task&lt;JsonValue&gt; ParseAsync(TextReader stream)

Parses data from a System.IO.StreamReader containing a JSON value.

**Parameter:** stream

the System.IO.StreamReader to parse.

**Returns:** The JSON value represented by the System.String.

**Exception:** System.ArgumentNullException

Thrown if *stream* is null.

**Exception:** System.ArgumentException

Thrown if *stream* is at the end.

**Exception:** Manatee.Json.JsonSyntaxException

Thrown if *stream* contains invalid JSON syntax.

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The System.Object to compare with the current System.Object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### bool Equals(JsonValue other)

Indicates whether the current object is equal to another object of the same type.

**Parameter:** other

An object to compare with this object.

**Returns:** true if the current object is equal to the *other* parameter; otherwise, false.

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The System.Object to compare with the current System.Object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### int GetHashCode()

Serves as a hash function for a particular type.

**Returns:** A hash code for the current System.Object.

#### Filterpriority

2

### string GetIndentedString(int indentLevel)

Creates a string representation of the JSON data.

**Parameter:** indentLevel

The indention level for the value.

**Returns:** A string.

### string ToString()

Creates a string that represents this [JsonValue](API-Json#jsonvalue).

**Returns:** A string representation of this [JsonValue](API-Json#jsonvalue).

#### Remarks

Passing the returned string back into the parser will result in a copy of
this [JsonValue](API-Json#jsonvalue).

# JsonValueType

Specifies various types of values for use in a JSON key:value pair.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- JsonValueType

## Fields

### Number

Indicates that the Json key:value pair contains a numeric value (double).

### String

Indicates that the Json key:value pair contains a string.

### Boolean

Indicates that the Json key:value pair contains a boolean value.

### Object

Indicates that the Json key:value pair contains a nested Json object.

### Array

Indicates that the Json key:value pair contains a Json array.

### Null

Indicates that the Json key:value pair contains a null value.

# JsonValueIncorrectTypeException

Thrown when a value is accessed via the incorrect type accessor.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- Exception
- SystemException
- InvalidOperationException
- JsonValueIncorrectTypeException

## Properties

### [JsonValueType](API-Json#jsonvaluetype) RequestedType { get; }

The type requested.

### [JsonValueType](API-Json#jsonvaluetype) ValidType { get; }

The correct type for the [JsonValue](API-Json#jsonvalue) that threw the exception.

# JsonObject

Represents a collection of key:value pairs in a JSON structure.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- Dictionary&lt;string, JsonValue&gt;
- JsonObject

#### Remarks

A key is always represented as a string. A value can consist of a string, a numerical value, 
a boolean (true or false), a null placeholder, a JSON array of values, or a nested JSON object.

## Constructors

### JsonObject()

Creates an empty instance of a JSON object.

## Properties

### [JsonValue](API-Json#jsonvalue) this[string key] { get; set; }

Gets or sets the value associated with the specified key.

**Parameter:** key

The key of the value to get or set.

#### Returns

The value associated with the specified key.

### [JsonValue](API-Json#jsonvalue) this[string key] { get; set; }

Gets or sets the value associated with the specified key.

**Parameter:** key

The key of the value to get or set.

#### Returns

The value associated with the specified key.

## Methods

### void Add(string key, JsonValue value)

Adds the specified key and value to the dictionary.

**Parameter:** key

The key of the element to add.

**Parameter:** value

The value of the element to add. If the value is null,
it will be replaced by [JsonValue.Null](API-Json#static-jsonvalue-null).

### void Add(string key, JsonValue value)

Adds the specified key and value to the dictionary.

**Parameter:** key

The key of the element to add.

**Parameter:** value

The value of the element to add. If the value is null,
it will be replaced by [JsonValue.Null](API-Json#static-jsonvalue-null).

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The System.Object to compare with the current System.Object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The System.Object to compare with the current System.Object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### int GetHashCode()

Serves as a hash function for a particular type.

**Returns:** A hash code for the current System.Object.

#### Filterpriority

2

### string GetIndentedString(int indentLevel)

Creates a string representation of the JSON data.

**Parameter:** indentLevel

The indention level for the object.

**Returns:** A string.

### string ToString()

Creates a string representation of the JSON data.

**Returns:** A string.

#### Remarks

Passing the returned string back into the parser will result in a copy of
this JSON object.

# JsonArray

Represents a collection of JSON values.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- List&lt;JsonValue&gt;
- JsonArray

#### Remarks

A value can consist of a string, a numeric value, a boolean (true or false), a null placeholder,
a JSON array of values, or a nested JSON object.

## Constructors

### JsonArray()

Creates an empty instance of a JSON array.

## Properties

### [ArrayEquality](API-Json#arrayequality) EqualityStandard { get; set; }

Defines how this [JsonArray](API-Json#jsonarray) evaluates equality.

## Methods

### void Add(JsonValue item)

Adds an object to the end of the [JsonArray](API-Json#jsonarray).

**Parameter:** item

The object to be added to the end of the [JsonArray](API-Json#jsonarray).
If the value is null, it will be replaced by [JsonValue.Null](API-Json#static-jsonvalue-null).

### void Add(JsonValue item)

Adds an object to the end of the [JsonArray](API-Json#jsonarray).

**Parameter:** item

The object to be added to the end of the [JsonArray](API-Json#jsonarray).
If the value is null, it will be replaced by [JsonValue.Null](API-Json#static-jsonvalue-null).

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The System.Object to compare with the current System.Object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### bool Equals(Object obj)

Determines whether the specified System.Object is equal to the current System.Object.

**Parameter:** obj

The System.Object to compare with the current System.Object.

**Returns:** true if the specified System.Object is equal to the current System.Object; otherwise, false.

#### Filterpriority

2

### int GetHashCode()

Serves as a hash function for a particular type.

**Returns:** A hash code for the current System.Object.

#### Filterpriority

2

### string GetIndentedString(int indentLevel)

Creates a string representation of the JSON data.

**Parameter:** indentLevel

The indention level for the array.

**Returns:** A string.

### string ToString()

Creates a string representation of the JSON data.

**Returns:** A string.

#### Remarks

Passing the returned string back into the parser will result in a copy of
this Json array.

# JsonOptions

Provides some configurability around the basic JSON entities.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- JsonOptions

## Properties

### static [ArrayEquality](API-Json#arrayequality) DefaultArrayEquality { get; set; }

Defines a default value for [EqualityStandard](API-Json#arrayequality-equalitystandard--get-set-).
The default is [ArrayEquality.SequenceEqual](API-Json#sequenceequal).

### static [DuplicateKeyBehavior](API-Json#duplicatekeybehavior) DuplicateKeyBehavior { get; set; }

Defines the how duplicate keys are handled for [JsonObject](API-Json#jsonobject)s.
The default is [DuplicateKeyBehavior.Throw](API-Json#throw).

### static char PrettyPrintIndentChar { get; set; }

Determines the indention character to use when calling Manatee.Json.JsonValue.GetIndentedString(System.Int32).
The default is a single tab.

### static bool ThrowOnIncorrectTypeAccess { get; set; }

Defines whether [JsonValue](API-Json#jsonvalue) should throw an exception when being accessed by the
wrong accessory type (e.g. accessing an array as a boolean). The default is true.

# DuplicateKeyBehavior

Defines behavior of [JsonObject](API-Json#jsonobject) when adding items at already exist.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- DuplicateKeyBehavior

## Fields

### Throw

Throw an exception.

### Overwrite

Overwrite the existing item.

# ArrayEquality

Defines different kinds of array equality.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- ArrayEquality

## Fields

### SequenceEqual

Defines that all elements in both arrays must match and be in the same sequence.

### ContentsEqual

Defines that all elements in both arrays much match, but may appear in any sequence.

# JsonSyntaxException

Thrown when an input string contains a syntax error while parsing a [JsonObject](API-Json#jsonobject), [JsonArray](API-Json#jsonarray), or [JsonValue](API-Json#jsonvalue).

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- Exception
- JsonSyntaxException

## Properties

### string Message { get; }

Gets a message that describes the current exception.

#### Returns

The error message that explains the reason for the exception, or an empty string(&quot;&quot;).

#### Filterpriority

1

### string Path { get; }

Gets the path up to the point at which the error was found.

