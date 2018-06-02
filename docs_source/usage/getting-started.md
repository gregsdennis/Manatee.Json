# Getting Started

## Modelling JSON

The structure of JSON is quite simple.  You can read about the language on [json.org](http://json.org).

Each of the JSON constructs can be mapped to a .Net construct as follows:

<table>
    <tr>
        <th>JSON</th>
        <th>↔</th>
        <th>.Net</th>
    </tr>
    <tr>
        <td>object</td>
        <td>↔</td>
        <td>Dictionary</td>
    </tr>
    <tr>
        <td>array</td>
        <td>↔</td>
        <td>List</td>
    </tr>
    <tr>
        <td>boolean</td>
        <td>↔</td>
        <td>Boolean</td>
    </tr>
    <tr>
        <td>numeric</td>
        <td>↔</td>
        <td>Double</td>
    </tr>
    <tr>
        <td>string</td>
        <td>↔</td>
        <td>String</td>
    </tr>
</table>

This JSON structure is implemented in .Net through three classes: `JsonValue`, `JsonObject`, and `JsonArray`.

The `JsonValue` class is a container class for all of the JSON types.  In order for everything to remain strongly typed, the class exposes a property for each JSON type.  To implement the JSON null value, the static member `JsonValue.Null` was created.

The `JsonObject` class is implemented as `Dictionary<string, JsonValue>`.  As such it can be navigated and manipulated as any other dictionary instance would be.

Similarly to the `JsonObject` class, the `JsonArray` class is implemented as `List<JsonValue>`, which can be navigated and manipulated directly as a list instance.

> **NOTE** Because of `JsonObject`'s and `JsonArray`'s inheritance structure, they are fully LINQ-compatible!

Each of these three classes override the `ToString()` method to correctly output in a JSON format.

Implicit casts from `JsonObject`, `JsonArray`, `string`, `double`, and `bool` were created in `JsonValue` to simplify coding and readability.  Also, comparison operators were overridden for the `JsonValue` class.  You can read more on these casts and operators below.

All of the extended JSON functionality supported by this library has dedicated object models, where applicable.

## Working with JSON directly

JSON constructs can be created directly through the use of the implicit cast operators:

```csharp
JsonValue jsonBool = false;
JsonValue jsonNum = 42;
JsonValue jsonString = "aString";
JsonValue jsonObject = new JsonObject { {"aKey", 42} };
JsonValue jsonArray = new JsonArray {4, true, "aValue"};
```

The above code creates five `JsonValue` instances.  To access these values, use these properties:

```csharp
jsonBool.Boolean
jsonNum.Number
jsonString.String
jsonObject.Object
jsonArray.Array
```

If a get accessor is used that does not correspond with the `JsonValue`’s type, an exception is thrown.  The default constructor for `JsonValue` creates a Null value.

These declarations can be combined in the same way as when declaring any other object.  For example, a moderately complex `JsonObject` can be built as follows:

```csharp
var json = new JsonObject
    {
        {"boolean", true},
        {"number", 42},
        {"string", "a string"},
        {"null", JsonValue.Null},
        {"array", new JsonArray {6.7, true, "a value"}},
        {"object", new JsonObject
            {
                {"aKey", 42},
                {"anotherKey", false}
            }
        }
    };
```

The object’s structure, and ultimately its output, is quite apparent directly from the code that created it.  Note also that we're not just building a string value to be parsed; we're actually building the object model, which will be checked at compile time, practically eliminating the occurrence of typographical errors.

Since `JsonObject` and `JsonArray` are implemented as strongly typed collections, all of the underlying operations (e.g. `Add()`, `AddRange()`, etc.) are accessible, including LINQ.  As such, the following statements are valid:

```csharp
json.Add("newItem", "a new string");
var onlyStrings = json.Select(jkv => jkv.Value.Type == JsonValueType.String).ToJson();
```

Here, the `ToJson()` method is an extension method on the `IEnumerable<KeyValuePair<string, JsonValue>>` type returned by the LINQ `Select()` method.  It returns a `JsonObject`.  There is also a corresponding `ToJson()` method for the `IEnumerable<JsonValue>` which returns a `JsonArray`.

As you can see, creating these constructs is quite easy and very readable.  As is expected, calling `json.ToString()` yields:

```json
{"boolean":true,"number":42,"string":"a string","null":null,"array":[6.7,true,"a value"],"object":{"aKey":42,"anotherKey":false}}
```

Furthermore, feeding this output back into the `JsonObject` constructor yields the original structure (although using new instances).

## Getting JSON Output

There are two methods that create output:  `ToString()`, which simply outputs the most concise JSON (single line, no white spacing), and `GetIndentedString()`, which outputs a multiline, indented string.

## Handling Errors

While parsing JSON data (through either the `JsonObject` constructor or `JsonValue.Parse()`), errors may occur.  These errors will be reported by throwing a `JsonSyntaxException`.  This exception exposes a Path property which contains a path to the error in [JSONPath syntax](path.html).  The messaging has been designed to direct the user directly to the error.

Here are a few examples:

<table>
    <tr>
        <th>JSON</th>
        <th>Error</th>
        <th>Error Message</th>
    </tr>
    <tr>
        <td><code>{"first":4,"int":no}</code></td>
        <td><code>no</code> is not a predefined JSON value</td>
        <td>Value not recognized: 'no} '. Path: '$.int'</td>
    </tr>
    <tr>
        <td><code>["first",4,"int",no]</code></td>
        <td><code>no</code> is not a predefined JSON value</td>
        <td>Value not recognized: 'no] '. Path: '$[3]'</td>
    </tr>
    <tr>
        <td><code>{"first":4,int:"no"}</code></td>
        <td><code>int</code> should be a string value: <code>"int"</code></td>
        <td>Expected key. Path: '$.first'</td>
    </tr>
</table>

As shown, the error message will give information as to what went wrong and where the error occurred.  The location is given in [JSONPath](http://goessner.net/articles/JsonPath/) notation.  In the last example, the key could not be determined from the input, so it gave the last-recognized key.

`JsonValue.Parse()` will fail quickly at the first error.
