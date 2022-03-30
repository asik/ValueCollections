`Block` is an immutable array type with structural equality. It is based on `System.Collections.Immutable.ImmutableArray`, a standard type in .NET.

```csharp
using ValueCollections;

// Equality based on the contents
Block.Create(1, 2, 3) == Block.Create(1, 2, 3); // true

// Works inside of records
record DataBlock(int Index, Block<string> Entries);
var db0 = new DataBlock(3, Block.Create("a", "b"));
var db1 = new DataBlock(3, Block.Create("a", "b"));
db0 == db1; // true

// Works as a key in Dictionary, HashMap
// or anything that uses GetHashCode.
var dict = new Dictionary<Block<int>, string>
{
    [Block.Create(1, 2, 3)] = "Entry1"
};
dict[Block.Create(1, 2, 3)]; // "Entry1"

// Plays well with all of .NET
var items = Block.Create(1, 2, 3);
var odds = items.Where(i => i % 2 == 1);
var list = new List<int>(items);
```

__`Block` is highly unstable and experimental at this stage.__

## Why the name `Block`?
It's short and it's consistent with the equivalent [planned F# feature also based on `ImmutableArray`](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1094-block.md).

## Why do we need this?
Comparing objects for equality is common and it's becoming more common with record types. Unfortunately, all the collection types in .NET default to reference equality, which means they don't work in records.

## Why should it be immutable?
Anything that supports equality should be immutable, since it can be used as keys in dictionaries and maps. In a DDD sense, this type represents a value, not an entity.

## Why is this a `struct`?
For the same reason as `ImmutableArray`: reducing overhead. Since it's immutable, it doesn't matter that it's passed by value. This has the unfortunate consequence that the type has a default constructor that leaves it uninitialized, throwing `NullReferenceException` when you try to do anything with it except check `IsDefault` or `IsDefaultOrEmpty`. For now, I'm following `ImmutableArray`'s design to the letter, but I'm pondering if this can be improved on.

## Does this create a copy of the entire array when I use it as a function argument?
No, it copies a reference. It's the same as `ImmutableArray` in that regard (and almost every other).

## Won't this be slow compared to `T[]`?
Creation, iteration, passing around and index access should be the same as regular arrays; this is an `ImmutableArray` which is really just a `T[]` under the hood.
Any mutation will involve a full copy; that is O(n). For building up a collection, I would suggest `ImmutableList` for now, which is built to be very efficient at adding and removing elements. For memory-like access, `T[]` is still fine, but .NET also now has more specialized types like `Span` and `Memory`.

