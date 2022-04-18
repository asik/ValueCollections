using System.Collections.Generic;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;
public class ToStringTests
{
    [Fact]
    void Empty() => Assert.Equal(
        "Block(0) { }",
        Block<int>.Empty.ToString());

    record MyRecord(string A, int B);

    [Fact]
    void BlockOfRecords() => Assert.Equal(
        "Block(2) { MyRecord { A = A, B = 3 }, MyRecord { A = ABC, B = 11 } }",
        Block.Create(new MyRecord("A", 3), new MyRecord("ABC", 11)).ToString());

    record BlocksRecord(Block<int> Ints);

    [Fact]
    void RecordOfBlocks() => Assert.Equal(
        "BlocksRecord { Ints = Block(2) { 2, 3 } }",
        new BlocksRecord(Block.Create(2, 3)).ToString());

    [Fact]
    void BlockOfArrays() => Assert.Equal(
        "Block(1) { Array(2) { 6, 7 } }",
        Block.Create<int[]>(new[] { 6, 7 }).ToString());

    [Fact]
    void BlockOfListsOfDictionaries() => Assert.Equal(
        "Block(1) { List(1) { Dictionary(1) { [abc, 345] } } }",
        Block.Create(
            new List<Dictionary<string, int>>
            {
                new Dictionary<string, int> { ["abc"] = 345 }
            }).ToString());

    [Fact]
    void CutOffInnerEnumerable() => Assert.Equal(
        "Block(1) { RangeIterator { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, ... } }",
        Block.Create(Enumerable.Range(0, 100)).ToString());

    [Fact]
    void CutOffInnerArray() => Assert.Equal(
        "Block(1) { Array(100) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, ... } }",
        Block.Create<int[]>(Enumerable.Range(0, 100).ToArray()).ToString());

    [Fact]
    void CutOff() => Assert.Equal(
        "Block(100) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, ... }",
        Block.CreateRange(Enumerable.Range(0, 100)).ToString());

    enum MyEnum { Case1, Case2 }

    [Fact]
    void Enums() => Assert.Equal(
        "Block(2) { Case1, Case2 }",
        Block.Create(MyEnum.Case1, MyEnum.Case2).ToString());


    [Fact]
    void TupleOfBlocks() => Assert.Equal(
        "(Block(0) { }, Block(1) { 2 })",
        (Block<int>.Empty, Block.Create(2)).ToString());

    [Fact]
    void BlockOfTuples() => Assert.Equal(
        "Block(2) { (1, hey), (3, jude) }",
        Block.Create((a: 1, b: "hey"), (a: 3, b: "jude")).ToString());

    [Fact]
    void HeterogenousBlock() => Assert.Equal(
        "Block(3) { hey, Array(1) { 3 }, Case2 }",
        Block.Create<object>("hey", new[] { 3 }, MyEnum.Case2).ToString());
}
