# JSON Path

The concept of JSONPath is widely credited to Stefan Goessner.  His [blog post](http://goessner.net/articles/JsonPath/) describes it as a parallel to XPath but for JSON instead of XML.  In short, the syntax allows one to query a JSON object for one or more values.  While this page covers the basics of JSONPath, reading the blog post is still worthwhile.

## Concept

As with any programming language, JsonPath is constructed using a series of operators and values.  These work together to describe which values are returned and which are filtered out.  Below is a list of the operators.

- `$` - The root of the JSON.  Can be used inside JavaScript expressions as well (see `()` and `?()`).
- `.` - Indicates the desired value should be an object.  E.g. `$.` would return the value at the root only if it is an object.
- `[]` - Indicates the desired value is an array.  E.g. `$[]` would return the value at the root only if it is an array.
    - Brackets must not be empty.
    - A number within brackets specifies an index of the array to return.
    - Numbers can be comma-separated to return multiple specific indices.
    - Numbers can be colon-separated to indicate a start-end-step sequence. See [this StackOverflow post](http://stackoverflow.com/a/509295/878701) for a concise description of the syntax.
- `..` - Performs a recursive search for the key that follows it.  E.g. `..store.` performs a recursive search for an object that contains the key `store` (and `..[]` recursively searches for an array).
- `*` - A wildcard.  Returns all values at the current level.  May appear after `.` or `..` or inside `[]`.
- `()` - Surrounds a JS expression which evaulates to an index.  \*
- `?()` - Surrounds a JS expression which evaluates to a boolean.  Used as a filter.  \*
- `@` - Self-referencing operator.  When inside `()`, references the array; when inside `?()`, references the current value while iterating; invalid elsewhere.

\* Only avlid inside the `[]` operator.

In order to reference specific keys within a JSON object, the key itself is used.  For instance, if the root is a JSON object `$.store` will return the value under the `store` key, if it exists.  There is one exception to this: `length` is a reserved keyword and will return the length of an array.  For example, given the JSON `[5, true, "string"]`, the path `$.length` will return `[3]`.

These operators can be chained together to create very complex paths.

The results returned from evaluating a path is a JSON array containing the matching values, if any.

> **NOTE** Although Mr. Goessner's post states that `false` should be returned for an empty result set, he does allow for an empty array to be optionally returned.  Manatee.Json takes this approach since client code will be simpler when a single return type is always expected.

## JavaScript Expression support in JsonPath

The `()` and `?()` operators require an expression to be entered.

The expression inside `()` must evaluate to an integer index.  If the index is within the bounds of the array, the value at this index will be returned.

The expression inside `?()` must evaluate to a boolean.  This expression will be evaluated for each value in the array.  If the expression returns `true`, the value is included; otherwise it is excluded from the results.

Inside a search expression, the array or current value may be referenced using the `@` operator.  This functions the same as the root operator `$` but targets the array or current value instead.  However, the `$` operator can still be used to back-reference the root as well.

> **NOTE**  Since the `()` operator does not iterate through the array, the target of the `@` operator is the array itself rather than an item contained within it.

## Building a JSONPath with Manatee.Json

To represent JSONPaths, Manatee.Json exposes the `JsonPath` object.  To create a `JsonPath` object, the `JsonPathWith` static class has been provided.  This class has static and extension methods which can be used to create any sort of path desired.  Below is a listing of the available methods.  All of these methods are available as extension methods as well, which allows them to be chained together.

> **NOTE**  The extension methods return a new `JsonPath` instance rather than modifying the passed instance.  This behavior emulates that of Linq extension methods.

- `Name()` - Adds `.*`.
- `Name(string)` - Adds a `.` operator and a key.
- `Length()` - Adds `.length`.
- `Array()` - Adds `[*]`
- `Array(params Slice[])` - Adds `[]` with a list of indices and slices to return.
- `Array(Expression<Func<JsonArray, int>>)` - Adds `[()]` with a function which takes a `JsonArray` and returns an `int`.
- `Array(Expression<Func<JsonValue, bool>>)` - Adds `[?()]` with a function which takes a `JsonValue` and returns a `bool`.
- `Search()` - Adds `..*`.
- `Search(string)` - Adds a `..` operator and a key.
- `SearchLength()` - Adds `..length`.
- `SearchArray()` - Adds `..[*]`
- `SearchArray(params Slice[])` - Adds `..[]` with a list of indices and slices to return.
- `SearchArray(Expression<Func<JsonArray, int>>)` - Adds `..[()]` with a function which takes a `JsonArray` and returns an `int`.
- `SearchArray(Expression<Func<JsonValue, bool>>)` - Adds `..[?()]` with a function which takes a `JsonValue` and returns a `bool`.

> **TIP** There is an implicit conversion from `int` to `Slice` so that you can pass just individual indexes into the `Array(params Slice[])` and `SearchArray(params Slice[])` methods.  This means that individual indexes mixed with slices are supported.  For example `JsonPathWith.Array(1, new Slice(3, 5))` will construct the path `$[1,3:5]`.

You may have noticed that a few of these take Linq Expressions as their argument.  This is to facilitate easy creation of JSONPath expression arguments in a way familiar to .Net developers.  It also provides compile-time checking on the expressions.

Two static classes have been created to aid in the creation of these expressions.  One that contains extension methods for `JsonValue` and `JsonArray`, and another which contains non-extension static versions of the same methods.  The reasoning for having two classes will be explained shortly.  For now let's have a look at the extension methods in `PathExpressionExtensions`.

- `Length()` - Returns the length of an array.  Extends `JsonValue` and `JsonArray` for use in both kinds of expressions.
- `HasProperty(string)` - Returns `false` if the current value:
    - is not an object,
    - is an object but does not contain the indicated key,
    - is an object and contains the indicated key, and the key's value is `false`.
- `Name(string)` - If the value is an object, returns the value under the indicated key; otherwise `null`.
- `ArrayIndex(int)` - If the value is an array, returns the value at the indicated index; otherwise `null`.
- `IndexOf(JsonValue)` - If the value is an array, returns the index of the indicated value if it is found; otherwise -1.
    - This can be useful when attempting to ascertain whether an array contains a specific value:  `.IndexOf() != -1` can be used like a `Contains()`.

> **NOTE** All of these extension methods can be considered `null`-proof.  That is, they will handle the case where they are called on a `null` object without throwing an exception.

The above methods allow you to build paths on the value passed into the expression (either a JsonValue or JsonArray).  If you'd like to build a path on the root object within an expression, use the `JsonPathRoot` static class to start the path.  (These non-extension methods are specifically purposed for referencing the root.  This is why they're segregated into a separate class.)

## Examples

Below is the Manatee.Json implementations of the examples presented on Mr. Goessner's post.

- `$.store.book[*].author`
    - `JsonPathWith.Name("store").Name("book").Array().Name("author")`
- `$..author`
    - `JsonPathWith.Search("author")`
- `$.store.*`
    - `JsonPathWith.Name("store").Name()`
- `$.store..price`
    - `JsonPathWith.Name("store").Search("price")`
- `$..book[2]`
    - `JsonPathWith.Search("book").Array(2)`
- `$..book[(@.length-1)]`
    - `JsonPathWith.Search("book").Array(a => a.Length() - 1)`
- `$..book[-1:]`
    - `JsonPathWith.Search("book").Array(new Slice(-1, null))`
- `$..book[0,1]`
    - `JsonPathWith.Search("book").Array(0, 1)`
- `$..book[:2]`
    - `JsonPathWith.Search("book").Array(new Slice(null, 2))`
- `$..book[?(@.isbn)]`
    - `JsonPathWith.Search("book").Array(v => v.HasProperty("isbn"))`
- `$..book[?(@.price<10)]`
    - `JsonPathWith.Search("book").Array(v => v.Name("price") < 10)`
- `$..*`
    - `JsonPathWith.Search()`

Here's an example of a path with an expression which contains a path referencing the current value and the root:

- `$..book[?(@.price<$.store.bicycle.price)]`
    - `JsonPathWith.Search("book").Array(v => v.Name("price") < JsonPathRoot.Name("store").Name("bicycle").Name("price"))`

You can perform arithmetic operations within your expressions:

- `$..book[?(@.price+1<15.99)]`
    - `JsonPathWith.Search("book").Array(v => v.Name("price") + 1 < 15.99)`

You can even use local and instance fields in your expression:

```csharp
double price = 15.99;
var path = JsonPathWith.Search("book").Array(v => v.Name("price") + 1 < price);
```

As the field updates, the path evaluation will change accordingly.

> **IMPORTANT** While you may use local and instance fields *in your expressions*, the extension methods currently only support literal arguments.  Passing fields, properties, or method calls into these methods will throw a `NotSupportedException`.

> **IMPORTANT** The methods contained within `PathExpressionExtensions` and `JsonPathRoot` will throw a `InvalidOperationException` if called outside of the context of one of the `JsonPathWith` methods.

## Parsing

Parsing string representations of JSONPaths is quite simple.  The `JsonPath` class exposes a `Parse(string)` method.  If there is a problem with the string which is passed, it will throw a `JsonPathSyntaxException` which contains the portion of the path which was successfully parsed.

The return is the `JsonPath` instance.  It's really that simple.  The benefit of building the path with the extension methods is that you're pretty much guaranteed that the path will be valid if it compiles.

## Working with `JsonPath`

Evaluating JSONPaths is easy.  The `JsonPath` class exposes the `Evaluate(JsonValue)` method.  This method will return a `JsonArray` containing all matches (or empty if there were none).

> **NOTE** The return deviates slightly from Mr. Groessner's implementation in that an empty array is returned when there are no matches, as opposed to a `false` value.