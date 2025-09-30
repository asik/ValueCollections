using System;
using System.Collections.Generic;

namespace ValueCollections;

public partial class Block<T> : IEquatable<Block<T>>
{
    static readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;

    /// <inheritdoc />
    public bool Equals(Block<T> other)
    {
        // Equality via IStructuralEquatable appears to box every single element.
        // We can do 57X faster and avoid all allocations like this.
        // Note that _arr.SequenceEquals(other._arr) is a very similar implementation to this but seemed about 30% slower.
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        var otherArray = other._arr;
        if (otherArray.Length != _arr.Length)
        {
            return false;
        }

        // Doing two optimizations here, both result in measurably faster code:
        // We cache EqualityComparer<T>.Default in our own static field
        // We copy that static field into a local here because local variable access is faster
        var comparer = _comparer;
        for (var i = 0; i < _arr.Length; ++i)
        {
            if (!comparer.Equals(_arr[i], otherArray[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) =>
        obj is Block<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // This should be an ok implementation.
        // HashCode source can be viewed here https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/HashCode.cs
        // (not sure where the current implementation is, seems hard to find)
        // It's recommended by IDE0070.
        // It does not allocate. It's based on a really fast algorithm.
        // Its hashes are not cryptographic in quality but that's not the purpose of GetHashCode.
        // Its hashes are randomized across two runs of the same program and should not be stored outside of the program.
        // This is to defend against a known attack. See https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        // Also see https://docs.microsoft.com/en-us/archive/blogs/ericlippert/guidelines-and-rules-for-gethashcode

        // Other ideas:
        // - IStructuralEquatable.GetHashCode() only looks at the last 8 elements, so it's basically useless.
        // - Cryptographic hashes like MD5, SHA256: presumably not worth much slower performance.
        // - Copy-pasting some bitwise-operations-based algorithm I don't understand from Stackoverflow or a blog: just no.

        var hashCode = new HashCode();
        for (var i = 0; i < _arr.Length; ++i)
        {
            hashCode.Add(_arr[i]);
        }
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(Block<T> left, Block<T> right) =>
        left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(Block<T> left, Block<T> right) =>
        !left.Equals(right);
}
