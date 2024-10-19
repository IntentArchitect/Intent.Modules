using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Templates;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// Abstract representation of various C# Types like Arrays, Generics, Tuple, Named Identifiers, etc.
/// </summary>
public abstract class CSharpType : ICSharpType
{
    // Task related
    internal const string TaskShortTypeName = "Task";
    internal const string TaskFullTypeName = "System.Threading.Tasks.Task";
    internal const string ValueTaskShortTypeName = "ValueTask";
    internal const string ValueTaskFullTypeName = "System.Threading.Tasks.ValueTask";
    internal const string IAsyncEnumerableFullTypeName = "System.Collections.Generic.IAsyncEnumerable";
    internal const string IAsyncEnumerableShortTypeName = "IAsyncEnumerable";

    // ICollection/IList related
    internal const string ICollectionShortTypeName = "ICollection";
    internal const string ICollectionFullTypeName = "System.Collections.Generic.ICollection";
    internal const string ArrayListShortTypeName = "ArrayList";
    internal const string ArrayListFullTypeName = "System.Collections.ArrayList";
    internal const string ListShortTypeName = "List";
    internal const string ListFullTypeName = "System.Collections.Generic.List";
    internal const string QueueShortTypeName = "Queue";
    internal const string QueueFullTypeName = "System.Collections.Queue";
    internal const string ConcurrentQueueShortTypeName = "ConcurrentQueue";
    internal const string ConcurrentQueueFullTypeName = "System.Collections.Concurrent.ConcurrentQueue";
    internal const string StackShortTypeName = "Stack";
    internal const string StackFullTypeName = "System.Collections.NonGeneric.Stack";
    internal const string ConcurrentStackShortTypeName = "ConcurrentStack";
    internal const string ConcurrentStackFullTypeName = "System.Collections.Concurrent.ConcurrentStack";
    internal const string LinkedListShortTypeName = "LinkedList";
    internal const string LinkedListFullTypeName = "System.Collections.Generic.LinkedList";

    // IDictionary related
    internal const string HashtableShortTypeName = "Hashtable";
    internal const string HashtableFullTypeName = "System.Collections.Hashtable";
    internal const string SortedListShortTypeName = "SortedList";
    internal const string SortedListFullTypeName = "System.Collections.SortedList";
    internal const string GenericSortedListShortTypeName = "SortedList";
    internal const string GenericSortedListFullTypeName = "System.Collections.Generic.SortedList";
    internal const string DictionaryShortTypeName = "Dictionary";
    internal const string DictionaryFullTypeName = "System.Collections.Generic.Dictionary";
    internal const string ConcurrentDictionaryShortTypeName = "ConcurrentDictionary";
    internal const string ConcurrentDictionaryFullTypeName = "System.Collections.Concurrent.ConcurrentDictionary";

    /// <summary>
    /// Creates a type-safe type that represents <see cref="System.Threading.Tasks.Task"/>.
    /// </summary>
    public static CSharpTypeName CreateTask(ICSharpTemplate? template)
    {
        return new CSharpTypeName(template?.UseType(TaskFullTypeName) ?? TaskFullTypeName);
    }

    /// <summary>
    /// Creates a type-safe type that represents <see cref="System.Threading.Tasks.Task&lt;T&gt;"/>.
    /// </summary>
    public static CSharpTypeGeneric CreateTask(CSharpType genericParamType, ICSharpTemplate? template)
    {
        return new CSharpTypeGeneric(template?.UseType(TaskFullTypeName) ?? TaskFullTypeName, [genericParamType]);
    }

    /// <summary>
    /// Creates a type-safe type that represents <see cref="System.Threading.Tasks.ValueTask"/>.
    /// </summary>
    public static CSharpTypeName CreateValueTask(ICSharpTemplate? template)
    {
        return new CSharpTypeName(template?.UseType(ValueTaskFullTypeName) ?? ValueTaskFullTypeName);
    }

    /// <summary>
    /// Creates a type-safe type that represents <see cref="System.Threading.Tasks.ValueTask&lt;T&gt;"/>.
    /// </summary>
    public static CSharpTypeGeneric CreateValueTask(CSharpType genericParamType, ICSharpTemplate? template)
    {
        return new CSharpTypeGeneric(template?.UseType(ValueTaskFullTypeName) ?? ValueTaskFullTypeName, [genericParamType]);
    }

    /// <summary>
    /// Creates a type-safe type that represents void.
    /// </summary>
    public static CSharpTypeVoid CreateVoid()
    {
        return CSharpTypeVoid.DefaultInstance;
    }

