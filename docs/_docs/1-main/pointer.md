---
title: JSON Pointer
category:
order: 6
---

[JSON Pointer](https://tools.ietf.org/html/rfc6901) is a syntax that allows the selection of a specific value within a JSON document.  This is contrasted to JSON Path which is used to perform searches, returning multiple values.

## JSON Pointer

The syntax is fairly simple:

- The path starts with `/`.
- Each segment is separated by `/`.
- Each segment can be interpreted as either an array index or an object key.

For example, given the JSON document (from the spec linked above):

```json
{
    "foo": ["bar", "baz"],
    "": 0,
    "a/b": 1,
    "c%d": 2,
    "e^f": 3,
    "g|h": 4,
    "i\\j": 5,
    "k\"l": 6,
    " ": 7,
    "m~n": 8
}
```

The following pointers evaluate to the accompanying values:

```
<empty>      // the whole document
/foo"       ["bar", "baz"]
/foo/0"     "bar"
/"          0
/a~1b       1
/c%d        2
/e^f        3
/g|h        4
/i\j        5
/k"l        6
/           7
/m~0n       8
```

Note the paths `a~1b` and `m~0n`.  These show the only two characters which must be delimited: `~` and `/` which are delimited as `~0` and `~1`, respectively.

JSON Pointer also supports a URL syntax which starts with `#` and allows for characters to be encoded as `%xx` format, where `xx` is the hexadecimal equivalent of the character.

## Usage

Manatee.Json's implementation is the `JsonPointer` class.  This class extends `List<string>` to represent each segment individually.  It also exposes `Evaluate(JsonValue)`.  This method returns a result object that exposes either the value found at the path or an error describing where the path ended.

To create a path, you can either write out the path in a string and parse it using the `JsonPointer.Parse()` method:

```csharp
var pointer = JsonPointer.Parse("/foo/0");
```

or you can supply the segments individually or in a collection through one of the constructors:

```csharp
var pointer = new JsonPointer { "foo", "0" };
```

Both of these create the same path.

Once created, evaluation is simple:

```csharp
var sample = new JsonObject { /* example from above */ };
var results = pointer.Evaluate(sample);
```

This will return a PointerEvaluationResults object with `results.Value` equal to the JsonValue `0`.

In the case a path does not exist, the `results.Error` property will contain the path as far as the first segment that was missing.  For example, the path `foo/0/notFound/dn` would return an error with the path `foo/0/notFound`.