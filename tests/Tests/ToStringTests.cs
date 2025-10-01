using System.Collections.Generic;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;
public class ToStringTests
{
    [Fact]
    void Empty() => Assert.Equal(
        "ValueArray(0) { }",
        ValueArray<int>.Empty.ToString());

    record MyRecord(string A, int B);

    [Fact]
    void ValueArrayOfRecords() => Assert.Equal(
        "ValueArray(2) { MyRecord { A = A, B = 3 }, MyRecord { A = ABC, B = 11 } }",
        ValueArray.Create(new MyRecord("A", 3), new MyRecord("ABC", 11)).ToString());

    record ValueArraysRecord(ValueArray<int> Ints);

    [Fact]
    void RecordOfValueArrays() => Assert.Equal(
        "ValueArraysRecord { Ints = ValueArray(2) { 2, 3 } }",
        new ValueArraysRecord(ValueArray.Create(2, 3)).ToString());

    [Fact]
    void ValueArrayOfArrays() => Assert.Equal(
        "ValueArray(1) { Array(2) { 6, 7 } }",
        ValueArray.Create<int[]>(new[] { 6, 7 }).ToString());

    [Fact]
    void ValueArrayOfListsOfDictionaries() => Assert.Equal(
        "ValueArray(1) { List(1) { Dictionary(1) { [abc, 345] } } }",
        ValueArray.Create(
            new List<Dictionary<string, int>>
            {
                new Dictionary<string, int> { ["abc"] = 345 }
            }).ToString());

    [Fact]
    void CutOffInnerEnumerable() => Assert.Equal(
        "ValueArray(1) { RangeIterator { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, ... } }",
        ValueArray.Create(Enumerable.Range(0, 100)).ToString());

    [Fact]
    void CutOffInnerArray() => Assert.Equal(
        "ValueArray(1) { Array(100) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, ... } }",
        ValueArray.Create<int[]>(Enumerable.Range(0, 100).ToArray()).ToString());

    [Fact]
    void CutOff() => Assert.Equal(
        "ValueArray(100) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, ... }",
        ValueArray.CreateRange(Enumerable.Range(0, 100)).ToString());

    enum MyEnum { Case1, Case2 }

    [Fact]
    void Enums() => Assert.Equal(
        "ValueArray(2) { Case1, Case2 }",
        ValueArray.Create(MyEnum.Case1, MyEnum.Case2).ToString());


    [Fact]
    void TupleOfValueArrays() => Assert.Equal(
        "(ValueArray(0) { }, ValueArray(1) { 2 })",
        (ValueArray<int>.Empty, ValueArray.Create(2)).ToString());

    [Fact]
    void ValueArrayOfTuples() => Assert.Equal(
        "ValueArray(2) { (1, hey), (3, jude) }",
        ValueArray.Create((a: 1, b: "hey"), (a: 3, b: "jude")).ToString());

    [Fact]
    void HeterogenousValueArrays() => Assert.Equal(
        "ValueArray(3) { hey, Array(1) { 3 }, Case2 }",
        ValueArray.Create<object>("hey", new[] { 3 }, MyEnum.Case2).ToString());
}
