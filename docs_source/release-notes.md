# 12.0.0

*Released on 27 Nov, 2019*

<span id="break">breaking change</span><span id="feature">feature</span><span id="patch">patch</span>

This change primarily adds .Net Core 3.0 support.  It has also been updated to support null reference types.

## Logging

Some cursory verbose logging has been introduced to allow the client to see what decisions Manatee.Json is making during (primarily) schema validation and serialization.  The `JsonOptions.Log` static property has been introduced for this purpose.  It is of type `ILog` which exposes a single method, `Verbose(string, LogCategory)`.

The log categories currently are general, serialization, and schema.  This allows the client to enable or disable certain activities, so that, for instance, they can only enable schema validation logging if they wish.  The `JsonOptions.LogCategory` static property controls what categories are actually sent.

**There is no default implementation for `ILog`.**  Keeping this option null will ensure faster processing in production scenarios.  This logging feature is intended for research and debugging purposes only as there are *a lot* of logs which can add processing time.

## Bug fixes

`unevaluatedItems` would not properly process items that nested keywords touched but failed.  See [JSON-Schema-Test-Suite #291](https://github.com/json-schema-org/JSON-Schema-Test-Suite/issues/291) for details and an example of the logging mentioned above.

## Breaking Changes

- `JsonValue` parameterless constructor removed.  Use `JsonValue.Null` instead.
- `JsonValue` parameterized constructor arguments changed to non-nullable or will throw `ArgumentNullException`.
- `JsonArray.OfType(JsonValueType)` now throws `ArgumentNullException` when passed a null array.
- `JsonSyntaxException.Source` replaced by `SourceData` so that it doesn't hide `Exception.Source`.
- `SerializationContext` used only for serialization now.  `DeserializationContext` now introduced for deserialization.  This changes method signatures for `ISerializer`.  Additional notes below.

### Serialization changes

In an effort to reduce memory footprint, a single `SerializationContext`/`DeserializationContext` instance will be retained throughout the serialization/deserialization process.  To facilitate this, new instances will no longer be created when a serializer needs to serialize an object's contents; instead it will "push" new details onto the existing context, serialize, then "pop" the details when finished.

For example, the internal `ListSerializer` in versions up to v11 would serialize a list's contents using [this code](https://github.com/gregsdennis/Manatee.Json/blob/7b5fa44bd2ece54dc616716482284baaca72ed35/Manatee.Json/Serialization/Internal/Serializers/ListSerializer.cs#L23-L34):

```c#
for (int i = 0; i < array.Length; i++)
{
    var newContext = new SerializationContext(context)
    {
        CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
        InferredType = list[i]?.GetType() ?? typeof(T),
        RequestedType = typeof(T),
        Source = list[i]
    };

    array[i] = context.RootSerializer.Serialize(newContext);
}
```

However, creating a new context instance for each item is resource intensive and creates a lot of work for the garbage collector afterward.

Instead we simply push new details onto the existing context before we serialize an item, then pop those details afterward, like [this](https://github.com/gregsdennis/Manatee.Json/blob/929e6d1b922df5f774522e0743cc05d19e2527b9/Manatee.Json/Serialization/Internal/Serializers/ListSerializer.cs#L23-L28):

```c#
for (int i = 0; i < array.Length; i++)
{
    context.Push(list[i]?.GetType() ?? typeof(T), typeof(T), i.ToString(), list[i]);
    array[i] = context.RootSerializer.Serialize(context);
    context.Pop();
}
```

The values that we push are

- The inferred type of the object we're pushing (or the requested type if the inferred type is null)
- The requested type
- The JSON path segment (in this case the index)
- The object to be serialized

These are maintained in a stack internally to the context object and are removed on the `Pop()` call, keeping the context synchronized with the serialization process.

# 11.0.4

*Released on 27 Nov, 2019*

<span id="patch">patch</span>

([#231](https://github.com/gregsdennis/Manatee.Json/pulls/231)) `$ref` throwing `SchemaNotFoundException` when `$id` is an anchor-type reference in drafts 07 and earlier.  (These are disallowed for draft 2019-09.  The functionality was split out to the `$anchor` keyword.)

Fixed an issue where not all evaluated properties were being reported to be used by `unevaluatedProperties`.  This came out of a JSON Schema Slack conversation.

# 11.0.3

*Released on 26 Oct, 2019*

<span id="patch">patch</span>

([#226](https://github.com/gregsdennis/Manatee.Json/pulls/226)) Fixes for JSON Patch move and add operations.

# 11.0.2

*Released on 14 Oct, 2019*

<span id="patch">patch</span>

Fixed an issue with deserialization name transformations where the function was passed the member name instead of the JSON key value.

# 11.0.1

*Released on 29 Sep, 2019*

<span id="patch">patch</span>

([#215](https://github.com/gregsdennis/Manatee.Json/pulls/215)) `IndexOutOfRangeException` thrown on JSON Path arrays when index equals array count.

The schema `format` keyword was incorrectly failing for some keywords when value type is not a string.

Minor serialization improvements.

# 11.0.0

*Released on 18 Sep, 2019*

<details><summary>Beta releases</summary>
<p>

- *v11.0.0 beta 7 - Released on 29 Aug, 2019*
- *v11.0.0 beta 6 - Released on 24 Jul, 2019*
- *v11.0.0 beta 5 - Released on 21 Jul, 2019*
- *v11.0.0 beta 4 - Released on 19 Jul, 2019*
- *v11.0.0 beta 3 - Released on 18 Jul, 2019*
- *v11.0.0 beta 2 - Released on 10 Jul, 2019*
- *v11.0.0 beta 1 - Released on 1 Jul, 2019 (introduces breaking changes)*
- *v10.2.0 beta 2 - Released on 28 Jun, 2019*
- *v10.2.0 beta 1 - Released on 22 Jun, 2019*

***NOTE** The properties for the new drafts contain version names like "Draft2019_06".  Since this depends on when the spec is released, these may change between the beta and the official release without incrementing the major version.*

</p>
</details>

<span id="break">breaking change</span><span id="feature">feature</span>

## Breaking changes

### Schema

In order to support some new independent reference tests, some changes were made to the schema validation logic to include a validation-run-independent schema registry, separate from the static one.  The first two changes support this requirement:

- `IJsonSchemaKeyword.RegisterSubschemas(Uri? baseUri, JsonSchemaRegistry localRegistry)` - The second parameter is new.
- `SchemaValidationContext` now requires a source context from which to copy values.
- `JsonSchemaOptions.OutputFormat` now has a default value of `Flag`, which only returns whether an instance is valid, without any error details.  This greatly improves performance out of the box.  Configuration is required to generate error details.
- `JsonSchemaoptions.RefResolutionStrategy` now has a default value of `ProcessSiblingId` to conform with the draft-08 definition of `$ref` and `$recursiveRef`.

Additionally, it was pointed out on [the JSON Schema spec repo](https://github.com/json-schema-org/json-schema-spec/issues/759) that `format` is not specifically intended for strings, but can also be used to validate other types.  To address this, the `StringFormat` type has been renamed to `Format` and now accepts `JsonValue` instead of merely `string`, although one string formats are defined by the spec.  Formatting for other types as well as their validation logic will need to be provided by the consumer (you).

### Serialization

- `IResolver.Resolve<T>()` removed since it wasn't used internally anywhere.
- `IResolver.Resolve(Type, Dictionary<SerializationInfo, object>)` - The second parameter is new.

### Other breaking changes

Dropped multi-target support.  Now only targeting .Net Standard 2.0.  This is still compatible with .Net Framework 4.6.1 and higher.

### Obsolete items removed

- `SchemaErrorMessages` - Error messages are now customizable via the `ErrorTemplate` static property on those keywords which have it.
- `FluentBuilderExtensions.Items(this JsonSchema, params JsonSchema[])` - The compiler could not distinguish between the single-schema usage of `items` and the array-usage which only contained a single schema.  Instead, multiple calls to `Item` should be used.  This is also more consistent with how `Definition` and `Property` work.

## Non-breaking changes

([#175](https://github.com/gregsdennis/Manatee.Json/issues/175)) JSON Schema draft-08 support.  

([#209](https://github.com/gregsdennis/Manatee.Json/issues/209)) `PropertiesKeyword` does not return nested results.  This was fixed as part of #175.

([#205](https://github.com/gregsdennis/Manatee.Json/issues/205)) Schema equality doesn't capture extra properties on second schema.  E.g. for `a.Equals(b)` if `b` has a property that doesn't exist in `a`, `.Equals()` still returns true.  The collection comparison has been fixed.

([#201](https://github.com/gregsdennis/Manatee.Json/issues/201)) CompilerAttributes causes errors in .Net Standard versions.  This dependency has been removed.

([#211](https://github.com/gregsdennis/Manatee.Json/issues/211)) [@desmondgc](https://github.com/desmondgc) Date/Time format validation within schemas is insufficient.  Default implementation now uses `DateTime.TryParseExact()` with support for fractional seconds.

([#219](https://github.com/gregsdennis/Manatee.Json/issues/219)) `$ref` resolution fails when using a relative URI as the `$id` for the root schema.  Added a new option `JsonSchemaOptions.DefaultBaseUri` as suggested by the spec.  The default value for this option is `manatee://json-schema/`.

- `JsonSchemaRegistry` is no longer static, but all previously existing functionality is still static.  All instance functionality is internal.
- Minor memory usage improvements for JSON Schema validation.
- Added the following to `JsonSchemaOptions` to more finely control the output, which can have a significant effect on performance.
  - `IgnoreErrorsForChildren<T>()` - Prevents collection of errors and annotations generated by subschemas for a given keyword throughout the entire schema structure.
  - `IgnoreErrorsForChildren(JsonPointer)` - Prevents collection of errors and annotations generated by subschemas of a specified location within the schema.
  - `ShouldReportChildErrors(IJsonSchemaKeyword, SchemaValidationContext)` - To be used by custom keywords, determines whether errors and annotations should be reported based on the above settings.

# 10.1.4

*Released on 8 Jun, 2019*

<span id="patch">patch</span>

([#206](https://github.com/gregsdennis/Manatee.Json/pull/206)) [@Kimtho](https://github.com/Kimtho) fixed JSON Patch `replace` verb.  Thanks!

# 10.1.3

*Released on 14 May, 2019*

<span id="patch">patch</span>

([#203](https://github.com/gregsdennis/Manatee.Json/issues/203)) Adjacent (consecutive) escape sequences that contained Unicode values were always considered as a surrogate pair even if they were just two separate characters.

Updating schema test suite references exposed a bug in `additionalProperties` keyword implementation in that it would look into applicator keywords (`anyOf`, `allOf`, etc.).  This bug was fixed.

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

([#175](https://github.com/gregsdennis/Manatee.Json/issues/175) (Part 1) In preparation for JSON Schema draft-08, the JSON Schema implementation has been overhauled.  The key takeaways from this work are:

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
