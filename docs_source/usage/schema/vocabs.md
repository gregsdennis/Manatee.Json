## Extending schemas

`JsonSchema` has been designed to allow you to create your own keywords.  There are several steps that need to be performed to create and use your own keywords.

- Implement `IJsonSchemaKeyword`.
- Add the keyword implementation to the catalog.
- ... um... yeah, I guess that's it.

### Write a keyword

Here's what you need to know about each member defined by `IJsonSchemaKeyword`.  You'll need to implement all of these as there is no base class.

#### `Name`

This property returns the keyword's string form (e.g. `$id` or `maximum`)

#### `SupportedVersions`

This property returns the official draft versions that are supported by this keyword.  For example, if you want to support all of the official schema drafts, you can return `JsonSchemaVersion.All`, but if you only want draft-06 and draft-07 supported, you can return `JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07`.

#### `ValidationSequence`

This property becomes important when you have several keywords that must be evaluated in a specific order.  A prime example of this are the `properties`, `patternProperties`, and `additionalProperties` (and soon `unevaluatedProperties` from draft-08).  These keywords must be evaluated in that order.  Generally, if you are creating an independent keyword (as most of them should be), this can simply return 1.

#### `RegisterSubschemas(Uri)`

The validation logic is performed iteratively.  This means that it can't  look forward when resolving `$ref` keywords (e.g. if the `definitions` keyword appears at the end of the schema).  To account for this, each schema will do an initialization pass when validating for the first time.  This ensures that any subschemas have been registered prior to any attempt at `$ref` resolution.

If your keyword does not contain a schema as part of its value, then this method can be a no-op.  Otherwise, simply call `JsonSchema`'s corresponding `RegisterSubschemas(Uri)` method and just pass the parameter.  `JsonSchema` will take care of the rest.

#### `ResolveSubschema(JsonPointer, Uri)`

This method is called during `$ref` resolution.  If your keyword doesn't contain a schema, just return null.  Otherwise, simply pass the call onto `JsonSchema`'s corresponding `ResolveSubschema(JsonPointer, Uri)` method.

#### `Validate(JsonSchemaContext)`

This is the guts of the keyword, where all of the actual validation takes place.

##### The validation context

The context fulfills two purposes: provide all of the required validation information to the keyword, and provide all of the location information needed to report any annotations and errors.  Below is a breakdown of all of the properties on the context and their roles in validation.

- `Local` - This property indicates the local schema that is being validated.  It's automatically set by the `JsonSchema` class, so you don't ever need to set this or modify it.
- `Root` - This is the root schema.  This is set at the beginning of the validation process and should never be updated.
- `Instance` - This is the instance being validated *at the current level*.  This may be a nested value inside the original instance.
- `InstanceLocation` - This is the location of the `Instance` property within the original instance.
- `EvaluatedPropertyNames` - This is a list of property names that have been validated by either your keyword or by subschemas.  You will need to add any properties your keyword processes as well as any properties in the contexts you send to subschemas to this list.
- `RelativeLocation` - This is the location of the current subschema with respect to the root schema.  It will contain `$ref` segments for any references that have been processed.
- `BaseUri` - This is the current base URI.  It will change throughout the validation process as subschemas declare `$id` or `id` keywords.
- `BaseRelativeLocation` - This is the location of the current subschema relative to the `BaseUri`.

If your keyword contains a nested schema, it's important that you create a new context based on the properties in the context that was given to your keyword.  Changing the existing context will affect the validation of sibling or cousin keywords.  Some of the context properties will have to be modified based on how your keyword behaves.

First, you'll always want to append your keyword name to the `RelativeLocation` and `BaseRelativeLocation` pointers.  There may be additional segments that you'll want to append. (The `items` keyword does this when it contains an array of subschemas.)

Secondly, if the subschemas apply to the same instance that your keyword was given (like the `*Of` or `if`/`then`/`else` keywords), you'll want to leave the `InstanceLocation` alone, copying it as-is to the new context.  However, if your keyword processes an instance property or array item (like the `items` keyword), you'll need to append that property name or array index appropriately.

##### Building a result

The result object is defined by a current proposal for draft-08 which seeks to standardize the output produced by a schema.  Manatee.Json builds the verbose hierarchy format then condenses it according to the `JsonSchemaOptions.OutputFormat` static property.  The location properties are taken care of simply by passing the context into the constructor.  You'll need to set the validation-oriented ones yourself.

- `IsValid` - This property defaults to `true`, so you'll need to set it to false when validation fails.
- `AnnotationValue` - If your keyword generates annotations, set this property when the validation passes.
- `AdditionalInfo` - This is just a `JsonObject` that you can use to pass any other pertinent information.
- `NestedResults` - If your keyword has one or more subschemas, this property is for the validation results that they produce.

### Add your keyword to the catalog

The `SchemaKeywordCatalog` static class is the curator of all of the keywords.  It also provides the keyword instances during deserialization.

To make your keyword available for use, call the `Add<T>()` method using your keyword type as the type parameter.  The method *does* contain type constraints that required `T` to implement `IJsonSchemaKeyword` and to have a parameterless constructor.

***NOTE** All of the built-in keywords also follow this convention, exposing parameterless constructors.  These constructors have been marked with a `DeserializationUseOnlyAttribute` as a reminder not to use them.*

### Now make it nice to use

To enable the fluent construction interface for your keyword, simply create an extension method on `JsonSchema` that adds the keyword and returns the schema.  For example, adding a `description` keyword is implemented by this method:

```csharp
public static JsonSchema Description(this JsonSchema schema, string description)
{
    schema.Add(new DescriptionKeyword(description));
    return schema;
}
```

