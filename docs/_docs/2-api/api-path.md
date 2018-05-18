---
title: JSON Path
category: API Reference
order: 12
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# JsonPath

Provides primary functionality for JSON Path objects.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Path

**Inheritance hierarchy:**

- Object
- JsonPath

## Methods

### static [JsonPath](API-Path#jsonpath) Parse(string source)

Parses a System.String containing a JSON path.

**Parameter:** source

the System.String to parse.

**Returns:** The JSON path represented by the System.String.

**Exception:** System.ArgumentNullException

Thrown if *source* is null.

**Exception:** System.ArgumentException

Thrown if *source* is empty or whitespace.

**Exception:** Manatee.Json.Path.JsonPathSyntaxException

Thrown if *source* contains invalid JSON path syntax.

### bool Equals(JsonPath other)

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

### [JsonArray](API-Json#jsonarray) Evaluate(JsonValue json)

Evaluates a JSON value using the path.

**Parameter:** json

The [JsonValue](API-Json#jsonvalue) to evaulate.

**Returns:** 

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

# JsonPathWith

Provides methods to be used when working with JSON Paths.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Path

**Inheritance hierarchy:**

- Object
- JsonPathWith

## Methods

### static [JsonPath](API-Path#jsonpath) Array(Slice[] slices)

Appends a [JsonPath](API-Path#jsonpath) by specifying a series of array indicies.

**Parameter:** slices

The indices and slices of the [JsonValue](API-Json#jsonvalue)s to include.

**Returns:** The new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) Array(JsonPath path)

Appends a [JsonPath](API-Path#jsonpath) by including all array values.

**Parameter:** path

The [JsonPath](API-Path#jsonpath) to extend.

**Returns:** The new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) Array(JsonPath path, Slice[] slices)

Appends a [JsonPath](API-Path#jsonpath) by specifying a series of array indicies.

**Parameter:** path

The [JsonPath](API-Path#jsonpath) to extend.

**Parameter:** slices

The indices of the [JsonValue](API-Json#jsonvalue)s to include.

**Returns:** The new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) Length()

Creates a new [JsonPath](API-Path#jsonpath) object which starts by specifying an array length.

**Returns:** A new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) Length(JsonPath path)

Appends a [JsonPath](API-Path#jsonpath) by specifying an array length.

**Parameter:** path

The [JsonPath](API-Path#jsonpath) to extend.

**Returns:** The new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) Name(string name)

Creates a new [JsonPath](API-Path#jsonpath) object which starts by specifying an object property.

**Parameter:** name

The name to follow.

**Returns:** A new [JsonPath](API-Path#jsonpath).

#### Remarks

If *name* is &quot;length&quot;, operates as Manatee.Json.Path.JsonPathWith.Length

### static [JsonPath](API-Path#jsonpath) Name(JsonPath path, string name)

Appends a [JsonPath](API-Path#jsonpath) by specifying an object property.

**Parameter:** path

The [JsonPath](API-Path#jsonpath) to extend.

**Parameter:** name

The name to follow.

**Returns:** The new [JsonPath](API-Path#jsonpath).

#### Remarks

If *name* is &quot;length&quot;, operates as Manatee.Json.Path.JsonPathWith.Length(Manatee.Json.Path.JsonPath)

### static [JsonPath](API-Path#jsonpath) Search(string name)

Creates a new [JsonPath](API-Path#jsonpath) object which starts by searching for an object property.

**Parameter:** name

The name to search for.

**Returns:** A new [JsonPath](API-Path#jsonpath).

#### Remarks

If *name* is &quot;length&quot;, operates as Manatee.Json.Path.JsonPathWith.SearchLength

### static [JsonPath](API-Path#jsonpath) Search(JsonPath path, string name)

Appends a [JsonPath](API-Path#jsonpath) by searching for an object property.

**Parameter:** path

The [JsonPath](API-Path#jsonpath) to extend.

**Parameter:** name

The name to follow.

**Returns:** The new [JsonPath](API-Path#jsonpath).

#### Remarks

If *name* is &quot;length&quot;, operates as Manatee.Json.Path.JsonPathWith.SearchLength(Manatee.Json.Path.JsonPath)

### static [JsonPath](API-Path#jsonpath) SearchArray(Slice[] slices)

Appends a [JsonPath](API-Path#jsonpath) by specifying a series of array indicies.

**Parameter:** slices

The indices and slices of the [JsonValue](API-Json#jsonvalue)s to include.

**Returns:** The new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) SearchArray(JsonPath path, Slice[] slices)

Appends a [JsonPath](API-Path#jsonpath) by specifying a series of array indicies.

**Parameter:** path

The [JsonPath](API-Path#jsonpath) to extend.

**Parameter:** slices

The indices and slices of the [JsonValue](API-Json#jsonvalue)s to include.

**Returns:** The new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) SearchLength()

Creates a new [JsonPath](API-Path#jsonpath) object which starts by searching for array lengths.

**Returns:** A new [JsonPath](API-Path#jsonpath).

### static [JsonPath](API-Path#jsonpath) SearchLength(JsonPath path)

Appends a [JsonPath](API-Path#jsonpath) by searching for array lengths.

**Parameter:** path

The [JsonPath](API-Path#jsonpath) to extend.

**Returns:** The new [JsonPath](API-Path#jsonpath).

# JsonPathRoot

Provides extension methods which can be used within array and search JSON Path queries.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Path

**Inheritance hierarchy:**

- Object
- JsonPathRoot

## Methods

### static Manatee.Json.Path.JsonPathValue ArrayIndex(int index)

Determines if an object contains a property containing a number and retrieves its value.

**Parameter:** index

The index to retreive.

**Returns:** The value if the property exists and is a number; otherwise null.

### static bool HasProperty(string name)

Determines if an object contains a property or, if its value is a boolean, whether the value is true.

**Parameter:** name

The name of the property.

**Returns:** true if the value is an object and contains key *name* or if its value is true; otherwise false.

### static int Length()

Specifies the length of a [JsonArray](API-Json#jsonarray).

**Returns:** The length of the array.

### static Manatee.Json.Path.JsonPathValue Name(string name)

Determines if an object contains a property containing a number and retrieves its value.

**Parameter:** name

The name of the property.

**Returns:** The value if the property exists and is a number; otherwise null.

# PathExpressionExtensions

Provides extension methods which can be used within array and search JSON Path queries.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Path

**Inheritance hierarchy:**

- Object
- PathExpressionExtensions

## Methods

### static Manatee.Json.Path.JsonPathValue ArrayIndex(JsonPathValue json, int index)

Determines if an object contains a property containing a number and retrieves its value.

**Parameter:** json

The value.

**Parameter:** index

The index to retreive.

**Returns:** The value if the property exists and is a number; otherwise null.

### static bool HasProperty(JsonPathValue json, string name)

Determines if an object contains a property or, if its value is a boolean, whether the value is true.

**Parameter:** json

The value.

**Parameter:** name

The name of the property.

**Returns:** true if the value is an object and contains key *name* or if its value is true; otherwise false.

### static int IndexOf(JsonPathValue json, JsonValue value)

Gets the index of a value within an array.

**Parameter:** json

The value.

**Parameter:** value

The query.

**Returns:** The index of the requested value or -1 if the value does not exist.

### static int Length(JsonPathArray json)

Specifies the length of a [JsonArray](API-Json#jsonarray).

**Parameter:** json

The array.

**Returns:** The length of the array.

### static int Length(JsonPathValue json)

Specifies the length of a [JsonArray](API-Json#jsonarray).

**Parameter:** json

The array.

**Returns:** The length of the array.

### static Manatee.Json.Path.JsonPathArray Name(JsonPathArray json, string name)

Specifies the length of a [JsonArray](API-Json#jsonarray).

**Parameter:** json

The array.

**Parameter:** name

The name of the property.

**Returns:** The length of the array.

### static Manatee.Json.Path.JsonPathValue Name(JsonPathValue json, string name)

Determines if an object contains a property containing a number and retrieves its value.

**Parameter:** json

The value.

**Parameter:** name

The name of the property.

**Returns:** The value if the property exists and is a number; otherwise null.

# JsonPathSyntaxException

Thrown when an input string contains a syntax error while parsing a [JsonPath](API-Path#jsonpath).

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Path

**Inheritance hierarchy:**

- Object
- Exception
- JsonPathSyntaxException

## Properties

### string Message { get; }

Gets a message that describes the current exception.

#### Returns

The error message that explains the reason for the exception, or an empty string(&quot;&quot;).

#### Filterpriority

1

### string Path { get; }

Gets the path up to the point at which the error was found.

