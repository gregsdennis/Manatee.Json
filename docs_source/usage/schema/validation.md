# Validation & annotations

## Validating the schema itself

Because the `JsonSchema` class will accept any keyword, and some keywords are only supported by specific drafts, it may be important to ensure that the schema that's created is valid against one of the drafts.

To ensure that this is the case, call the `ValidateSchema()` method.  This will analyze all of the keywords and report on whether they are all compatible.

The results object is an instance of `MetaSchemaValidationResults` which exposes the following properties:

- `IsValid` simply indicates a pass/fail
- `SupportedVersions` indicates the JSON Schema draft versions that this schema passes.
- `MetaSchemaValidations` is a dictionary, keyed by schema ID strings, that contains validation results for those schemas.  Typically the keys will be the IDs of the draft meta-schemas, but could be the ID of any schema.
- `OtherErrors` is a list of strings to support other errors that may result outside of those produced by meta-schema validations.

## Validating instances

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
var schema = new JsonSchema()
                   .Property("myProperty", new JsonSchema()
                                                 .Type(JsonSchemaType.String)
                                                 .MinLength(10))
                   .Required("myProperty");
var emptyJson = new JsonObject();
var booleanJson = new JsonObject { ["myProperty"] = false };
var stringJson = new JsonObject { ["myProperty"] = "some string" };
var shortJson = new JsonObject { ["myProperty"] = "short" };
var numberJson = new JsonObject { ["otherProperty"] = 35.4 };
var nonObject = (JsonValue)"nonObject";

