---
title: JSON Patch
category: API Reference
order: 10
---

> In setting up this GitHub Pages site, many of the links have been broken.  I am efforting to correct this.  I apologize for the inconvenience.

# JsonPatch

Models JSON Patch documents.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Patch

**Inheritance hierarchy:**

- Object
- List&lt;JsonPatchAction&gt;
- JsonPatch

## Fields

### static [JsonSchema04](API-Schema-4#jsonschema04) Schema

Provides a schema that can be used to validate JSON Patch documents before deserialization.

## Methods

### [JsonPatchResult](API-Patch#jsonpatchresult) TryApply(JsonValue json)

Attempts to apply the patch to a JSON instance.

**Parameter:** json



**Returns:** 

# JsonPatchAction

Defines an action that can be applied within a JSON Patch document.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Patch

**Inheritance hierarchy:**

- Object
- JsonPatchAction

## Properties

### string From { get; set; }

Gets or sets the source for a value.

### [JsonPatchOperation](API-Patch#jsonpatchoperation) Operation { get; set; }

Gets or sets the operation.

### string Path { get; set; }

Gets or sets the path.

### [JsonValue](API-Json#jsonvalue) Value { get; set; }

Gets or sets a discrete value to be used.

# JsonPatchOperation

Defines available operations for JSON Patch actions.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Patch

**Inheritance hierarchy:**

- Object
- ValueType
- Enum
- JsonPatchOperation

## Fields

### Add

Indicates an addition operation.

### Remove

Indicates a removal operation.

### Replace

Indicates a replacement operation.

### Move

Indicates a movement operation.

### Copy

Indicates a copy operation.

### Test

Indicates a test operation.

# JsonPatchResult

Provides the results of a JSON Patch application.

**Assembly:** Manatee.Json.dll

**Namespace:** Manatee.Json.Patch

**Inheritance hierarchy:**

- Object
- JsonPatchResult

## Properties

### string Error { get; }

Gets any errors that have occurred during a patch.

### [JsonValue](API-Json#jsonvalue) Patched { get; }

The resulting document, if the patch was successful.

### bool Success { get; }

Gets whether the patch was successful.

