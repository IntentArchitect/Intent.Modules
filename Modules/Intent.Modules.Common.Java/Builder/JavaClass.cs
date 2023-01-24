using System;

namespace Intent.Modules.Common.Java.Builder;

public class JavaClass
{
    public JavaClass(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
    }

    public string Name { get; }
    
    public override string ToString()
    {
        return ToString(string.Empty);
    }
    
    public string ToString(string indentation)
    {
//         return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{(IsSealed ? "sealed " : "")}{(IsStatic ? "static " : "")}{(IsAbstract ? "abstract " : "")}{(IsPartial ? "partial " : "")}class {Name}{GetGenericParameters()}{GetBaseTypes()}{GetGenericTypeConstraints(indentation)}
// {indentation}{{{GetMembers($"{indentation}    ")}
// {indentation}}}";
        return "";
    }
}