    /// <summary>
    /// Is the current type representing a <see cref="Task"/>, a <see cref="Task{T}"/> or <see cref="IAsyncEnumerable{T}"/>?
    /// </summary>
    public bool IsTask()
    {
        return (this is CSharpTypeName name && name.IsTask()) || (this is CSharpTypeGeneric generic && (generic.IsTask() || generic.TypeName is IAsyncEnumerableFullTypeName or IAsyncEnumerableShortTypeName));
    }

    /// <summary>
    /// Is the current type representing a <see cref="System.Threading.Tasks.ValueTask"/> or a <see cref="System.Threading.Tasks.ValueTask&lt;T&gt;"/>?
    /// </summary>
    public bool IsValueTask()
    {
        return (this is CSharpTypeName name && name.IsValueTask()) || (this is CSharpTypeGeneric generic && generic.IsValueTask());
    }

    /// <summary>
    /// Takes the current type and wraps it inside the generic type <see cref="System.Threading.Tasks.Task&lt;T&gt;"/>.
    /// </summary>
    public CSharpType WrapInTask(ICSharpTemplate template)
    {
        if (this is CSharpTypeVoid)
        {
            return CSharpType.CreateTask(template);
        }

        return CSharpType.CreateTask(this, template);
    }

    /// <summary>
    /// Takes the current type and wraps it inside the generic type <see cref="System.Threading.Tasks.ValueTask&lt;T&gt;"/>.
    /// </summary>
    public CSharpType WrapInValueTask(ICSharpTemplate template)
    {
        if (this is CSharpTypeVoid)
        {
            return CSharpType.CreateValueTask(template);
        }

        return CSharpType.CreateValueTask(this, template);
    }

    /// <summary>
    /// If the current type is a <see cref="System.Threading.Tasks.Task&lt;T&gt;"/>, it will extract the generic parameter type.
    /// </summary>
    public CSharpType? GetTaskGenericType()
    {
        if (this.IsTask() && this is CSharpTypeGeneric generic)
        {
            return generic.TypeArgumentList.FirstOrDefault();
        }
        return null;
    }

    /// <summary>
    /// If the current type is a <see cref="System.Threading.Tasks.Task&lt;T&gt;"/>, it will extract the generic parameter type.
    /// </summary>
    public CSharpType? GetValueTaskGenericType()
    {
        if (this.IsValueTask() && this is CSharpTypeGeneric generic)
        {
            return generic.TypeArgumentList.FirstOrDefault();
        }
        return null;
    }

    public CSharpTypeName GetCollectionImplementationType()
    {
        var originalType = string.Empty;

        if (this is CSharpTypeNullable nullableType)
        {
            if (nullableType.ElementType is CSharpTypeGeneric nullableGeneric)
            {
                originalType = nullableGeneric.TypeName;
            }
        }

        if (this is CSharpTypeGeneric genericType)
        {
            originalType = genericType.TypeName;
        }

        if (this is CSharpTypeName nameType)
        {
            originalType = nameType.TypeName;
        }

        return new CSharpTypeName(MapCollectionToImplementationType(originalType));
    }

    private static string MapCollectionToImplementationType(string typeName) => typeName switch
    {
        ICollectionShortTypeName or ICollectionFullTypeName => ListShortTypeName,
        ArrayListShortTypeName or ArrayListFullTypeName => ArrayListFullTypeName,
        ListShortTypeName or ListFullTypeName => ListFullTypeName,
        QueueShortTypeName or QueueFullTypeName => QueueFullTypeName,
        ConcurrentQueueShortTypeName or ConcurrentQueueFullTypeName => ConcurrentQueueFullTypeName,
        StackShortTypeName or StackFullTypeName => StackFullTypeName,
        ConcurrentStackShortTypeName or ConcurrentStackFullTypeName => ConcurrentStackFullTypeName,
        LinkedListShortTypeName or LinkedListFullTypeName => LinkedListFullTypeName,
        HashtableShortTypeName or HashtableFullTypeName => HashtableFullTypeName,
        SortedListShortTypeName or SortedListFullTypeName => SortedListFullTypeName,
        GenericSortedListFullTypeName => GenericSortedListFullTypeName,
        DictionaryShortTypeName or DictionaryFullTypeName => DictionaryFullTypeName,
        ConcurrentDictionaryShortTypeName or ConcurrentDictionaryFullTypeName => ConcurrentDictionaryFullTypeName,
        _ => "default"
    };

    #region ICSharpType implementation

    ICSharpType ICSharpType.WrapInTask(ICSharpTemplate template) => WrapInTask(template);
    ICSharpType ICSharpType.WrapInValueTask(ICSharpTemplate template) => WrapInValueTask(template);
    ICSharpType? ICSharpType.GetTaskGenericType() => GetTaskGenericType();
    ICSharpType? ICSharpType.GetValueTaskGenericType() => GetValueTaskGenericType();

    #endregion
}