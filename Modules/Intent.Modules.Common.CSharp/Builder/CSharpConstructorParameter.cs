using System;
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

    public CSharpConstructorParameter IntroduceField(Action<CSharpField> configure = null)
    {
        _constructor.Class.AddField(Type, Name.ToPrivateMemberName(), field =>
        {
            _constructor.AddStatement($"{(field.Name == Name ? "this." : "")}{field.Name} = {Name};");
            configure?.Invoke(field);
        });
        return this;
    }

    public CSharpConstructorParameter IntroduceReadonlyField(Action<CSharpField> configure = null)
    {
        return IntroduceField(field =>
        {
            field.PrivateReadOnly();
            configure?.Invoke(field);
        });
    }

    public CSharpConstructorParameter IntroduceProperty(Action<CSharpProperty> configure = null)
    {
        _constructor.Class.AddProperty(Type, Name.ToPascalCase(), property =>
        {
            _constructor.AddStatement($"{(property.Name == Name ? "this." : "")}{property.Name} = {Name};");
            configure?.Invoke(property);
        });
        return this;
    }

    public override string ToString()
    {
        return $@"{Type} {Name}";
    }
}