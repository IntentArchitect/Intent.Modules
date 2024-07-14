using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// Abstract representation of various C# Types like Arrays, Generics, Tuple, Named Identifiers, etc.
/// </summary>
public abstract class CSharpType
{
    internal const string TaskShortTypeName = "Task";
    internal const string TaskFullTypeName = "System.Threading.Tasks.Task";

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
    /// Creates a type-safe type that represents void.
    /// </summary>
    public static CSharpTypeVoid CreateVoid()
    {
        return CSharpTypeVoid.DefaultInstance;
    }
    
    /// <summary>
    /// Is the current type representing a <see cref="System.Threading.Tasks.Task"/> or a <see cref="System.Threading.Tasks.Task&lt;T&gt;"/>?
    /// </summary>
    public bool IsTask()
    {
        return (this is CSharpTypeName name && name.IsTask()) || (this is CSharpTypeGeneric generic && generic.IsTask());
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
}