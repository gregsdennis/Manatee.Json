# Manatee.Json

[![Join the chat at https://gitter.im/gregsdennis/Manatee.Json](https://badges.gitter.im/gregsdennis/Manatee.Json.svg)](https://gitter.im/gregsdennis/Manatee.Json?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![littlecrabsolutions MyGet Build Status](https://www.myget.org/BuildSource/Badge/littlecrabsolutions?identifier=7898edc2-8d91-411c-88c9-2023d9d9fd41)](https://www.myget.org/) <a href="http://www.jetbrains.com/resharper"><img src="http://i61.tinypic.com/15qvwj7.jpg" alt="ReSharper" title="ReSharper"></a>

The primary goal of Manatee.Json is to make working with JSON simple and intuitive for the developer.  This library recognizes that JSON is much more than just a mechanism for data transfer.

Secondarily, Manatee.Json is *intended* to be strictly ECMA-404 compliant, which means that it purposefully does not support JSON variants, like single-quoted strings or BSON.

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

- Conforms to ECMA-404: The JSON specification
- Supports .Net Standard 1.3
- Outputs compact and prettified JSON text
- Supports [JSON Schema](http://json-schema.org/) (both draft-04 and draft-06) **INCLUDED AND FREE!** (with object model)
- Supports [JSONPath](http://goessner.net/articles/JsonPath/) (with object model and compile-time checking)
- Supports [JsonPatch](http://jsonpatch.com/) (with object model)
- Is fully LINQ-compatible
- Converts between JSON and XML
- Reports parsing errors using JSONPath to identify location
- Supports streamed parsing
- Is fully open-source under the MIT license

Serialization features:

- De/Serialize abstraction types (abstract classes and interfaces) by type registration
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

See the wiki pages for more information on how to use this wonderful library!

## Contributing

If you'd like to contribute to Manatee.Json, please feel free to fork and create a pull request.

### The Project

This code uses C# 7 features, so a compiler/IDE that supports these features is required.

The project is a single project that targets both .Net Framework 4.6 and .Net Standard 1.3.

### Building

During development, building within Visual Studio should be fine.  There is a build script in the root directory, but I use that for CI and to generate Nuget packages.

### Code style and maintenance

I use [Jetbrains Resharper](https://www.jetbrains.com/resharper/) in Visual Studio [JetBrains Rider](https://www.jetbrains.com/rider/) to maintain the code style (and for many of the other things that it does).  The solution is set up with team style settings, so if you're using Resharper the settings should automatically load.  Please follow the suggestions.
