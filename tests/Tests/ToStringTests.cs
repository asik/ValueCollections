using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueCollections;
using Xunit;

namespace Tests;
public class ToStringTests
{
    [Fact]
    void Empty() =>
        Assert.Equal(
            "Block(0) { }",
            Block<int>.Empty.ToString());
}
