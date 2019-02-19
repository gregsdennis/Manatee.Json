# Welcome!

The primary goal of Manatee.Json is to make working with JSON simple and intuitive for the developer.  This library recognizes that JSON is much more than just a mechanism for data transfer.

Secondarily, Manatee.Json is *intended* to be strictly compliant with RFC-8259, which means that it purposefully does not support JSON variants, like single-quoted strings or BSON.

## Read from a file

    var text = File.ReadAllText("content.json");
    var json = JsonValue.Parse(text);

The `json` field now contains the content of the *.json* file.  The object structure is exactly what you'd expect by looking at the file.

## Object model

These JSON types map to primitive .Net types:

- `true`/`false` as `Boolean`,
- numbers as `Double`,
- strings as `String`

These JSON types map to types defined in Manatee.Json:

- objects (`{"key":"value", "otherKey":9}`) as `JsonObject` which derives from `Dictionary<string, JsonValue>`
- arrays (`["value", 9]` as `JsonArray` which derives from `List<JsonValue>`

All of these types are encapsulated in a container type, `JsonValue`.  This type exposes a property for each value type, as well as a property for which type the value contains.

For the JSON `null` there is a readonly static `JsonValue.Null` field.

## Building JSON manually

Manatee.Json defines implicit conversions to `JsonValue` from `Boolean`, `Double`, `String`, `JsonObject`, and `JsonArray`.  This helps greatly in building complex objects manually.

    JsonValue str = "value",
              num = 10,
              boolean = false;

Because the collection types are derived from core .Net types, you also get all of the initialization capabilities.

    var obj = new JsonObject
        {
            ["key"] = "value",
            ["otherKey"] = 9
        }
    var array = new JsonArray { "value", 9, obj };

## Serialization

Converting .Net objects to and from JSON is also simple:

1. Create a serializer

        var serializer = new JsonSerializer();

2. De/Serialize

        var myObject = serializer.Deserialize<MyObject>(json);
        var backToJson = serializer.Serialize(myObject);

There are many ways to customize serialization.  See the wiki page for more details!

## But wait, there's more!

Manatee.Json also:

- Is covered by over 2000 unit tests
- Conforms to RFC-8259: The JSON specification
- Supports:
    - .Net Framework 4.5
    - .Net Standard 1.3
    - .Net Standard 2.0
- Outputs compact and prettified JSON text
- Supports [JSON Schema](http://json-schema.org/) **INCLUDED AND FREE!**
    - Draft 4
    - Draft 6
    - Draft 7
    - Draft 8 (spec to be released soon)
    - Native object model
    - Output contains all info required to craft custom error messages
    - User-defined keywords
- Supports [JSONPath](http://goessner.net/articles/JsonPath/)
    - Native object model
    - Compile-time checking
- Supports [JsonPatch](http://jsonpatch.com/) (with object model)
- Supports [JSON Pointer](https://tools.ietf.org/html/rfc6901) (with object model)
- Is fully LINQ-compatible
- Converts between JSON and XML
- Reports parsing errors using JSON Pointer to identify location
- Supports streamed parsing
- Is fully open-source under the MIT license

Serialization features:

- De/Serialize abstraction types (abstract classes and interfaces) by type registration
- De/Serialize dynamic types
- JIT type creation for unregistered abstraction types
- De/Serialize anonymous types
- Fully customizable serialization of both 1st- and 3rd-party types
- De/Serialize static types/properties
- De/Serialize fields
- De/Serialize enumerations by name or numeric value
- Maintain object references/graphs
- De/Serialize circular references
- Optionally include type names
- Each serializer instance can be independently configured
- Supports multiple date/time formats (ISO 8601, JavaScript, custom)
- Supports using DI containers for object creation
- Supports non-default constructors
- Property name customization via attribute
- Global property name transformations
- Opt-out property inclusion via attribute
- Optionally serialize only properties for requested type or all properties defined by object

# About this library

Most JSON libraries skip over the JSON part and move right on to serialization.  Some even give extended functionality, like JSON Schema and JSONPath.

This library is different.  Rather than hiding the JSON structure, Manatee.Json proudly exposes it.  In fact, working with the object model is an integral part of using this library.

## Why create another JSON library?

I was introduced to the JSON format while working for a previous employer where we were designing remote systems for flight simulators and the JSON data format was specified by the client.  Those programs were written in C and were quite limited as they had to run on microcontrollers.

In order to test these systems, I had to write a Windows-based client with which they could communicate.  I looked at some of the solutions out there (Json.Net, etc.) and was unimpressed with how the libraries glossed over the object model.  I didn't need serialization so much as direct JSON manipulation.  So I wrote my own.
