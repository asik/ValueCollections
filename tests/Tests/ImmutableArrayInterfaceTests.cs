using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ValueCollections;
using Xunit;

namespace Tests;

public class ImmutableArrayInterfaceTests
{
    readonly Block<int> defaultInstance = new();
    readonly Block<int> emptyInstance = Block<int>.Empty;

    [Fact]
    void LengthDefault() =>
        // We inherit this behavior from ImmutableArray. It's not nice, we could easily change it to return 0.
        // Maybe ImmutableArray has a good reason to do this? CLR inlining? Consistency?
        Assert.Throws<NullReferenceException>(() => defaultInstance.Length);

    [Fact]
    void LengthEmpty() =>
        Assert.Equal(0, emptyInstance.Length);

    [Fact]
    void IsDefault()
    {
        Assert.True(defaultInstance.IsDefault);
        Assert.False(emptyInstance.IsDefault);
        Assert.False(Block.Create(1, 2).IsDefault);
    }

    [Fact]
    void IsDefaultOrEmpty()
    {
        Assert.True(defaultInstance.IsDefaultOrEmpty);
        Assert.True(emptyInstance.IsDefaultOrEmpty);
        Assert.False(Block.Create(1, 2).IsDefaultOrEmpty);
    }

    [Fact]
    void IsEmpty()
    {
        // Again, we could change this, see rationale above.
        Assert.Throws<NullReferenceException>(() => defaultInstance.IsEmpty);
        Assert.True(emptyInstance.IsEmpty);
        Assert.False(Block.Create(1, 2).IsEmpty);
    }

    [Fact]
    void Empty() =>
        Assert.Equal(Array.Empty<int>(), Block<int>.Empty);
}
