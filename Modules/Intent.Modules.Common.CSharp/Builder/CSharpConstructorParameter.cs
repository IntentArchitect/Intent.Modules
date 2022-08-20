using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructorParameter
{
    private readonly CSharpConstructor _constructor;
    public string Type { get; }
    public string Name { get; }

    public CSharpConstructorParameter(string type, string name, CSharpConstructor constructor)
    {
        _constructor = constructor;
        Type = type;
        Name = name;
    }

    public CSharpField IntroduceField()
    {
        var field = _constructor.Class.AddField(Type, Name.ToPrivateMemberName());
        _constructor.AddStatement($"{(field.Name == Name ? "this." : "")}{field.Name} = {Name};");
        return field;
    }

    public CSharpField IntroduceReadonlyField()
    {
        return IntroduceField().PrivateReadOnly();
    }

    public CSharpProperty IntroduceProperty()
    {
        var property = _constructor.Class.AddProperty(Type, Name.ToPascalCase());
        _constructor.AddStatement($"{(property.Name == Name ? "this." : "")}{property.Name} = {Name};");
        return property;
    }

    public override string ToString()
    {
        return $@"{Type} {Name}";
    }
}