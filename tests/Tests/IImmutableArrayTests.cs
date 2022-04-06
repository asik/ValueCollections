using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueCollections;
using Xunit;

namespace Tests;
public class IImmutableArrayTests
{
    static readonly Block<string> nonEmptyBlock = Block.Create("a", "b", "c");
    static readonly Block<string> emptyBlock = Block<string>.Empty;
    static readonly IImmutableList<string> nonEmptyBlockAsImmutableList = nonEmptyBlock;
    static readonly IImmutableList<string> emptyBlockAsImmutableList = emptyBlock;

    [Fact]
    void Clear()
    {
        Assert.Same(Block<string>.Empty, nonEmptyBlockAsImmutableList.Clear());
        Assert.Same(Block<string>.Empty, emptyBlockAsImmutableList.Clear());
    }
}
