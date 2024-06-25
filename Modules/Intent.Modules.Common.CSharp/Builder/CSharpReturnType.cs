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
        return $"{indentation}{TypeName}<{string.Join(", ", TypeArgumentList.Select(s => s.GetText("")))}>";
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
                return $"{s.Type.GetText("")} {s.Name}";
            }

            return s.Type.GetText("");
        }))})";
    }
}