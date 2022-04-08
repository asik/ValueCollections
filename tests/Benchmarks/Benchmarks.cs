using BenchmarkDotNet.Attributes;
using ValueCollections;

namespace Benchmarks;

[MemoryDiagnoser]
public class BlockBenchmarks
{
    const int largeSize = 100000;
    readonly Block<int> largeIntBlock = Enumerable.Range(0, largeSize).ToBlock();
    readonly Block<int> largeIntBlockCopy = Enumerable.Range(0, largeSize).ToBlock();
    readonly int[] largeIntArray = Enumerable.Range(0, largeSize).ToArray();
    readonly int[] largeIntArrayCopy = Enumerable.Range(0, largeSize).ToArray();

    readonly Block<string> largeStringBlock = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToBlock();
    readonly Block<string> largeStringBlockCopy = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToBlock();
    readonly string[] largeStringArray = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToArray();
    readonly string[] largeStringArrayCopy = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToArray();


    readonly Block<int> blockCopy = new(1, 2, 3);
    readonly List<int> sourceList = new() { 1, 2, 3 };
    readonly int[] sourceArray = new[] { 1, 2, 3 };

    //[Benchmark]
    //public Block<int> ConstructorFromArray() => 
    //    new(sourceArray);

    //[Benchmark]
    //public Block<int> ConstructorFromList() => 
    //    new(sourceList);

    //[Benchmark]
    //public bool LargeIntBlockEquality() =>
    //    largeIntBlock.Equals(largeIntBlockCopy);

    //[Benchmark]
    //public bool LargeIntArrayEquality() =>
    //    largeIntArray.SequenceEqual(largeIntArrayCopy);

    //[Benchmark]
    //public bool LargeStringBlockEquality() =>
    //    largeStringBlock.Equals(largeStringBlockCopy);

    //[Benchmark]
    //public bool LargeStringSequenceEquals() =>
    //    largeStringArray.SequenceEqual(largeStringArrayCopy);

    //[Benchmark]
    //public int GetHashCodeOnInts() =>
    //    block.GetHashCode();

    //[Benchmark]
    //public Block<int> CreateFromArray() =>
    //    Block.Create(sourceArray);

    //[Benchmark]
    //public Block<int> CreateRange() =>
    //    Block.CreateRange(sourceList);

    /* 

    BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1586 (20H2/October2020Update)
    Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
    .NET SDK=6.0.201
      [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
      DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


    Via IStructuralEquatable:
    |                Method |        Mean |      Error |     StdDev |    Gen 0 |   Allocated |
    |---------------------- |------------:|-----------:|-----------:|---------:|------------:|
    | LargeIntBlockEquality | 9,006.72 us | 137.606 us | 121.984 us | 750.0000 | 4,800,032 B |
    | LargeIntArrayEquality |    16.53 us |   0.285 us |   0.484 us |        - |           - |

    Via directly calling EqualityComparer<T>.Default.Compare:
    |                Method |     Mean |   Error |  StdDev | Allocated |
    |---------------------- |---------:|--------:|--------:|----------:|
    | LargeIntBlockEquality | 157.9 us | 2.46 us | 2.31 us |         - |

    Via SequenceEquals on the underlying ImmutableArrays:
    |                Method |     Mean |   Error |  StdDev | Allocated |
    |---------------------- |---------:|--------:|--------:|----------:|
    | LargeIntBlockEquality | 208.7 us | 1.85 us | 1.64 us |         - |

    Via rewriting the type to be based on T[] instead of ImmutableArray<T>
    |                Method |     Mean |    Error |   StdDev | Allocated |
    |---------------------- |---------:|---------:|---------:|----------:|
    | LargeIntBlockEquality | 16.03 us | 0.429 us | 1.266 us |         - |

    The story is different for non-blittable types though. Strings:
    |                   Method |     Mean |    Error |   StdDev | Allocated |
    |------------------------- |---------:|---------:|---------:|----------:|
    | LargeStringBlockEquality | 790.8 us |  7.27 us |  5.67 us |         - |
    | LargeStringArrayEquality | 895.4 us | 16.63 us | 33.21 us |         - |

    So basically Array equality has an optimization for blittable types where it is implemented as a memcmp, and
    this makes it 10 times faster in that case. However, to support this, we'd have to implement Block<T> on top
    of T[] directly which is a lot more work. As it stands, Block<T> equality is generally on par or better than
    SequenceEquals, so other things will take priority over a total rewrite to get that memcmp optimization.
    */
}
