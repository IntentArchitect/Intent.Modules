using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpPrimaryConstructor
{
    private readonly CSharpClass _class;

    public List<PrimaryConstructorParameter> Parameters { get; } = new();

    public CSharpPrimaryConstructor(CSharpClass @class)
    {
        _class = @class;
    }

    public CSharpPrimaryConstructor AddClassParameter(string type, string name, string? value = null)
    {
        if (_class.TypeDefinitionType is not CSharpClass.Type.Class)
        {
            throw new InvalidOperationException($"Trying to add a Class parameter to a Primary Constructor that is on a {_class.TypeDefinitionType}");
        }

        var field = CSharpField.CreateFieldOmittedFromRender(type, name, null);
        Parameters.Add(new PrimaryConstructorParameter(type, name, value));
        _class.Fields.Add(field);
        return this;
    }
    
    public CSharpPrimaryConstructor AddRecordParameter(string type, string name, string? value = null)
    {
        if (_class.TypeDefinitionType is not CSharpClass.Type.Record)
        {
            throw new InvalidOperationException($"Trying to add a Record parameter to a Primary Constructor that is on a {_class.TypeDefinitionType}");
        }

        var field = CSharpProperty.CreatePropertyOmittedFromRender(type, name, _class);
        Parameters.Add(new PrimaryConstructorParameter(type, name, value));
        _class.Properties.Add(field);
        return this;
    }
    
    public string GetText(string indentation)
    {
        if (Parameters.Count == 0)
        {
            return string.Empty;
        }

        return $"({ToStringParameters(indentation)})";
    }
    
    private string ToStringParameters(string indentation)
    {
        if (Parameters.Count > 1 && $"{indentation}{_class.Name}(".Length + Parameters.Sum(x => x.ToString().Length) > 120)
        {
            return string.Join($@",
{indentation}    ", Parameters.Select(x => x.ToString()));
        }
        else
        {
            return string.Join(", ", Parameters.Select(x => x.ToString()));
        }
    }
}

public record PrimaryConstructorParameter(string Type, string Name, string? Value)
{
    public override string ToString()
    {
        return $"{Type} {Name}{(!string.IsNullOrEmpty(Value) ? " = " + Value : string.Empty)}";
    }
}