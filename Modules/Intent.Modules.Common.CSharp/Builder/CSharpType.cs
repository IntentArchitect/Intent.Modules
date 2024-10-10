using System.Collections.Generic;
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
    internal const string TaskShortTypeName = "Task";
    internal const string TaskFullTypeName = "System.Threading.Tasks.Task";
    internal const string ValueTaskShortTypeName = "ValueTask";
    internal const string ValueTaskFullTypeName = "System.Threading.Tasks.ValueTask";
    internal const string IAsyncEnumerableFullTypeName = "System.Collections.Generic.IAsyncEnumerable";
    internal const string IAsyncEnumerableShortTypeName = "IAsyncEnumerable";

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

    #region ICSharpType implementation

    ICSharpType ICSharpType.WrapInTask(ICSharpTemplate template) => WrapInTask(template);
    ICSharpType ICSharpType.WrapInValueTask(ICSharpTemplate template) => WrapInValueTask(template);
    ICSharpType? ICSharpType.GetTaskGenericType() => GetTaskGenericType();
    ICSharpType? ICSharpType.GetValueTaskGenericType() => GetValueTaskGenericType();

    #endregion
}