`ValueArray` is an immutable array with value equality, that is, two arrays are equal if they have the same contents.

To install the [nuget package](https://www.nuget.org/packages/ValueCollections.ValueArray):
```
dotnet add package ValueCollections.ValueArray --prerelease
```

Example usage:

```csharp
using ValueCollections;

// Equality based on contents, not references
ValueArray.Create(1, 2, 3) == ValueArray.Create(1, 2, 3); // true

// This holds whether it is stored in a record, a tuple, 
// or anything else that compares using default equality comparers.
record DataBlock(
    ValueArray<string> Entries
);

var db0 = new DataBlock(ValueArray.Create("a", "b"));
var db1 = new DataBlock(ValueArray.Create("a", "b"));
db0 == db1 // true

// Works as a key in Dictionary, HashMap or anything that uses GetHashCode.
var dict = new Dictionary<ValueArray<int>, string>
{
    [ValueArray.Create(1, 2, 3)] = "Entry1"
};
dict[ValueArray.Create(1, 2, 3)]; // "Entry1"

// Nice, structural "ToString()" recursively prints nested data,
// making it a joy to use in scripting, logging and debugging.
ValueArray.Create(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }).ToString()
"ValueArray(2) { Array(3) { 1, 2, 3 }, Array(3) { 4, 5, 6 } }"

// Supports C# 8 slices and ranges:
var slice = valueArray[1..^1];

// Supports C# 12 collection expressions*:
ValueArray<int> valueArray = [..span0, item, ..span1];

// Seamless interop to and from LINQ:
ValueArray<int> items = ValueArray.Create(1, 2, 3);
ValueArray<int> oddsSquared = items.Where(i => i % 2 == 1).Select(i => i * i).ToValueArray();
"ValueArray(2) { 1, 9 }"

// Update operations are non-destructive:
var newValueArray = valueArray.Append(item); // does not modify the original
var newValueArray = valueArray.SetItem(2, item); // use this instead of valueArray[2] = item;
```

__`ValueArray` is highly unstable and experimental at this stage.__
Not every method is yet covered by unit tests. The design might still change.

### Why do we need this?
`Arrays`, `Lists`, `Dictionaries` and even collections from `System.Collections.Immutable` compare by reference.
The introduction of records and tuples in C#, which compare by value, mean that there's a need for a collection
type that compares by value too.
[People](https://stackoverflow.com/q/63813872)
[regularly](https://stackoverflow.com/q/67462463)
[ask](https://stackoverflow.com/q/69354610)
[for](https://stackoverflow.com/q/71008787)
[this](https://stackoverflow.com/q/70304163).

### Why should it be immutable?
Anything that supports equality should be immutable, since it can be used as keys in dictionaries and maps. In a DDD sense, this type represents a value, not an entity.

### Why is this a reference type when `ImmutableArray` is a value type?
Value types support `default` initialization, which would be an invalid state for this type 
(it would throw `NullReferenceException` when you'd' try to do anything with it).
This is easy to run into and the compiler wouldn't be able to help you spot it.
`ImmutableArray` seems to cater to experts writing low-allocation code (e.g. Roslyn); `ValueArray` tries to be more general-purpose.

### Performance considerations
`foreach` is optimized not to allocate and the performance is on par with native arrays.

We try to leverage available optimizations in LINQ as well, allowing casts to `ICollection` and `IList` (but you should not use these interfaces).

Of course Equality and `GetHashCode` become O(n) since they have to consider all elements.

Equality leverages all optimizations in `Enumerable.SequenceEquals`, e.g. arrays of blittable structs get optimized to a memcmp.

`GetHashCode` relies on `System.HashCode` for security and performance.

`ToString` strongly optimizes for usefulness over speed. It should be fine for logging, but if squeezing every bit of performance matters,
you might want to implement your own.

* Perf warning: note that collection expressions are less efficient than using the type's own methods directly. `[]` incurs a run-time length check, `ValueArray<T>.Empty` does not. Object spread syntax incurs a redundant temporary copy, `ValueArray.Create` and `ValueArray.ToValueArray` don't. For the technical details, see [this issue](https://github.com/dotnet/csharplang/discussions/9697) on the C# design repo. Please upvote it if this matters to you.

### Can I provide a custom comparer?
No. I may consider adding this feature at a later point.

### Can I use this on .NET Framework?
Yes, provided you are using a .NET Standard 2.0 compatible version.
