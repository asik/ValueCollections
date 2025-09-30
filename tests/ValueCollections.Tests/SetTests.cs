using Xunit;

namespace ValueCollections.Tests;
public class SetTests
{
    [Fact]
    void SetUnion() => Assert.Equal(
        Set.Create(1, 2, 3, 4),
        Set.Create(1, 2, 3) | Set.Create(2, 3, 4));

    [Fact]
    void SetUnionWithIEnumerable() => Assert.Equal(
        Set.Create(1, 2, 3, 4),
        Set.Create(1, 2, 3) | new[] { 2, 3, 4 });

    [Fact]
    void SetUnionWithIEnumerableLeft() => Assert.Equal(
        Set.Create(1, 2, 3, 4),
        new[] { 1, 2, 3 } | Set.Create(2, 3, 4));

    [Fact]
    void SetIntersection() => Assert.Equal(
        Set.Create(2, 3),
        Set.Create(1, 2, 3) & Set.Create(2, 3, 4));

    [Fact]
    void SetDifference() => Assert.Equal(
        Set.Create(1),
        Set.Create(1, 2, 3) - Set.Create(2, 3, 4));

    [Fact]
    void SetSymmetricDifference() => Assert.Equal(
        Set.Create(1, 4),
        Set.Create(1, 2, 3) ^ Set.Create(2, 3, 4));
}