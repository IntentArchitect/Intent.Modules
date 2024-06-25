using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpReturnType : CSharpStatement
{
}

public class CSharpReturnTypeVoid : CSharpReturnType
{
    public CSharpReturnTypeVoid()
    {
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}void";
    }
}

public class CSharpReturnTypeName : CSharpReturnType
{
    public CSharpReturnTypeName(string typeName)
    {
        TypeName = typeName;
    }

    public string TypeName { get; private set; }

    public override string GetText(string indentation)
    {
        return $"{indentation}{TypeName}";
    }
}

public class CSharpReturnTypeGeneric : CSharpReturnType
{
    public CSharpReturnTypeGeneric(string typeName, IReadOnlyList<CSharpReturnType> typeArgumentList)
    {
        TypeName = typeName;
        TypeArgumentList = typeArgumentList;
    }

    public string TypeName { get; private set; }
    public IReadOnlyList<CSharpReturnType> TypeArgumentList { get; private set; }

    public override string GetText(string indentation)
    {
        return $"{indentation}{TypeName}<{string.Join(", ", TypeArgumentList.Select(s => s.GetText(string.Empty)))}>";
    }
}

public record CSharpTupleElement(CSharpReturnType Type, string? Name = null);

public class CSharpReturnTypeTuple : CSharpReturnType
{
    public CSharpReturnTypeTuple(IReadOnlyList<CSharpTupleElement> elements)
    {
        Elements = elements;
    }

    public IReadOnlyList<CSharpTupleElement> Elements { get; private set; }

    public override string GetText(string indentation)
    {
        return $"{indentation}({string.Join(", ", Elements.Select(s => {
            if (!string.IsNullOrWhiteSpace(s.Name))
            {
                return $"{s.Type.GetText(string.Empty)} {s.Name}";
            }

            return s.Type.GetText(string.Empty);
        }))})";
    }
}

public static class CSharpReturnTypeExtensions
{
    private const string TaskShortTypeName = "Task";
    private const string TaskFullTypeName = "System.Threading.Task";
    
    private const string ListShortTypeName = "List";
    private const string ListFullTypeName = "System.Collections.Generic.List";

    public static bool IsTask(this CSharpReturnType returnType)
    {
        return (returnType is CSharpReturnTypeName name && name.IsTask()) || (returnType is CSharpReturnTypeGeneric generic && generic.IsTask());
    }
    
    public static bool IsTask(this CSharpReturnTypeName name)
    {
        return name.TypeName is TaskFullTypeName or TaskShortTypeName;
    }
    
    public static bool IsTask(this CSharpReturnTypeGeneric generic)
    {
        return generic.TypeName is TaskFullTypeName or TaskShortTypeName;
    }

    public static bool IsList(this CSharpReturnTypeGeneric generic)
    {
        return generic.TypeName is ListFullTypeName or ListShortTypeName;
    }
    
    public static CSharpReturnType WrapInTask(this CSharpReturnType returnType)
    {
        if (returnType is CSharpReturnTypeVoid)
        {
            return new CSharpReturnTypeName(TaskFullTypeName);
        }
        
        return new CSharpReturnTypeGeneric(TaskFullTypeName, [returnType]);
    }

    public static CSharpReturnType WrapInList(this CSharpReturnType returnType)
    {
        return new CSharpReturnTypeGeneric(ListFullTypeName, [returnType]);
    }
}