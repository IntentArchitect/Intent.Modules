using System;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Builder;

public class JavaConstructorParameter
{
    private readonly JavaConstructor _constructor;
    public string Type { get; }
    public string Name { get; }

    public JavaConstructorParameter(string type, string name, JavaConstructor constructor)
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
    public JavaConstructorParameter IntroduceField(Action<JavaField> configure = null)
    {
        return IntroduceField((field, _) => configure?.Invoke(field));
    }

    public JavaConstructorParameter IntroduceField(Action<JavaField, JavaFieldAssignmentStatement> configure)
    {
        _constructor.Class.AddField(Type, Name.ToCamelCase(), field =>
        {
            var statement = new JavaFieldAssignmentStatement(field.Name, Name);
            _constructor.AddStatement(statement);
            configure?.Invoke(field, statement);
        });
        return this;
    }

    public JavaConstructorParameter IntroduceReadonlyField(Action<JavaField> configure = null)
    {
        return IntroduceReadonlyField((field, _) => configure?.Invoke(field));
    }
    
    public JavaConstructorParameter IntroduceReadonlyField(Action<JavaField, JavaFieldAssignmentStatement> configure)
    {
        return IntroduceField((field, statement) =>
        {
            field.PrivateFinal();
            configure?.Invoke(field, statement);
        });
    }

    public override string ToString()
    {
        return $@"{Type} {Name}";
    }
}