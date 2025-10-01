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

## Should we expose constructors?

1. We don't want to allow this syntax:
```
new ValueArray() { 1, 2, 3 }
```

We do want a method `Add` like `ImmutableArray` and `List`.

We don't have a default constructor, but it'd be probably less confusing to have no constructors altogether so people are less likely to be tempted to use that syntax that can't ever be supported.

## Should we expose a ValueArray.Builder class like ImmutableArray>?

