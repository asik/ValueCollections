using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class LinqTests
{
    readonly Block<int> _block = Block.CreateRange(Enumerable.Range(0, 8));

    [Fact]
    void ToBlock() => 
        Assert.Equal(
            _block,
            Enumerable.Range(0, 8).ToBlock());

    [Fact]
    void Select() =>
        Assert.Equal(
            _block.Select(x => x + 1),
            Enumerable.Range(1, 8));

    [Fact]
    void Where() =>
        Assert.Equal(
            _block.Where(x => x % 2 == 0),
            new[] { 0, 2, 4, 6 });

    [Fact]
    void Count() => 
        Assert.Equal(_block.Count, _block.Count());

    [Fact]
    [SuppressMessage("Assertions", "xUnit2013:Do not use equality check to check for collection size.")]
    void CountEmpty() =>
        Assert.Equal(0, Block<int>.Empty.Count);

    // Many more to add...
}
