

namespace System.Runtime.CompilerServices
{
    // If we were targeting .NET 9+, we could rely on .NET to provide it,
    // but since we're targeting .NET Standard 2.0, we have to provide it ourselves.
    // NOT public of course
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
#pragma warning disable CS9113 // Parameter is unread.
    sealed class CollectionBuilderAttribute(Type builderType, string methodName) : Attribute { }
#pragma warning restore CS9113 // Parameter is unread.
}