# Serialization

The primary objective of every other JSON library that I have found is serialization; that is, converting an object into a JSON-formatted string.  The problem with existing implementations is that they gloss over the part about building the JSON structure, which hides the actual structure from the library consumer.  To contrast this behavior, Manatee.Json neatly defines and exposes the underlying structure to the consumer.  The main benefit of this approach is that the consumer can browse and even edit the JSON structure before serializing it to a string or deserializing it into a model.

The process of serializing an object checks for certain cases.  These cases are listed in order of descending priority.

1. Has the object already been serialized during the current call?
    - This ensures that each object is only serialized once and to maintain reference trees when deserializing.  Further attempts to serialize the object will place a reference marker.  The deserialization process will key on this marker to maintain object references.
2. Is the object a (JSON) primitive type or an enumeration?
    - Strings, numeric values, and booleans are considered primitive types for JSON.  Serialization of other types requires more information.
    - Enumerations are optionally serialized to their numeric value or to their string representation.
3. Does the object implement `IJsonSerializable`?
    - By implementing `IJsonSerializable`, an object expresses that it has a preferred format for JSON serialization.  The serializer respects this preference.
4. Does a pair of converter methods exist for the object’s type?
    - For types that do not implement `IJsonSerializable`, are not primitive, and will not be auto-serialized (see next point) as desired, a public registry exists that manages pairs of methods which convert those types to and from the JSON structure.
5. Attempt to auto-serialize.
    - If none of the conditions above are met, the serializer will attempt to de/serialize the object based on a best-guess and a few other rules.
    - If this fails, `JsonValue.Null` is returned for serialization, and the type’s default value is returned for deserialization.

Furthermore, to reduce the size of the serialized data, any object whose value is the default for its type will be serialized to `JsonValue.Null`.  By default, the serializer will omit these values from the output structure.

## Maintaining References

Object references will be tracked by the serializer.  This is performed by an object cache that tracks every object that is serialized through a single call to the serializer.  This means that if the same object is referenced by multiple properties in the hierarchy, a reference marker (GUID) is created for those properties in the JSON representation, and the original object will receive a define marker with the same GUID.

With these markers, the deserialization process can properly reconstruct the object tree as it existed prior to serialization.

An a highly-contrived example, given the following classes:

```csharp
class Child
{
    public string Name { get; set; }
}

class Parent
{
    public string Name { get; set; }
    public IEnumerable<Child> Children { get; set; }
}
```

and the following structure:

```csharp
var joe = new Child { Name = "Joe" };
var sue = new Child { Name = "Sue" };
var alex = new Parent
    {
        Name = "Alex",
        Children = new[]{ joe, sue, joe }
    };
```

the resulting JSON would look like this:

```csharp
{
    "Name" : "Alex",
    "Children" : [
        {
            "#Def" : "1ef15b13-7063-4894-8a1c-5793f4957763",
            "Name" : "Joe"
        },
        {
            "Name" : "Sue"
        },
        {
            "#Ref" : "1ef15b13-7063-4894-8a1c-5793f4957763"
        }
    ]
}
```

When the serializer encounters the reference key, it knows to look up that GUID and insert the corresponding object.

> **NOTE** Due to the nature of value types (structs), maintaining references is not enabled for them.

## IJsonSerializable

This interface has been created to allow objects to define their own serialization requirements.  In this way, non-public members may be serialized and deserialized.  These requirements will be respected when the object is serialized as a member of another object.  The interface exposes two methods:

- `JsonValue ToJson(JsonSerializer serializer)`
    - Returns a JSON representation of the object.
- `void FromJson(JsonValue json, JsonSerializer serializer)`
    - Given a JSON representation, this method assigns values to the pertinent data members.

At minimum, the `FromJson()` method should correctly deserialize the output from the `ToJson()` method.

> **NOTE** The serializer cannot prevent default values from appearing in the JSON structure when these methods are used.

## JsonSerializationTypeRegistry

This class has been implemented as a work-around for those objects which are not controlled by the client and will not be auto-serialized properly or as desired.  Many explicit types, such as `DateTime`, and generic types, such as `Nullable<T>`, are examples of these.

Some common explicit types are automatically registered.  These include:

- `System.DateTime`
- `System.TimeSpan`
- `System.Guid`
- `System.Uri`
- *more to come as they are requested*

To register explicit types such as these, first two methods must be implemented: one that converts to a `JsonValue`, and one that converts back from a `JsonValue` to an instance of the type.  Then a call is made to the `RegisterType<T>()` method.  For example, registering the `System.Drawing.Point` object could be implemented as follows:

```csharp
public static class Program
{
    JsonSerializer _serializer = new JsonSerializer();

    public void Main()
    {
        JsonSerializationTypeRegistry.RegisterType(PointToJson, JsonToPoint);
        var json = _serializer.Serialize(new Point(5, 6));
    }

    static JsonValue PointToJson(Point p, JsonSerializer serializer)
    {
        return new JsonObject { {"x", p.x}, {"y", p.y} };
    }

    static Point JsonToPoint(JsonValue p, JsonSerializer serializer)
    {
        return new Point(json.Object["x"].Number, json.Object["y"].Number);
    }
}
```

