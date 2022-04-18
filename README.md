`Block` is an immutable array with value equality, that is, two arrays are equal if they have the same contents.

To install the [nuget package](https://www.nuget.org/packages/ValueCollections.Block):
```
dotnet add package ValueCollections.Block --prerelease
```

Example usage:

```csharp
using ValueCollections;

// Equality based on contents, not references
Block.Create(1, 2, 3) == Block.Create(1, 2, 3); // true

// This holds whether it is stored in a record, a tuple, 
// or anything else that compares using default equality comparers.
record DataBlock(Block<string> Entries);

var db0 = new DataBlock(Block.Create("a", "b"));
var db1 = new DataBlock(Block.Create("a", "b"));
db0 == db1 // true

// Works as a key in Dictionary, HashMap or anything that uses GetHashCode.
var dict = new Dictionary<Block<int>, string>
{
    [Block.Create(1, 2, 3)] = "Entry1"
};
dict[Block.Create(1, 2, 3)]; // "Entry1"

// Nice, structural "ToString()" recursively prints nested data,
// making it a joy to use in scripting, logging and debugging.
Block.Create(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }).ToString()
"Block(2) { Array(3) { 1, 2, 3 }, Array(3) { 4, 5, 6 } }"

// Supports C# 8 slices and ranges:
var slice = block[1..^1];

// Seamless interop to and from LINQ:
Block<int> items = Block.Create(1, 2, 3);
Block<int> oddsSquared = items.Where(i => i % 2 == 1).Select(i => i * i).ToBlock();
"Block(2) { 1, 9 }"

// Update operations are non-destructive:
var newBlock = block.Append(item); // does not modify the original
var newBlock = block.SetItem(2, item); // use this instead of block[2] = item;
```

__`Block` is highly unstable and experimental at this stage.__
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

### Why the name `Block`?
It's short and it's consistent with the equivalent [planned F# feature also based on `ImmutableArray`](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1094-block.md).

### Why should it be immutable?
Anything that supports equality should be immutable, since it can be used as keys in dictionaries and maps. In a DDD sense, this type represents a value, not an entity.

### Why is this a reference type when `ImmutableArray` is a value type?
Value types support `default` initialization, which would be an invalid state for this type 
(it would throw `NullReferenceException` when you'd' try to do anything with it).
This is easy to run into and the compiler wouldn't be able to help you spot it.
`ImmutableArray` seems to cater to experts writing low-allocation code (e.g. Roslyn); `Block` tries to be more general-purpose.

### Won't this be slow compared to `T[]`?
It leverages optimizations in `ImmutableArray` to make `for` and `foreach` as fast as or faster than any other collection.

It tries to leverage available optimizations in LINQ as well.

Equality is as fast or faster than `SequenceEquals`, except for arrays of blittable types, which .NET optimizes to a memcmp.

`GetHashCode` is fast but O(n) since it considers all elements.

`ToString` strongly optimizes for usefulness over speed. It should be fine for logging, but if squeezing every bit of performance matters,
you might want to implement your own.

### How can I customize equality?
Derive from the type, implement `IEquatable<T>.Equals` and override `GetHashCode` so that two instances that compare equal also return the same hash code.

Alternatively, the `IImutableList` interface provides some methods that allow you to pass an `EqualityComparer`.

I'm aware that this is not great, but that's also how records and tuples work. 
Allowing you to pass an `EqualityComparer` at creation would significantly alter and complicate the design of this type, I think.

### Can I use this on .NET Framework?
Yes, provided you are using a .NET Standard 2.0 compatible version (4.6.2 and above, I believe.)
Side note: you can use records on .NET Framework.
