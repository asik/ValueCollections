using BenchmarkDotNet.Attributes;
using ValueCollections;

namespace Benchmarks;

[MemoryDiagnoser]
public class ValueArrayBenchmarks
{
    private const int largeSize = 100000;
    private readonly ValueArray<int> largeIntValueArray = Enumerable.Range(0, largeSize).ToValueArray();
    private readonly ValueArray<int> largeIntValueArrayCopy = Enumerable.Range(0, largeSize).ToValueArray();
    private readonly int[] largeIntArray = Enumerable.Range(0, largeSize).ToArray();
    private readonly int[] largeIntArrayCopy = Enumerable.Range(0, largeSize).ToArray();
    private readonly ValueArray<string> largeStringValueArray = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToValueArray();
    private readonly ValueArray<string> largeStringValueArrayCopy = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToValueArray();
    private readonly string[] largeStringArray = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToArray();
    private readonly string[] largeStringArrayCopy = Enumerable.Range(0, largeSize).Select(i => i.ToString()).ToArray();
    private readonly ValueArray<int> ValueArrayCopy = ValueArray.Create(1, 2, 3);
    private readonly List<int> sourceList = new() { 1, 2, 3 };
    private readonly int[] sourceArray = new[] { 1, 2, 3 };

    //[Benchmark]
    //public ValueArray<int> ConstructorFromArray() => 
    //    new(sourceArray);

    //[Benchmark]
    //public ValueArray<int> ConstructorFromList() => 
    //    new(sourceList);

    //[Benchmark]
    //public bool LargeIntValueArrayEquality() =>
    //    largeIntValueArray.Equals(largeIntValueArrayCopy);

    //[Benchmark]
    //public bool LargeIntArrayEquality() =>
    //    largeIntArray.SequenceEqual(largeIntArrayCopy);

    //[Benchmark]
    //public bool LargeStringValueArrayEquality() =>
    //    largeStringValueArray.Equals(largeStringValueArrayCopy);

    //[Benchmark]
    //public bool LargeStringSequenceEquals() =>
    //    largeStringArray.SequenceEqual(largeStringArrayCopy);

    //[Benchmark]
    //public int GetHashCodeOnInts() =>
    //    valueArray.GetHashCode();

    //[Benchmark]
    //public ValueArray<int> CreateFromArray() =>
    //    ValueArray.Create(sourceArray);

    //[Benchmark]
    //public ValueArray<int> CreateRange() =>
    //    ValueArray.CreateRange(sourceList);

    [Benchmark]
    public void ForEachIteration()
    {
        int sum = 0;
        foreach(var _item in largeIntValueArray)
        {
            sum += _item;
        }
        bleh = sum;
    }

    static int bleh = 0;

    /* 

    BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1586 (20H2/October2020Update)
    Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
    .NET SDK=6.0.201
      [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
      DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


    Via IStructuralEquatable:
    |                Method |        Mean |      Error |     StdDev |    Gen 0 |   Allocated |
    |---------------------- |------------:|-----------:|-----------:|---------:|------------:|
    | LargeIntvalueArrayEquality | 9,006.72 us | 137.606 us | 121.984 us | 750.0000 | 4,800,032 B |
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
    this makes it 10 times faster in that case. However, to support this, we'd have to implement ValueArray<T> on top
    of T[] directly which is a lot more work. As it stands, ValueArray<T> equality is generally on par or better than
    SequenceEquals, so other things will take priority over a total rewrite to get that memcmp optimization.

    Some results for HashCode:

|                                Method |             Mean |          Error |         StdDev |            Median | Allocated |
|-------------------------------------- |-----------------:|---------------:|---------------:|------------------:|----------:|
|                          GetHashCode_ | 1,341,724.938 ns | 26,772.8537 ns | 75,073.9520 ns | 1,356,153.9062 ns |       1 B |
| GetHashCodeNoStaticAccessOptimization |   870,717.506 ns | 17,348.2829 ns | 41,230.0299 ns |   883,716.2598 ns |         - |
| GetHashCodeCallingGetHashCodeManually | 1,018,821.900 ns | 26,986.0200 ns | 76,992.6300 ns |                           1 B |
|       GetHashCodeIStructuralEquatable |       201.600 ns |      4.0600 ns |     11.6500 ns |                             - |

    So passing in EqualityComparer<T>.Default slows it down.
    Calling GetHashCode and passing it to HashCode.Add is also slower than letting HashCode do it.
    IStructuralEquatable is fast but it's also cheating, it only looks at the last 8 elements.

    */
}
