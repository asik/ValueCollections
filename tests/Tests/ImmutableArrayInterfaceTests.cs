﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using ValueCollections;
using Xunit;

namespace Tests;

public class ImmutableArrayInterfaceTests
{
    private readonly Block<int> defaultInstance = new();
    private readonly Block<int> emptyInstance = Block<int>.Empty;
    private readonly Block<string> nonEmptyBlock = Block.Create("a", "b", "c");
    private readonly Block<string> emptyBlock = Block<string>.Empty;

    [Fact]
    private void LengthDefault()
    {
        Assert.Equal(0, defaultInstance.Length);
    }

    [Fact]
    private void LengthEmpty()
    {
        Assert.Equal(0, emptyInstance.Length);
    }

    [Fact]
    private void EmptyIsEmpty()
    {
        Assert.Equal(Array.Empty<int>(), Block<int>.Empty);
    }

    [Fact]
    private void EmptyIsSingleton()
    {
        Assert.Same(Block<int>.Empty, Block<int>.Empty);
    }

    [Fact]
    private void FindIndexi_NotFound()
    {
        var source = Block.Create("a", "b", "b", "c");
        var expected = source.Select((item, i) => (item, i));
        var actual = new List<(string, int)>();
        var idx = source.FindIndex((item, i) =>
        {
            actual.Add((item, i));
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    [Fact]
    private void FindIndexi_Found()
    {
        var source = Block.Create("a", "b", "b", "c");
        var expected = new[] { ("a", 0), ("b", 1) };
        var actual = new List<(string, int)>();
        var idx = source.FindIndex((item, i) =>
        {
            actual.Add((item, i));
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(1, idx);
    }

    [Fact]
    private void FindLastIndexi_NotFound()
    {
        var source = Block.Create("a", "b", "b", "c");
        var expected = new[] { ("c", 3), ("b", 2), ("b", 1), ("a", 0) };
        var actual = new List<(string, int)>();
        var idx = source.FindLastIndex((item, i) =>
        {
            actual.Add((item, i));
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    [Fact]
    private void FindLastIndexi_Found()
    {
        var source = Block.Create("a", "b", "b", "c");
        var expected = new[] { ("c", 3), ("b", 2) };
        var actual = new List<(string, int)>();
        var idx = source.FindLastIndex((item, i) =>
        {
            actual.Add((item, i));
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(2, idx);
    }

    [Fact]
    private void FindIndex_NotFound()
    {
        var source = Block.Create("a", "b", "b", "c");
        var expected = source.ToArray();
        var actual = new List<string>();
        var idx = source.FindIndex(item =>
        {
            actual.Add(item);
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    private record A();

    private record B() : A;

    [Fact]
    private void FindIndex_Found()
    {
        var source = Block.Create("a", "b", "b", "c");

        var s = Block<A>.Empty;
        var b = Block.Create("Hello");
        ((IImmutableList<A>)s).AddRange(new[] { new B() });
        var expected = new[] { "a", "b" };
        var actual = new List<string>();
        var idx = source.FindIndex(item =>
        {
            actual.Add(item);
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(1, idx);
    }

    [Fact]
    private void FindLastIndex_NotFound()
    {
        var source = Block.Create("a", "b", "b", "c");
        var expected = new[] { "c", "b", "b", "a" };
        var actual = new List<string>();
        var idx = source.FindLastIndex(item =>
        {
            actual.Add(item);
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    [Fact]
    private void FindLastIndex_Found()
    {
        var source = Block.Create("a", "b", "b", "c");
        var expected = new[] { "c", "b" };
        var actual = new List<string>();
        var idx = source.FindLastIndex(item =>
        {
            actual.Add(item);
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(2, idx);
    }
}
