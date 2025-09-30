namespace ValueCollections.Unsafe;

/// <summary>
/// An unsafe class that provides a set of methods to access the underlying data representations of value collections.
/// </summary>
public static class ValueCollectionsMarshal
{
    /// <summary>
    /// Gets a <see cref="Block{T}"/> value wrapping the input <typeparamref name="T"/> array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input array.</typeparam>
    /// <param name="array">The input array to wrap in the returned <see cref="Block{T}"/> value.</param>
    /// <returns>An <see cref="Block{T}"/> value wrapping <paramref name="array"/>.</returns>
    /// <remarks>
    /// <para>
    /// When using this method, callers should take extra care to ensure that they're the sole owners of the input
    /// array, and that it won't be modified once the returned <see cref="Block{T}"/> value starts being
    /// used. Doing so might cause undefined behavior in code paths which don't expect the contents of a given
    /// <see cref="Block{T}"/> values to change after its creation.
    /// </para>
    /// </remarks>
    public static Block<T> AsBlock<T>(T[] array) =>
        new(array);


    /// <summary>
    /// Gets the underlying <typeparamref name="T"/> array for an input <see cref="Block{T}"/> value.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input <see cref="Block{T}"/> value.</typeparam>
    /// <param name="block">The input <see cref="Block{T}"/> value to get the underlying <typeparamref name="T"/> array from.</param>
    /// <returns>The underlying <typeparamref name="T"/> array for <paramref name="block"/>, if present.</returns>
    /// <remarks>
    /// <para>
    /// When using this method, callers should make sure to not pass the resulting underlying array to methods that
    /// might mutate it. Doing so might cause undefined behavior in code paths using <paramref name="block"/> which
    /// don't expect the contents of the <see cref="Block{T}"/> value to change.
    /// </para>
    /// </remarks>
    public static T[] AsArray<T>(Block<T> block) =>
        block.UnsafeInternalArray;
}
