using System;
using Intent.Modules.Common.Typescript.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptConstructorParameter
{
    private readonly TypescriptConstructor _constructor;
    public string Type { get; }
    public string Name { get; }

    public TypescriptConstructorParameter(string type, string name, TypescriptConstructor constructor)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _constructor = constructor;
        Type = type;
        Name = name;
    }
    public TypescriptConstructorParameter IntroduceField(Action<TypescriptField> configure = null)
    {
        return IntroduceField((field, _) => configure?.Invoke(field));
    }

    public TypescriptConstructorParameter IntroduceField(Action<TypescriptField, TypescriptFieldAssignmentStatement> configure)
    {
        _constructor.Class.AddField(Type, Name.ToPrivateMemberName(), field =>
        {
            var statement = new TypescriptFieldAssignmentStatement(field.Name, Name);
            _constructor.AddStatement(statement);
            configure?.Invoke(field, statement);
        });
        return this;
    }

    public TypescriptConstructorParameter IntroduceReadonlyField(Action<TypescriptField> configure = null)
    {
        return IntroduceReadonlyField((field, _) => configure?.Invoke(field));
    }

    public TypescriptConstructorParameter IntroduceReadonlyField(Action<TypescriptField, TypescriptFieldAssignmentStatement> configure)
    {
        return IntroduceField((field, statement) =>
        {
            field.PrivateReadOnly();
            configure?.Invoke(field, statement);
        });
    }

    public TypescriptConstructorParameter IntroduceProperty(Action<TypescriptProperty> configure = null)
    {
        return IntroduceProperty((property, _) => configure?.Invoke(property));
    }

    public TypescriptConstructorParameter IntroduceProperty(Action<TypescriptProperty, TypescriptFieldAssignmentStatement> configure)
    {
        _constructor.Class.AddProperty(Type, Name.ToPascalCase(), property =>
        {
            var statement = new TypescriptFieldAssignmentStatement(property.Name, Name);
            _constructor.AddStatement(statement);
            configure?.Invoke(property, statement);
        });
        return this;
    }

    public override string ToString()
    {
        return $@"{Type} {Name}";
    }
}