At any time, the serialization methods for a type can be changed by calling the `RegisterType<T>()` method with a new method pair.  This means, for example, that if the default encoding for `DateTime` is not preferred for the application, the behavior can be overridden by registering new methods.  In this way, serialization is completely customizable.

Some generic types are automatically registered.  These include:

- `Nullable<T>`
- `T[]` *(not a generic, but included)*
- `List<T>`
- `Dictionary<TKey, TValue>`
- `Queue<T>`
- `Stack<T>`
- *more to come as they are requested*

> **NOTE** The default behavior for serializing a `Dictionary<TKey, TValue>` yields a `JsonArray` of `JsonObjects`, each containing “Key” and “Value” items.  This is done because the JSON object structure only supports strings as keys, and TKey may not be representable as a string.

For generic types that are not listed above, simply call `RegisterType<T>()` on the explicitly-defined type.  For instance, if a generic class `MyGenericClass<T>` exists, and a you need to register a `MyGenericClass<int>`, create the required conversion methods and call `RegisterType<MyGenericClass<int>>()`.

To unregister a type, simply call `RegisterType<T>()` and pass nulls for both methods.  If only one method is null, an exception is thrown.  This works for both explicit types and explicitly-defined generic types.

- `RegisterType<int>(null, null)`
- `RegisterType<List<int>>(null, null)`

This approach can be likened to implementing `IJsonSerializable` on third-party types.  The ideal solution would be to create one or more static classes which contain these conversion methods, then make a single call to a static method to register them all.  Once registered, they remain in memory throughout the life of the application or until they are unregistered.

## Auto-serialization

The serializer can automatically serialize most types.  There are some notes to consider, however:

- All of the properties to serialize must be implemented with public getters and setters,
    - For instance, the `DateTime` object cannot be automatically serialized because it exposes no public properties that have both public getters and public setters.
- Properties of interface or abstract class types can be serialized, but the value’s type will be explicitly listed in the serialization.
    - Serialization of properties of these types will result in a `JsonObject` with an additional key:  `“#Type”`.  This is used to indicate the serialized value’s assembly-qualified type name.
- Any property marked with the `JsonIgnore` attribute will not be serialized.
- Any property marked with the `JsonMapTo` attribute will be serialized to the supplied key, not to the property name.  This can be used to support non-property-friendly keys within JSON objects.

Any type that does not implement `IJsonSerializable` and cannot be properly auto-serialized will need to be registered, or else it will be ignored by the serializer. 

## Type Serialization

In addition to serializing objects, the serializer can also serialize the public static properties of types.  To do this, simply call the `SerializeType<T>()` method with the desired type.

No type information is encoded in the JSON structure for type serializations.  It remains the responsibility of the client to track the various JSON structures that are created to know which types they represent.

## Deserialization

To deserialize an object, call the `Deserialize<T>()` method with the appropriate type argument and pass in the JSON representation of the object.  The deserializer will parse out which properties are set.

To deserialize a type, call the `DeserializeType<T>()` method with the appropriate type argument.
 
If the JSON structure contains keys that are not properties of the object or type, the associated values are ignored by default.  Any properties which are not explicitly set in the JSON structure will remain the default value for the type of that property.

Deserializing the type `object` or `dynamic` will result in an `ExpandoObject` that contains all of the properties contained within the JSON.  Since the types of nested properties cannot be determined, they will also be deserialized as `ExpandoObject`.

### Validation during deserialization

If so desired, JSON can be validated by a JSON Schema before deserialization of client-defined types.  The schema must be available either:

- by URI (on the web or in a remote or local file) or
- on the class as a public static property

To enable validation, place the `SchemaAttribute` on the type definition.

```csharp
[Schema("http://json-schema.org/geo")]
public class Geo
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}
```

or

```csharp
[Schema(nameof(Schema))]
public class Geo
{
    public static IJsonSchema Schema =>
        new JsonSchema06
        {
            Id = "http://json-schema.org/geo",
            Schema = "http://json-schema.org/draft-06/schema#",
            Description = "A geographical coordinate",
            Type = JsonSchemaType.Object,
            Definitions = new Dictionary<string, IJsonSchema>
            {
                ["Longitude"] = new JsonSchema06 { Type = JsonSchemaType.Number },
                ["Latitude"] = new JsonSchema06 { Type = JsonSchemaType.Number }
            }
        }

    public double Longitude { get; set; }
    public double Latitude { get; set; }
}
```

## Deserialization of abstract class and interface types

The serializer has the ability to automatically deserialize abstract class and interface types.  There are two mechanisms through which this is performed.

- `JsonSerializationAbstractionMap`
- Auto-generation of interface types (JIT type creation)

The `JsonSerializationAbstractionMap` static class maps an abstraction type to an implementation of that abstraction.  During deserialization the registered implementation type will be instantiated and returned, having been casted to the abstraction type requested.

To create a mapping between an abstraction and its implementation, simply call the `Map<TAbstract, TConcrete>()` method.  At any time the mapping can be removed by using the `RemoveMapping<TAbstract>()` method.

