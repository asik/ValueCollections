## `ImmutableArray` vs `ImmutableList` as an implementation

Should we be based on `ImmutableArray` or `ImmutableList`? After all, we're going to support IImmutableList,
and `ImmutableList` is a better type for that purpose, optimized for adding/removing.

Why select a type that's going to be slow on updates if we want to support updates?

Rationale: if update performance is important, are you going to be happy with `ImmutableList`?
Or are you going to choose `List<T>` or `T[]` or something imperative.

I think the vast majority of use cases for this type will be simple updates where a single array allocation is fine.

For lots of updates where perf matters, you can use something mutable.

Advantages of `ImmutableArray` that we would lose by going to `ImmutableList`:
- Consistency with planned F# ValueArray type
- Array-like performance in creation, iteration
- 0 GC overhead over the underlying array (less overhead than `List<T>`!)

So it should be based on `ImmutableArray`.

## Reference vs Value type

Should it be a reference type or a value type?
`ImmutableArray` is a value type. It does this because it is designed as a low-overhead alternative to
`ImmutableList`. It's also unsafe, throwing `InvalidOperationException`/`NullReferenceException`
when uninitialized. We could make every function go through an if statement to make it safe, but that would
slow down usage.

`ImmutableArray` exposes `IsDefault`, `IsDefaultOrEmpty` and `IsEmpty`. It needs all 3

C# now has nullability checking for reference types, making them much safer than they used to be when
`ImmutableArray` was designed. Since they allow us to control our initialization, we can be fast on usage.
With nullability checking, reference type also offer a correctness advantage that structs lack: the compiler 
warns if they're left uninitialized.

However, making it a reference means two allocations per instance instead of one. This means using ValueArray<T>
creates more GC pressure, leading to more frequent GC. However, this is true of every collection type outside of
plain arrays.

Overall, making it a reference type is the better solution.