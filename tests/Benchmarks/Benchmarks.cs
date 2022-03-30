using BenchmarkDotNet.Attributes;
using ValueCollections;

namespace Benchmarks;

[MemoryDiagnoser]
public class BlockBenchmarks
{
    readonly Block<int> block = new(1, 2, 3);
    readonly Block<int> blockCopy = new(1, 2, 3);
    readonly List<int> sourceList = new() { 1, 2, 3 };
    readonly int[] sourceArray = new[] { 1, 2, 3 };

    [Benchmark]
    public Block<int> ConstructorFromArray() => 
        new(sourceArray);

    [Benchmark]
    public Block<int> ConstructorFromList() => 
        new(sourceList);

    [Benchmark]
    public bool EqualityOnInts() => 
        block.Equals(blockCopy);

    [Benchmark]
    public int GetHashCodeOnInts() =>
        block.GetHashCode();

    [Benchmark]
    public Block<int> CreateFromArray() =>
        Block.Create(sourceArray);

    [Benchmark]
    public Block<int> CreateRange() =>
        Block.CreateRange(sourceList);

/* Latest results:

|               Method |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
|--------------------- |----------:|----------:|----------:|-------:|----------:|
| ConstructorFromArray |  13.70 ns |  0.616 ns |  1.746 ns | 0.0064 |      40 B |
|  ConstructorFromList |  34.77 ns |  1.002 ns |  2.776 ns | 0.0063 |      40 B |
|       EqualityOnInts | 368.41 ns | 11.110 ns | 30.414 ns | 0.0267 |     168 B |
|    GetHashCodeOnInts | 192.91 ns |  6.532 ns | 18.950 ns | 0.0114 |      72 B |
|      CreateFromArray |  13.38 ns |  0.450 ns |  1.298 ns | 0.0064 |      40 B |
|          CreateRange |  36.77 ns |  1.263 ns |  3.622 ns | 0.0063 |      40 B |

Might be possible to reduce allocations on equality.
*/
}
