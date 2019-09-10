# Manatee.Json

[![NuGet version (Manatee.Json)](https://img.shields.io/nuget/v/Manatee.Json.svg?style=flat-square)](https://www.nuget.org/packages/Manatee.Json/)
[![Build status](https://ci.appveyor.com/api/projects/status/wda5exdfiuic3kg2/branch/master?svg=true)](https://ci.appveyor.com/project/gregsdennis/manatee-json/branch/master)
[![MyGet Build Status](https://www.myget.org/BuildSource/Badge/littlecrabsolutions?identifier=7898edc2-8d91-411c-88c9-2023d9d9fd41)](https://www.myget.org/)
[![Percentage of issues still open](http://isitmaintained.com/badge/open/gregsdennis/Manatee.Json.svg)](http://isitmaintained.com/project/gregsdennis/Manatee.Json "Percentage of issues still open")
[![Average time to resolve an issue](http://isitmaintained.com/badge/resolution/gregsdennis/Manatee.Json.svg)](http://isitmaintained.com/project/gregsdennis/Manatee.Json "Average time to resolve an issue")

<!-- [![Discuss on Slack](/Resources/Slack_RGB.png)](https://join.slack.com/t/manateeopensource/shared_invite/enQtMzU4MjgzMjgyNzU3LWQ0ODM5ZTVhMTVhODY1Mjk5MTIxMjgxZjI2NWRiZWZkYmExMDM0MDRjNGE4OWRkMjYxMTc1M2ViMTZiYzM0OTI) -->

<a href="https://join.slack.com/t/manateeopensource/shared_invite/enQtMzU4MjgzMjgyNzU3LWQ0ODM5ZTVhMTVhODY1Mjk5MTIxMjgxZjI2NWRiZWZkYmExMDM0MDRjNGE4OWRkMjYxMTc1M2ViMTZiYzM0OTI"><img src="/Resources/Slack_RGB.svg" alt="Discuss on Slack" title="Slack" height="100"></a>

<!-- [![Made with Jetbrains Resharper](/Resources/Resharper.svg)](http://www.jetbrains.com/resharper) -->

<a href="http://www.jetbrains.com/resharper"><img src="/Resources/Resharper.svg" alt="Made with Jetbrains Resharper" title="Resharper" height="100"></a>

**Documentation for this library can be found [here](https://gregsdennis.github.io/Manatee.Json).**

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

- Is covered by over 3000 unit tests
- Conforms to RFC-8259: The JSON specification
- Support .Net Standard 2.0
- Outputs compact and prettified JSON text
- Supports [JSON Schema](http://json-schema.org/) **INCLUDED AND FREE!**
    - Draft 4
    - Draft 6
    - Draft 7
    - Draft 8 (In beta! Spec to be released soon!)
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
- De/Serialize immutable types
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

See the [docs](https://gregsdennis.github.io/Manatee.Json) for more information on how to use this wonderful library!

## Contributing

If you have questions, experience problems, or feature ideas, please create an issue.

If you'd like to help out with the code, please feel free to fork and create a pull request.

### Special thanks

@sixlettervariables (Christopher Watford) for digging around the muck that was the serialization code and drastically improving performance.

@Kimtho for finding and fixing some backwards logic in the JSON Patch `replace` verb.

@desmondgc (Desmond Cox) for improving the validation within schemas for `date-time` formatted strings.

### The Project

This code uses C# 7 features, so a compiler/IDE that supports these features is required.

The library consists of a single project that target .Net Standard 1.3.  The test project targets .Net Framework 4.6.

### Building

During development, building within Visual Studio should be fine.

### Code style and maintenance

I use [Jetbrains Resharper](https://www.jetbrains.com/resharper/) in Visual Studio to maintain the code style (and for many of the other things that it does).  The solution is set up with team style settings, so if you're using Resharper the settings should automatically load.  Please follow the suggestions.

### Appreciation

If you've enjoyed using this library and you'd like to contribute financially, please use the button below.

[![Donate](https://i.imgur.com/Fkk2ET1.png)](https://ko-fi.com/gregsdennis)
