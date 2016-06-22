#Manatee.Json

[![Join the chat at https://gitter.im/gregsdennis/Manatee.Json](https://badges.gitter.im/gregsdennis/Manatee.Json.svg)](https://gitter.im/gregsdennis/Manatee.Json?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

The primary goal of Manatee.Json is to make working with JSON simple and intuitive for the developer.  This library recognizes that JSON is much more than just a mechanism for data transfer.

##Read from a file

    var text = File.ReadAllText("content.json");
    var json = JsonValue.Parse(text);

The `json` field now contains the content of the *.json* file.  The object structure is exactly what you'd expect by looking at the file.

##Object model

These JSON types map to primitive .Net types:

- `true`/`false` as `Boolean`,
- numbers as `Double`,
- strings as `String`

These JSON types map to types defined in Manatee.Json:

- objects (`{"key":"value", "otherKey":9}`) as `JsonObject` which derives from `Dictionary<string, JsonValue>`
- arrays (`["value", 9]` as `JsonArray` which derives from `List<JsonValue>`

All of these types are encapsulated in a container type, `JsonValue`.  This type exposes a property for each value type, as well as a property for which type the value contains.

For the JSON `null` there is a readonly static `JsonValue.Null` field.

##Building JSON manually

Manatee.Json defines implicit conversions to `JsonValue` from `Boolean`, `Double`, `String`, `JsonObject`, and `JsonArray`.  This helps greatly in building complex objects manually.

    JsonValue str = "value",
              num = 10,
              boolean = false;

Because the collection types are derived from common .Net types, you get all of the initialization capabilities.

    var obj = new JsonObject
        {
            ["key"] = "value",
            ["otherKey"] = 9
        }
    var array = new JsonArray { "value", 9, obj };

##Serialization

Converting .Net object to and from JSON is also simple:

1. Create a serializer

        var serializer = new JsonSerializer();

2. De/Serialize

        var myObject = serializer.Deserialize<MyObject>(json);
        var backToJson = serializer.Serialize(myObject);

There are many ways to customize serialization.  See the wiki page for more details!

##But wait, there's more!

Manatee.Json also:

- Conforms to ECMA-404: The JSON specification
- Supports .Net 3.5+
- Outputs compact and prettified JSON text
- Supports JsonSchema **INCLUDED AND FREE!** (with similar object model)
- Supports JSONPath (with object model and compile-time checking)
- Is fully LINQ-compatible
- Converts between JSON and XML
- Parsing errors are reported using JSONPath to identify location
- Supports streamed parsing
- Is fully open-source
- Uses the Apache 2.0 license

Serialization features:

- De/Serialize abstraction types (abstract classes and interfaces) by type registration
- JIT type creation for unregistered abstraction types
- De/Serializes anonymous types
- Fully customizable serialization of both 1st- and 3rd-party types
- De/Serialize static types/properties
- De/Serialize fields
- De/Serialize enumerations by name or numeric value
- De/Serialize circular references
- Optionally include type names
- Each serializer instance can be independently configured
- Supports multiple date/time formats (ISO 8601, JavaScript, custom)
- Supports using DI containers for object creation
- Supports non-default constructors
- JSON template generation from .Net types (beta)
- Property name customization via attribute
- Opt-out property inclusion via attribute

See the wiki pages for more information on how to use this wonderful library!
