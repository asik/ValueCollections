`Block` is an immutable array with value equality. It builds upon `ImmutableArray<T>`, a standard type in .NET.

To install the [nuget package](https://www.nuget.org/packages/ValueCollections.Block):
```
dotnet add package ValueCollections.Block --prerelease
```

Example usage:

```csharp
using ValueCollections;

// Equality based on the contents
Block.Create(1, 2, 3) == Block.Create(1, 2, 3); // true

// Works inside of records
record DataBlock(
    int Index, 
    Block<string> Entries);

var db0 = new DataBlock(3, Block.Create("a", "b"));
var db1 = new DataBlock(3, Block.Create("a", "b"));
db0 == db1 // true

// And vice-versa. There's no depth limit, this is all based on default equality comparers.
record UserId(string Value);

var userIds0 = Block.Create(new UserId("abc"), new UserId("def"));
var userIds1 = Block.Create(new UserId("abc"), new UserId("def"));
userIds0 == userIds1 // true

// Works as a key in Dictionary, HashMap
// or anything that uses GetHashCode.
var dict = new Dictionary<Block<int>, string>
{
    [Block.Create(1, 2, 3)] = "Entry1"
};
dict[Block.Create(1, 2, 3)]; // "Entry1"

// Supports IEnumerable<T> and IReadOnlyList<T> for the widest interop
// possible with LINQ and other collection APIs:
var items = Block.Create(1, 2, 3);
var odds = items.Where(i => i % 2 == 1);
var list = new List<int>(items);

// Supports C# 8 slices and ranges:
var slice = block[1..^1];

// .ToBlock() extension method provides easy conversion for all existing collection types
myArray.ToBlock();
myArray.Where(condition).Select(selector).ToBlock();
myList.ToBlock();
myDictionary.ToBlock(); // not sure why you'd do this, but you can!

// Supports IImmutableList<T>, which means it can be used as a drop-in replacement for ImmutableList or ImmutableArray.

// Update operations are non-destructive:
var newBlock = block.Append(item); // does not modify the original
var newBlock = block.SetItem(2, item); // use this instead of block[2] = item;
```

__`Block` is highly unstable and experimental at this stage.__
Not every method is yet covered by unit tests. The design might still change.

## Rationale
### Why the name `Block`?
It's short and it's consistent with the equivalent [planned F# feature also based on `ImmutableArray`](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1094-block.md).

### Why do we need this?
Comparing objects for equality is common and it's becoming more common with record types. Unfortunately, all collection types in .NET, including those in `ImmutableCollections`, default to reference equality, which means they don't work in records.

### Why should it be immutable?
Anything that supports equality should be immutable, since it can be used as keys in dictionaries and maps. In a DDD sense, this type represents a value, not an entity.

### Why is this a reference type when `ImmutableArray` is a value type?
Value types support default (zero) initialization, which means either Block throws exceptions when in that state, or defends against them with checks, slowing down performance.
With nullable reference types, C# is also more helpful at telling whether you're forgetting to initialize something. We can leverage this by making `Block` a reference type.
`ImmutableArray` is a value type because it tries to be a zero-overhead alternative to `ImmutableList`. 
We don't have the same goals. `Block` could be based on `ImmutableList`, in theory.

### Won't this be slow compared to `T[]`?
It's a thin wrapper around a T[], so you're basically paying one extra allocation. Array access and iteration is as fast as regular arrays.
Any mutation will involve a full copy; that is O(n). For building up a collection, I would suggest `ImmutableList` for now, which is built to be very efficient at adding and removing elements. For memory-like access, `T[]` is still fine, but .NET also now has more specialized types like `Span` and `Memory`.

