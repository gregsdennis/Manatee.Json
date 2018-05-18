---
title: JSON Pointer
category: API Reference
order: 11
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# JsonPointer

Represents a JSON Pointer.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Pointer

**Inheritance hierarchy:**

- Object
- List&lt;string&gt;
- JsonPointer

## Constructors

### JsonPointer()

Creates a new [JsonPointer](API-Pointer#jsonpointer) instance.

## Methods

### static [JsonPointer](API-Pointer#jsonpointer) Parse(string source)

Parses a string containing a JSON Pointer.

**Parameter:** source

The source string.

**Returns:** A [JsonPointer](API-Pointer#jsonpointer) instance.

### [PointerEvaluationResults](API-Pointer#pointerevaluationresults) Evaluate(JsonValue root)

Evaluates the pointer against a JSON instance.

**Parameter:** root

The JSON instance.

**Returns:** The element the pointer references, if any.

### string ToString()

Returns a string that represents the current object.

**Returns:** A string that represents the current object.

#### Filterpriority

2

# PointerEvaluationResults

Provides results for a JSON Pointer evaluation.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Pointer

**Inheritance hierarchy:**

- Object
- PointerEvaluationResults

## Properties

### string Error { get; }

Gets any errors that may have resulted in not finding the referenced value.

### [JsonValue](API-Json#jsonvalue) Result { get; }

Gets the referenced value, if found.