```csharp
// To create a map:
JsonSerializationAbstractionMap.Map<IEnumerable, Array>();
// To remove the map:
JsonSerializationAbstractionMap.RemoveMapping<IEnumerable>();
```

You can also map an open generic abstract or interface type, like `IEnumerable<T>,` to an open generic concrete type, like `List<T>`.  This will tell the serializer to use `List<T>` whenever `IEnumerable<T>` is requested.

```csharp
JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
```

> **NOTE** For this example, you must still register the specific `List<T>` types using the `JsonSerializationTypeRegistry` methods mentioned above.

The other mechanism, type generation, should only be used when a compile-time solution is not possible.  If the requested type is an interface, the system has the ability to dynamically create a type which implements that interface.  If relying on this feature, please note the following:

- Properties and indexers with both getters and setters will correctly return the values to which they are set.
- Methods do not process any inputs.
- Methods with return values will only return the default value for the return type.
- Events will not be raised, although they can be subscribed to.

The generated types are, for all intents and purposes, dummy classes.

## Object Resolution

The serializer must (obviously) create instances of objects in order to deserialize to them.  This is achieved via a configurable resolver.

The default resolver can be replaced with any DI container you wish to use.

The default resolver uses reflection to discover constructors on a type and chooses one in order to attempt to create and instance, passing in default values for any of the parameters.  This may result in an exception being thrown that the instance cannot be created.  

One example is `Guid`.  The simplest constructor for this type takes a string, but throws an exception if the string is null.  Unfortunately, this is precisely what the default resolver tries to do.

To combat this, a simple resolver decorator can be created that properly resolves `Guid`s, but passes other types on to the default resolver.

```csharp
class ResolverDecorator : IResolver
{
    private IResolver _innerResolver;
    public ResolverDecorator(IResolver innerResolver)
    {
        _innerResolver = innerResolver;
    }

    public T Resolve<T>()
    {
        if (typeof(T) == typeof(Guid)) return Guid.NewGuid();
        else return _innerResolver.Resolve<T>();
    }

    public object Resolve(Type type)
    {
        if (type == typeof(Guid)) return Guid.NewGuid();
        else return _innerResolver.Resolve(type);
    }
}
```

This can then be set in the serializer options (see below):

```csharp
serializer.Options.Resolver = new ResolverDecorator(serializer.Options.Resolver);
```

## Serialization Options

The `JsonSerializerOptions` object represents a series of options available for the serializer.  Each serializer instance may have its own options.  There is also a static default options object.  The options for the serializer are as follows:

- `EncodeDefaultValues`
    - `true` will encode properties whose values are the default for the type.
    - `false` will ignore these properties.
    - The default is `false`.
- `InvalidPropertyKeyBehavior`
    - `DoNothing` will ignore properties that do not belong to a type during deserialization.
    - `ThrowException` will throw an exception when the deserializer encounters these properties.
    - The default is `DoNothing`.
- `DateTimeSerializationFormat`
    - `Iso8601` outputs a string formatted to meet the requirements of ISO 8601.
    - `JavaConstructor` outputs a string in the format "/Date(ms)/", where ms is the number of milliseconds since January 1, 1970.
    - `Milliseconds` outputs the number of milliseconds since January 1, 1970 as a numeric value.
    - `Custom` uses the format in the `CustomDateTimeSerializationFormat` property.
    - The default is `Iso8601`.
    - See [http://weblogs.asp.net/bleroy/archive/2008/01/18/dates-and-json.aspx](http://weblogs.asp.net/bleroy/archive/2008/01/18/dates-and-json.aspx) for more information regarding encoding dates and times into JSON constructs.
- `CustomDateTimeSerializationFormat`
    - Defines a custom format for serializing `DateTime` objects.
    - Only applicable when `DateTimeSerializationFormat` is set to `Custom`.
- `EnumSerializationFormat`
    - `AsInteger` will encode enumerations to their numeric value.
    - `AsName` will encode enumerations to their name (string) value.
    - The default is `AsInteger`.
- `CaseSensitiveDeserialization`
    - Controls whether the auto-serializer considers case while deserializing.
    - The default is `false`.
- `AlwaysSerializeTypeName`
    - Controls whether the auto-serializer always includes the type name when serializing.
    - The default is `false`.
- `Resolver`
    - Provides an interface to inject a custom type resolver.  This is useful when a DI container is being used to control object lifetime.
    - The default is `null`.  (An internal implementation is used in this case which uses reflection to discover constructors.)
- `AutoSerializeFields`
    - `true` will encode public fields.
    - `false` will ignore fields.
    - The default is `false`.
- `SerializationNameTransform`
    - Provides a key transformation method used during serialization; for instance, reversing the string.
    - The default implementation provides no transformation.
- `DeserializationNameTransform`
    - Provides a key transformation method used during deserialization; for instance, un-reversing the string.
    - The default implementation provides no transformation.
- `OnlyExplicitProperties`
    - `true` will only serialize those properties defined by the type in the `.Serialize<T>()` call.
    - `false` will serialize all properties defined by the object.
    - The default is `false`
