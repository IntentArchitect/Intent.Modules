using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpType
{
    internal const string TaskShortTypeName = "Task";
    internal const string TaskFullTypeName = "System.Threading.Tasks.Task";

    public static CSharpTypeName CreateTask(ICSharpTemplate? template)
    {
        return new CSharpTypeName(template?.UseType(TaskFullTypeName) ?? TaskFullTypeName);
    }

    public static CSharpTypeGeneric CreateTask(CSharpType genericParamType, ICSharpTemplate? template)
    {
        return new CSharpTypeGeneric(template?.UseType(TaskFullTypeName) ?? TaskFullTypeName, [genericParamType]);
    }

    public static CSharpTypeVoid CreateVoid()
    {
        return CSharpTypeVoid.DefaultInstance;
    }
    
    public bool IsTask()
    {
        return (this is CSharpTypeName name && name.IsTask()) || (this is CSharpTypeGeneric generic && generic.IsTask());
    }
    
    public CSharpType WrapInTask(ICSharpTemplate template)
    {
        if (this is CSharpTypeVoid)
        {
            return CSharpType.CreateTask(template);
        }

        return CSharpType.CreateTask(this, template);
    }

    public CSharpType? GetTaskType()
    {
        if (!this.IsTask())
        {
            return null;
        }

        return (this as CSharpTypeGeneric)?.TypeArgumentList.FirstOrDefault();
    }
}