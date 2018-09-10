# 10.0.0

*Released on XXXXXXXXXXXXXXXXXXXXXXXXXX*

<span id="feature">feature</span>

## Summary

([#147](https://github.com/gregsdennis/Manatee.Json/issues/147)) Updated how custom serialization is handled.  Rather than registering independent methods (`CustomSerializations`), custom sub-serializer implementations that each handle specific scenarios can be registered.  Each implementation specifies whether it can handle the (de)serialization based on the expected type, the serializer options, and (for deserialization, specifically) the JSON value.  It will also specify whether it should maintain referential integrity.

([#132](https://github.com/gregsdennis/Manatee.Json/issues/132)) Changed parsing and schema errors to use JSON Pointers to indicate the locations of errors instead of JSON Paths.  The reasoning behind this is JSON Path is a query syntax that can return multiple values, whereas JSON Pointer was specifically designed to represent a single path resulting in a single value.

## Changes

### New members

- `JsonPointer SchemaValidationError.Property { get; }`

### Removed members

- `string SchemaValidationError.PropertyName { get; }`

### New types

- `SerializerFactory`
- `ISerializer`

### Removed types

- `CustomSerializations`
- `JsonSerializationAbstractionMap` (previously obsolete)
- `JsonSerializationTypeRegistry` (previously obsolete)
- `TypeRegistrationException`

### Functional changes

# 9.9.4 & 9.9.5

*Released on 23 August, 2018*

<span id="patch">patch</span>

([#179](https://github.com/gregsdennis/Manatee.Json/issues/179)) Fixed concurrency issue that may occur while auto-serializing a single object type for the first time on two threads simultaneously.

# 9.9.3

*Released on 12 Jul, 2018*

<span id="patch">patch</span>

Fixed bug where attempting to download schemas behind `https` links would throw the least helpful of exceptions: a message-less `Exception`.

# 9.9.2

*Released on 13 July, 2018*

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
