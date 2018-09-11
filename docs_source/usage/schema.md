# JSON Schema

> *This documentation only applies to Manatee.Json version 10 and later.  To work with JSON Schema in previous Manatee.Json versions, please refer to the [legacy JSON Schema page](schema-v9.html).*

The occasion may arise when you wish to validate that a JSON object is in the correct form (has the appropriate keys and the right types of values).  Enter JSON Schema.  Much like XML Schema with XML, JSON Schema defines a pattern for JSON data.  A JSON Schema validator can verify that a given JSON object meets the requirements as defined by the JSON Schema.  This validation can come in handy as a precursor step before deserializing.

More information about JSON Schema can be found at [json-schema.org](http://json-schema.org).

To support JSON Schema, Manatee.Json exposes the `JsonSchema` type.  This type is implemented as a list of keywords, each of which correspond to one of the keywords defined in the JSON Schema specifications.

## Drafts

There are currently four active drafts of the JSON Schema specification:

- Draft-03
- Draft-04
- Draft-06
- Draft-07

*Draft-08 is currently in progress and due out soon!*

Manatee.Json supports draft-04 and above.

### Meta-schemas

Each draft defines a meta-schema.  This is a special JSON Schema that describes all of the keywords available for that draft.  They are intended to be used to validate other schemas.  Usually, a schema will declare the draft it should adhere to using the `$schema` keyword.

Manatee.Json declares the meta-schemas for draft-04, draft-06, and draft-07 as members of the `MetaSchemas` static class.

## Keywords

JSON Schema is expressed as a collection of keywords, each of which provides a specific constraint on a JSON instance.  For example, the `type` keyword specifies what type of data an instance may be, whereas the `minimum` keyword specifies a minimum numeric value *for numeric data*.  These keywords can be combined to express the expected shape of any JSON instance.  Once defined, the schema validates the instance, providing feedback on errors that occurred, including what and where the error occurred.

## Building a schema

There are two options when building a schema: defining it inline using the object model and defining it externally and deserializing.  Which method you use depends on your specific requirements.

### Deserialization

Manatee.Json schemas are fully serializable using the default serializer settings.  Just create a new `JsonSerializer` and deserialize as you would any other object.

```csharp
var serializer = new JsonSerializer();
var text = File.ReadAllText("mySchema.json");
var json = JsonValue.Parse(text);
var mySchema = serializer.Deserialize<JsonSchema>(json);
```

Done.

### Inline

To build a schema inline, you can either declare all of the keywords individually and add them to a `JsonSchema` instance:

```json 
{
    "properties":{
        "myProperty":{
            "type":"string",
            "minLength":10
        }
    },
    "required":["myProperty"]
}
```

```csharp
var schema = new JsonSchema {
    new PropertiesKeyword {
        ["myProperty"] = new JsonSchema {
            new TypeKeyword(JsonSchemaType.String),
            new MinLengthKeyword(10)
        }
    },
    new RequiredKeyword{ "myProperty" }
};
```

or you can use the fluent interface:

```csharp
var schema = new JsonSchema()
    .Property("myProperty", new JsonSchema()
        .Type(JsonSchemaType.String),
        .MinLength(10))
    .Required("myProperty");
```

***NOTE** The meta-schemas mentioned above are declared using the inline syntax.  I've found it to be easier than by instantiating keywords explicitly.*

## Validation & annotations

### Validating the schema itself

Because the `JsonSchema` class will accept any keyword, and some keywords are only supported by specific drafts, it may be important to ensure that the schema that's created is valid against one of the drafts.

To ensure that this is the case, call the `ValidateSchema()` method.  This will analyze all of the keywords and report on whether they are all compatible.

### Validating instances

`JsonSchema` exposes a `Validate(JsonValue)` method which is used to validate incoming JSON values.  Let's begin with the schema from above and a few JSON objects:

```json 
{
    "properties":{
        "myProperty":{
            "type":"string",
            "minLength":10
        }
    },
    "required":["myProperty"]
}

{}
{"myProperty":false}
{"myProperty":"some string"}
{"otherProperty":35.4}
"nonObject"
```

To validate these, all we have to do is pass these into our schema's `Validate(JsonValue)` method.

```csharp
var schema = new JsonSchema(); // defines the schema we have listed above.
var emptyJson = new JsonObject();
var booleanJson = new JsonObject { {"myProperty", false} };
var stringJson = new JsonObject { {"myProperty", "some string"} };
var shortJson = new JsonObject { {"myProperty", "short"} };
var numberJson = new JsonObject { {"otherProperty", 35.4} };
var nonObject = (JsonValue)"nonObject";

var emptyResults = schema.Validate(emptyJson);
var booleanResults = schema.Validate(booleanJson);
var stringResults = schema.Validate(stringJson);
var numberResults = schema.Validate(numberJson);
var nonObjectResults = schame.Validate(nonObject);
```

> **IMPORTANT** You may have noticed that the second parameter is not used.  This parameter is used internally for validating subschema and resolving references and should not be used explicitly.

The various results objects are of type `SchemaValidationResults`, which has two properties:

- IsValid - Indicates if the tested JSON value is valid,
- Errors - A collection of errors found while validating.  Each error lists a JSONPath to the problem element and a message which describes the issue.

In the above example, the following would be reported:

- `emptyJson` and `numberJson` failed because `"myProperty"` was not found.
- `booleanJson` failed because the value of `"myProperty"` is of the wrong type.
- `stringJson` passed validation.
- `shortJson` failed because the value of `"myProperty"` was too short.
- `nonObject` also passed validation because `properties` and `required` ignore non-object JSON.

## Registering well-known schemas



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