var emptyResults = schema.Validate(emptyJson);
var booleanResults = schema.Validate(booleanJson);
var stringResults = schema.Validate(stringJson);
var numberResults = schema.Validate(numberJson);
var nonObjectResults = schame.Validate(nonObject);
```

The various results objects are of type `SchemaValidationResults`.  More information about the results object can be found in the next section.

In the above example, the following would be reported:

- `emptyJson` and `numberJson` failed because `"myProperty"` was not found.
- `booleanJson` failed because the value of `"myProperty"` is of the wrong type.
- `stringJson` passed validation.
- `shortJson` failed because the value of `"myProperty"` was too short.
- `nonObject` also passed validation because `properties` and `required` ignore non-object JSON.

## Validation results

Planned for JSON Schema draft-08 is a standardized format for validation output in order to support cross-platform and cross-implementation compatibility.  The format is described in [this GitHub issue](https://github.com/json-schema-org/json-schema-spec/issues/643).  This includes support for both errors and annotation collection.

In summary, there are four levels of verbosity for output: Basic, List, Hierarchy, and Verbose Hierarchy.

Basic will simply return a boolean value indicating a pass/fail result.  All other formats include JSON Pointers and URIs to indicate the source of the errors or annotations that were produced.

A list output reduces all of the errors to a flat list.

The hierarchical views mirror the structure of the schema.  The verbose one copies this structure exactly, whereas the standard hierarchy will condense the results where possible.

The default output format is the condensed hierarchy, but this can be configured via the `JsonSchemaOptions.OutputFormat` static property.

***NOTE** It's only possible to translate from a more detailed to a less detailed format.*

### Examples of output

#### Verbose Hierarchy

```json
{
  "valid" : false,
  "keywordLocation" : "#",
  "instanceLocation" : "#",
  "errors" : [
      {
        "valid" : false,
        "keywordLocation" : "#/allOf",
        "instanceLocation" : "#",
        "keyword" : "allOf",
        "errors" : [
            {
              "valid" : false,
              "keywordLocation" : "#/allOf/0",
              "instanceLocation" : "#",
              "errors" : [
                  {
                    "valid" : false,
                    "keywordLocation" : "#/allOf/0/type",
                    "instanceLocation" : "#",
                    "keyword" : "type",
                    "additionalInfo" : {
                        "expected" : "array",
                        "actual" : "object"
                      }
                  }
                ]
            },
            {
              "valid" : false,
              "keywordLocation" : "#/allOf/1",
              "instanceLocation" : "#",
              "errors" : [
                  {
                    "valid" : false,
                    "keywordLocation" : "#/allOf/1/type",
                    "instanceLocation" : "#",
                    "keyword" : "type",
                    "additionalInfo" : {
                        "expected" : "number",
                        "actual" : "object"
                      }
                  }
                ]
            }
          ]
      }
    ]
}
```

#### Condensed Hierarchy

```json
{
  "valid" : false,
  "keywordLocation" : "#/allOf",
  "instanceLocation" : "#",
  "keyword" : "allOf",
  "errors" : [
      {
        "valid" : false,
        "keywordLocation" : "#/allOf/0/type",
        "instanceLocation" : "#",
        "keyword" : "type",
        "additionalInfo" : {
            "expected" : "array",
            "actual" : "object"
          }
      },
      {
        "valid" : false,
        "keywordLocation" : "#/allOf/1/type",
        "instanceLocation" : "#",
        "keyword" : "type",
        "additionalInfo" : {
            "expected" : "number",
            "actual" : "object"
          }
      }
    ]
}
```

#### Flat List

```json
{
  "valid" : false,
  "errors" : [
      {
        "valid" : false,
        "keywordLocation" : "#/allOf",
        "instanceLocation" : "#",
        "keyword" : "allOf"
      },
      {
        "valid" : false,
        "keywordLocation" : "#/allOf/0/type",
        "instanceLocation" : "#",
        "keyword" : "type",
        "additionalInfo" : {
            "expected" : "array",
            "actual" : "object"
          }
      },
      {
        "valid" : false,
        "keywordLocation" : "#/allOf/1/type",
        "instanceLocation" : "#",
        "keyword" : "type",
        "additionalInfo" : {
            "expected" : "number",
            "actual" : "object"
          }
      }
    ]
}
```

## Value format validation

The `format` keyword has been around a while.  It's available in all of the drafts supported by Manatee.Json.  Although this keyword is techincally classified as an annotation, the specification does allow (the word used is "SHOULD") that implementation provide some level of validation on it so long as that validation may be configured on and off.

Manatee.Json makes a valiant attempt at validating a few of them.  These are hardcoded as static properties on the `Format` class.  Out of the box, these are available:

- `date`
- `date-time`
- `duration`
- `email`
- `hostname`
- `ipv4`
- `ipv6`
- `iri`
- `iri-reference`
- `json-pointer`
- `regex`
- `relative-json-pointer`
- `uri`
- `uri-reference`
- `uri-template`

I'm not going to claim that the validation on any of these is perfect, but it will likely suffice for most applications.  In the (rare) event that it doesn't support your needs, they are completely overridable.

All of the static properties can be set to new instances.  When creating a new instance, it it automatically registered internally (for deserialization purposes) and any lookup by string will result in the newest instance for that key.

In the same way, entirely new formats can be created to make them available to Manatee.Json.

## Static options

The `JsonSchemaOptions` class gives you a few configuration points that likely won't change at runtime.

- `Download` - This function property is the mechanism by which `JsonSchemaRepository` downloads unregistered schemas.  By default, it knows to use `HttpClient` for *http:* endpoints and `System.IO.File` for file paths.  If you need more functionality (for instance if your schema is buried inside an FTP share), override this with a new function that can read from your endpoint.
- `ValidateFormatKeyword` - This defines whether a schema will attempt to apply string format validation based on the value of a `format` keyword.  This is enabled by default.  See above for more information on string format validation.
- `AllowUnknownFormats` - This specifies whether the system will allow unknown string formats.  It is enabled by default.  If `ValidateFormatKeyword` is disabled, this option has no effect.  There are two effect of disabling this option,
  - Validations by schemas with unknown string formats will always return invalid.  This impacts schemas explicitly built in code.
  - If a schema with an unknown string format is deserialized (loaded from an external source), a `JsonSerializationException` will be thrown.
- `OutputFormat` - You already read about output formats above.  This is the property that controls it all.  By default, a collapsed hierarchy is returned.
