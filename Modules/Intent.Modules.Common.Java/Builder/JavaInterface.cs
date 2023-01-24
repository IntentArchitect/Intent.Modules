namespace Intent.Modules.Common.Java.Builder;

public class JavaInterface
{
    public string Name { get; }

    public JavaInterface(string name)
    {
        Name = name;
    }
    
    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
//         return $@"{GetAttributes(indentation)}{indentation}{AccessModifier}{(IsPartial ? "partial " : "")}interface {Name}{GetGenericParameters()}{GetBaseTypes()}{GetGenericTypeConstraints(indentation)}
// {indentation}{{{GetMembers($"{indentation}    ")}
// {indentation}}}";
        return "";
    }
}