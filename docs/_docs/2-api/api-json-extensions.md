---
title: Extension Methods
category: API Reference
order: 2
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# LinqExtensions

These extension methods cover LINQ compatibility.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- LinqExtensions

## Methods

### static T FromJson&lt;T&gt;(JsonObject json, JsonSerializer serializer)

Deserializes a [JsonValue](API-Json#jsonvalue) to its equivalent object.

**Type Parameter:** T : class, [IJsonSerializable](API-Serializer#ijsonserializable), new()

The type of object

**Parameter:** json

The [JsonValue](API-Json#jsonvalue) to deserialize

**Parameter:** serializer

The [JsonSerializer](API-Serializer#jsonserializer) instance to use for additional
serialization of values.

**Returns:** A collection of the deserialized objects

# XmlExtensions

Contains functionality to map JSON values to XML constructs.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json

**Inheritance hierarchy:**

- Object
- XmlExtensions

## Methods

### static [JsonValue](API-Json#jsonvalue) ToJson(XElement xElement)

Converts an System.Xml.Linq.XElement to a [JsonObject](API-Json#jsonobject).

**Parameter:** xElement

An System.Xml.Linq.XElement.

**Returns:** The [JsonValue](API-Json#jsonvalue) representation of the System.Xml.Linq.XElement.

### static System.Xml.Linq.XElement ToXElement(JsonValue json, string key)

Converts a [JsonValue](API-Json#jsonvalue) to an XElement

**Parameter:** json

A [JsonValue](API-Json#jsonvalue).

**Parameter:** key

The key to be used as a top-level element name.

**Returns:** An System.Xml.Linq.XElement representation of the [JsonValue](API-Json#jsonvalue).

**Exception:** System.ArgumentException

Thrown if *key* is null, empty, or whitespace
and *json* is not a non-empty [JsonObject](API-Json#jsonobject).

#### Remarks

The &#39;key&#39; parameter may be null only when the underlying JSON is an
object which contains a single key/value pair.

