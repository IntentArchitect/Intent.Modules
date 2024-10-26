using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

    internal static Dictionary<string, string> CollectionMap = new()
    {
        { "ICollection", "System.Collections.Generic.List" },
        { "System.Collections.Generic.ICollection", "System.Collections.Generic.List" },
        { "ArrayList", "System.Collections.ArrayList" },
        { "System.Collections.ArrayList", "System.Collections.ArrayList" },
        { "List", "System.Collections.Generic.List" },
        { "System.Collections.Generic.List", "System.Collections.Generic.List" },
        { "Queue", "System.Collections.Queue" },
        { "System.Collections.Queue", "System.Collections.Queue" },
        { "ConcurrentQueue", "System.Collections.Concurrent.ConcurrentQueue" },
        { "System.Collections.Concurrent.ConcurrentQueue", "System.Collections.Concurrent.ConcurrentQueue" },
        { "Stack", "System.Collections.NonGeneric.Stack" },
        { "System.Collections.NonGeneric.Stack", "System.Collections.NonGeneric.Stack" },
        { "ConcurrentStack", "System.Collections.Concurrent.ConcurrentStack" },
        { "System.Collections.Concurrent.ConcurrentStack", "System.Collections.Concurrent.ConcurrentStack" },
        { "LinkedList", "System.Collections.Generic.LinkedList" },
        { "System.Collections.Generic.LinkedList", "System.Collections.Generic.LinkedList" },
        { "Hashtable", "System.Collections.Hashtable" },
        { "System.Collections.Hashtable", "System.Collections.Hashtable" },
        { "SortedList", "System.Collections.SortedList" },
        { "System.Collections.SortedList", "System.Collections.SortedList" },
        { "System.Collections.Generic.SortedList", "System.Collections.Generic.SortedList" },
        { "Dictionary", "System.Collections.Generic.Dictionary" },
        { "System.Collections.Generic.Dictionary", "System.Collections.Generic.Dictionary" },
        { "ConcurrentDictionary", "System.Collections.Concurrent.ConcurrentDictionary" },
        { "System.Collections.Concurrent.ConcurrentDictionary", "System.Collections.Concurrent.ConcurrentDictionary" },
        { "IEnumerable", "System.Collections.Generic.List" },
        { "System.Collections.Generic.IEnumerable", "System.Collections.Generic.List" }
    };

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

    /// <summary>
    /// Does the current type representing a collection type (a collection type based on IList, ICollection or IDictionary)?
    /// </summary>
    public bool IsCollectionType()
    {
        return CollectionMap.ContainsKey(GetTypeName());
    }

    /// <summary>
    /// If the current type is a collection type, it will return the valid implementation type, otherwise will return "default"
    /// </summary>
    public ICSharpType GetCollectionImplementationType() => new CSharpTypeName(MapCollectionToImplementationType(GetTypeName()));

    private static string MapCollectionToImplementationType(string typeName) 
    {
        if(CollectionMap.TryGetValue(typeName, out var map))
        {
            return map;
        }

        return "default";
    }

    private string GetTypeName()
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

        return originalType;
    }

    #region ICSharpType implementation

    ICSharpType ICSharpType.WrapInTask(ICSharpTemplate template) => WrapInTask(template);
    ICSharpType ICSharpType.WrapInValueTask(ICSharpTemplate template) => WrapInValueTask(template);
    ICSharpType? ICSharpType.GetTaskGenericType() => GetTaskGenericType();
    ICSharpType? ICSharpType.GetValueTaskGenericType() => GetValueTaskGenericType();

   

    #endregion
}