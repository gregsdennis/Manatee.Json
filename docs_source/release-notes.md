# 10.1.2

*Released on 21 Nov, 2018*

<span id="patch">patch</span>

BUG FIX: There was a concurrency issue when determining the correct internal serializer to use.

# 10.1.1

*Released on 21 Nov, 2018*

<span id="patch">patch</span>

BUG FIX: Generic collections typed with abstract and interface types were not saving the instance type for the items.

# 10.1.0

*Released on 16 Nov, 2018*

<span id="feature">feature</span> <span id="patch">patch</span>

Expanded on the fixes from 10.0.5 and added the `JsonSchemaOptions.AllowUnknownFormats` static property to control behavior when unknown string formats are encountered.

# 10.0.5

*Released on 15 Nov, 2018*

<span id="patch">patch</span>

([#193](https://github.com/gregsdennis/Manatee.Json/issues/193)) Invalid string formats were not being included in the schema validation result data when the format was not recognized.  It would just fail.  Now the result object will contain a new `isKnownFormat` key in the `AdditionalInfo` property to indicate whether the specified keyword was recognized.

([#194](https://github.com/gregsdennis/Manatee.Json/issues/194)) Schema validations that occurred through `$ref` keywords were not being reported at all.  Instead the report would state that the error occurred at the `$ref`, but would give no further information.

# 10.0.4

*Released on 8 Oct, 2018*

<span id="patch">patch</span>

User-defined serialization should have priority over built-in ones.

Enum deserialization should be smarter about detecting numeric or string formatting.

# 10.0.3

*Released on 8 Oct, 2018*

<span id="patch">patch</span>

Fixed issue with serialization of non-enumerated `IEnumerable<T>` types like `.Where()` and `.Select()` queries.

Added missing XML comments on some types.

# 10.0.2

*Released on 4 Oct, 2018*

<span id="patch">patch</span>

Removed compiler warnings for using `JsonSchema`.

# 10.0.1

*Released on 3 Oct, 2018*

<span id="patch">patch</span>

Fixed issue where a class with a nullable struct fails to serialize.

Fixed issue where a non-null object with all properties as default values would be serialized to `null` then summarily removed from the serialization output.  This is bad because null is different than a non-null default object.

# 10.0.0

*Released on 30 Sep, 2018*

<span id="break">breaking change</span> <span id="feature">feature</span>

## Summary

([#147](https://github.com/gregsdennis/Manatee.Json/issues/147)) Updated how custom serialization is handled.  Rather than registering independent methods (`CustomSerializations`), custom sub-serializer implementations that each handle specific scenarios can be registered.  Each implementation specifies whether it can handle the (de)serialization based on the expected type, the serializer options, and (for deserialization, specifically) the JSON value.  It will also specify whether it should maintain referential integrity.

([#132](https://github.com/gregsdennis/Manatee.Json/issues/132)) Changed parsing and schema errors to use JSON Pointers to indicate the locations of errors instead of JSON Paths.  The reasoning behind this is JSON Path is a query syntax that can return multiple values, whereas JSON Pointer was specifically designed to represent a single path resulting in a single value.

([#175](https://github.com/gregsdennis/Manatee.Json/issues/175) Part 1) In preparation for JSON Schema draft-08, the JSON Schema implementation has been overhauled.  The key takeaways from this work are:

- The schema drafts are no longer represented by multiple classes, but rather a single `JsonSchema` class that is capable of modeling all of the drafts.
- The `JsonSchema` type does not expose keywords as properties, but is a container of keyword implementations.  This allows for easier extension.

([#112](https://github.com/gregsdennis/Manatee.Json/issues/112)) Another change intended to support JSON Schem draft-08 is a [standardized output format](https://github.com/json-schema-org/json-schema-spec/issues/643).  The validation results have been restructured to conform to this proposal.

([#177](https://github.com/gregsdennis/Manatee.Json/issues/177)) Opened up the `StringFormat` class (schema `format` keyword) for extension and customization.

([#152](https://github.com/gregsdennis/Manatee.Json/issues/152)) Updated referential integrity for serialization to use JSON Pointers that point elsewhere in the structure instead of the previous `#Def` and `#Ref` tags.  Now you will see something more like `{"$ref":"#/property1/array/3/data"}` in the serialized JSON for references.  The idea of the `#Def` tag has been removed.

*In addition to the below listings, the entire JSON Schema implementation has been overhauled.  Please see the JSON Schema documentation for details.*

### Changes

- `JsonSyntaxException` - `string Path` property replaced by `JsonPointer Location` property.
- `JsonOptions.PrettyPrintIndent` - Updated from `char` to `string` to support indentations like multiple spaces.

### New types

- `SerializerFactory`
- `ISerializer`

### Removed types

- `CustomSerializations`
- `JsonSerializationAbstractionMap` (previously obsolete)
- `JsonSerializationTypeRegistry` (previously obsolete)
- `TypeRegistrationException`

### Functional changes

- Updated the default enumeration serialization method from using numeric values to using named values.  (See `JsonSerializerOptions.EnumSerializationFormat`.)

# 9.9.4 & 9.9.5

*Released on 23 August, 2018*

<span id="patch">patch</span>

([#179](https://github.com/gregsdennis/Manatee.Json/issues/179)) Fixed concurrency issue that may occur while auto-serializing a single object type for the first time on two threads simultaneously.

# 9.9.3

*Released on 12 Jul, 2018*

<span id="patch">patch</span>

Fixed bug where attempting to download schemas behind `https` links would throw the least helpful of exceptions: a message-less `Exception`.

# 9.9.2

*Released on 8 Jun, 2018*

<span id="patch">patch</span> <span id="spec">spec</span>

([#170](https://github.com/gregsdennis/Manatee.Json/issues/170)) [JSON-Schema.org](http://json-schema.org/) updated the meta-schemas for all drafts to be more inline with the specifications.  This update matches those changes.

# 9.9.1

*Released on 13 Jun, 2018*

<span id="patch">patch</span>

([#167](https://github.com/gregsdennis/Manatee.Json/issues/167)) JSON Schema: `required` is only processed when `properties` is present.

# 9.9.0

*Released on 9 May, 2018.*

<span id="feature">feature</span>

([#161](https://github.com/gregsdennis/Manatee.Json/issues/161)) Added the ability to customize schema error messages.
