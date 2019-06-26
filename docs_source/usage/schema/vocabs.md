# Extending schemas

## Vocabularies

JSON Schema draft-08 has introduced the idea of vocabularies to enable some spec support for custom keywords.

A vocabulary is a collection of keywords.  It will be identified by a URI and should have an associated specification that describes the function of each of the keywords.  There _may_ also be an associated meta-schema.

Because Manatee.Json supported extending schemas with custom keywords before vocabularies were introduced, creating a vocabulary isn't strictly required.  But if you're using it to consume and validate draft-08 schemas, it is strongly suggested.

### How they work

This is best explained with an example.  Suppose I have a meta-schema **M**, a schema **S** that uses that meta-schema, and an instance **I** to be validated by the schema.

```json
// meta-schema M
{
  "$schema": "https://myserver.net/meta-schema#",                           // 1
  "$id": "https://myserver.net/meta-schema#",
  "$vocabulary": {
    "https://json-schema.org/draft/2019-04/vocab/core": true,               // 2
    "https://json-schema.org/draft/2019-04/vocab/applicator": true
  },
  "allOf": [                                                                // 3
    { "$ref": "https://json-schema.org/draft/2019-06/schema#" }
  ],
  "properties": {
    "newKeyword": {                                                         // 4
      "type": "integer",
      "minimum": 1
    }
  }
}

// schema S
{
  "$schema": "https://myserver.net/meta-schema#",                           // 5
  "$id": "https://myserver.net/schema#",
  "properties": {
    "foo": { "type": "string" }
  },
  "newKeyword": 5                                                           // 6
}

// instance I
{
  "foo": "value"
}
```

First I'll summarize each of the parts, then we can go over them in more depth.

1. I declare a meta-schema.  The meta-schema should validate itself, so I declare the `$schema` to be the same as the `$id`.
2. I need to list the vocabularies that I'm going to use to describe this new meta-schema.  `allOf` and `properties` are listed in the Applicators vocabulary, and Core is assumed if it's not listed explicitly (it contains `$id`, etc.).
3. I also want the draft-08 schema to be applicable, so I include it in an `allOf`.
4. I define a new keyword: `newKeyword`.
5. I create a schema that uses my new meta-schema (because I want to use the new keyword).
6. I use the new keyword.

