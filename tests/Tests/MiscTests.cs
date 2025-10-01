using System;
using System.Collections.Generic;
using System.Text;
using ValueCollections;
using Xunit;

namespace Tests;

public class MiscellaneousTests
{
    [Fact]
    void AsReadOnlySpanReturnsSameItemsAndLength()
    {
        var valueArray = ValueArray.Create(1, 2, 3);
        var span = valueArray.AsReadOnlySpan();
        Assert.Equal(valueArray, span.ToArray());
    }

    [Fact]
    void AsReadOnlySpanReturnsItemsStartingAtSpecifiedPosition()
    {
        var valueArray = ValueArray.Create(0, 1, 2, 3, 4, 5, 6);
        var span = valueArray.AsReadOnlySpan(4);
        Assert.Equal([4, 5, 6], span);
    }

    [Fact]
    void AsReadOnlySpanReturnsItemsStartingAtSpecifiedPositionAndEndingAfterSpecifiedCount()
    {
        var valueArray = ValueArray.Create(0, 1, 2, 3, 4, 5, 6);
        var span = valueArray.AsReadOnlySpan(2, 3);
        Assert.Equal([2, 3, 4], span);
    }

    [Fact]
    void AsReadOnlySpanWithStartOutOfBoundsThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ValueArray.Create(0, 1, 2).AsReadOnlySpan(4));

    [Fact]
    void AsReadOnlySpanWithStartAtTheEndReturnsEmpty() =>
        Assert.Empty(ValueArray.Create(0, 1, 2).AsReadOnlySpan(3).ToArray());

    [Fact]
    void AsReadOnlySpanWithLengthOutOfBoundsThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ValueArray.Create(0, 1, 2).AsReadOnlySpan(1, 3));
}