Note that the `newKeyword` value can be validated when **M** is validating **S**, but there is no details as to what `newKeyword` does when **S** is validating **I**.  To do that, I need to write some code.  See [Write a keyword](#write-a-keyword) for more details.

Now that makes sense, but why are the vocabularies needed?  It seems like **S** would work just fine without them so long as I provide the logic for `newKeyword`.  The value of vocabularies comes in when *you* want to extend *my* meta-schema.

```json
// meta-schema X
{
  "$schema": "https://yourserver.net/meta-schema#",
  "$id": "https://yourserver.net/meta-schema#",
  "$vocabulary": {
    "https://json-schema.org/draft/2019-04/vocab/core": true,
    "https://json-schema.org/draft/2019-04/vocab/applicator": true,
    "https://myserver.net/vocabs/my-vocab": true                            // 1
  },
  "allOf": [
    { "$ref": "https://myserver.net/meta-schema#" }                         // 2
  ],
  "properties": {
    "anotherNewKeyword": {                                                  // 3
      "type": "boolean"
    }
  },
  "newKeyword": 10                                                          // 4
}

// schema Y
{
  "$schema": "https://yourserver.net/meta-schema#",                         // 5
  "$id": "https://yourserver.net/schema#",
  "properties": {
    "foo": { "type": "string" }
  },
  "newKeyword": 5,                                                          // 6
  "anotherNewKeyword": false                                                // 7
}
```

1. 



`JsonSchema` has been designed to allow you to create your own keywords.  There are several steps that need to be performed to do this.

1. Implement `IJsonSchemaKeyword`.
1. Add the keyword implementation to the catalog.
1. ... um... yeah, I guess that's it.

## Write a keyword

Here's what you need to know about each member defined by `IJsonSchemaKeyword`.  You'll need to implement all of these as there is no base class.

### `Name`

This property returns the keyword's string form (e.g. `$id` or `maximum`)

### `SupportedVersions`

This property returns the official draft versions that are supported by this keyword.  For example, if you want to support all of the official schema drafts, you can return `JsonSchemaVersion.All`, but if you only want draft-06 and draft-07 supported, you can return `JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07`.

### `ValidationSequence`

This property becomes important when you have several keywords that must be evaluated in a specific order.  A prime example of this are the `properties`, `patternProperties`, and `additionalProperties` (and soon `unevaluatedProperties` from draft-08).  These keywords must be evaluated in that order.  Generally, if you are creating an independent keyword (as most of them should be), this can simply return 1.

### `Vocabulary`

If the keyword is to be supported as part of JSON Schema draft-08, then this should return the `SchemaVocabulary` object that defines the keyword.  Otherwise it should just return `SchemaVocabularies.None`.

See [Vocabularies](vocabs.html) for more information on vocabularies and the `SchemaVocabulary` object.

### `RegisterSubschemas(Uri)`

The validation logic is performed iteratively.  This means that it can't  look forward when resolving `$ref` keywords (e.g. if the `definitions` keyword appears at the end of the schema).  To account for this, each schema will do an initialization pass when validating for the first time.  This ensures that any subschemas have been registered prior to any attempt at `$ref` resolution.

If your keyword does not contain a schema as part of its value, then this method can be a no-op.  Otherwise, simply call `JsonSchema`'s corresponding `RegisterSubschemas(Uri)` method and just pass the parameter.  `JsonSchema` will take care of the rest.

### `ResolveSubschema(JsonPointer, Uri)`

This method is called during `$ref` resolution.  If your keyword doesn't contain a schema, just return null.  Otherwise, simply pass the call onto `JsonSchema`'s corresponding `ResolveSubschema(JsonPointer, Uri)` method.

### `Validate(JsonSchemaContext)`

This is the guts of the keyword, where all of the actual validation takes place.

#### The validation context

The context fulfills two purposes: provide all of the required validation information to the keyword, and provide all of the location information needed to report any annotations and errors.  Below is a breakdown of all of the properties on the context and their roles in validation.

- `Local` - This property indicates the local schema that is being validated.  It's automatically set by the `JsonSchema` class, so you don't ever need to set this or modify it.
- `Root` - This is the root schema.  This is set at the beginning of the validation process and should never be updated.
- `RecursiveAnchor` - This defines the schema that is currently pointed to by the `$recursiveAnchor` and `$recursiveRoot` keywords.
- `Instance` - This is the instance being validated *at the current level*.  This may be a nested value inside the original instance.
- `EvaluatedPropertyNames` - This is a list of property names that have been validated by either your keyword or by subschemas.  You will need to add any properties your keyword processes as well as any properties in the contexts you send to subschemas to this list.
- `LocallyEvaluatedPropertyNames` - Similar to `EvaluatedPropertyNames` except that it only tracks those properties which have been evaluated by keywords at the current tier of the schema.
- `LastEvaluatedIndex` - Indicates the last evaluated index in an array.
- `LocakTierLastEvaluatedIndex` - Indicates the last evaluated index in an array by keywords at the current tier of the schema.
- `BaseUri` - This is the current base URI.  It will change throughout the validation process as subschemas declare `$id` or `id` keywords.
- `InstanceLocation` - This is the location of the `Instance` property within the original instance.
- `RelativeLocation` - This is the location of the current subschema with respect to the root schema.  It will contain `$ref` segments for any references that have been processed.
- `BaseRelativeLocation` - This is the location of the current subschema relative to the `BaseUri`.
- `IsMetaSchemaValidation` - Indicates that the current validation pass is a meta-schema validation (validation of a schema by another schema).
- `Misc` - This is just a string-based lookup for any additional data you may need passed between keywords.  The `if`/`then`/`else` keywords do this.

If your keyword contains a nested schema, it's important that you create a new context based on the properties in the context that was given to your keyword.  Changing the existing context will affect the validation of sibling or cousin keywords.  Some of the context properties will have to be modified based on how your keyword behaves.

First, you'll always want to append your keyword name to the `RelativeLocation` and `BaseRelativeLocation` pointers.  There may be additional segments that you'll want to append. (The `items` keyword does this when it contains an array of subschemas.)

Secondly, if the subschemas apply to the same instance that your keyword was given (like the `*Of` or `if`/`then`/`else` keywords), you'll want to leave the `InstanceLocation` alone, copying it as-is to the new context.  However, if your keyword processes an instance property or array item (like the `items` keyword), you'll need to append that property name or array index appropriately.

Lastly, copy any properties from the original context that you haven't supplied yourself.

#### Building a result

The result object is defined by a current proposal for draft-08 which seeks to standardize the output produced by a schema.  Manatee.Json builds the verbose hierarchy format then condenses it according to the `JsonSchemaOptions.OutputFormat` static property.  The location properties are taken care of simply by passing the context into the constructor.  You'll need to set the validation-oriented ones yourself.

- `IsValid` - This property defaults to `true`, so you'll need to set it to false when validation fails.
- `AnnotationValue` - If your keyword generates annotations, set this property when the validation passes.
- `ErrorMessage` - This is the error message generated by the schema.  It is intended to have all values inline rather than being a format string.
- `AdditionalInfo` - This is just a `JsonObject` that you can use to pass any other pertinent information.
- `NestedResults` - If your keyword has one or more subschemas, this property is for the validation results that they produce.

## Add your keyword to the catalog

The `SchemaKeywordCatalog` static class is the curator of all of the keywords.  It also provides the keyword instances during deserialization.

To make your keyword available for use, call the `Add<T>()` method using your keyword type as the type parameter.  The method *does* contain type constraints that required `T` to implement `IJsonSchemaKeyword` and to have a parameterless constructor.

***NOTE** All of the built-in keywords also follow this convention, exposing parameterless constructors.  These constructors have been marked with a `DeserializationUseOnlyAttribute` as a reminder not to use them.*

## Now make it nice to use

To enable the fluent construction interface for your keyword, simply create an extension method on `JsonSchema` that adds the keyword and returns the schema.  For example, adding a `description` keyword is implemented by this method:

```csharp
public static JsonSchema Description(this JsonSchema schema, string description)
{
    schema.Add(new DescriptionKeyword(description));
    return schema;
}
```

