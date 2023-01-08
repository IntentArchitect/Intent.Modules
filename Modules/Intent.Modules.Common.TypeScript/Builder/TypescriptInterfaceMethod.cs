using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptInterfaceMethod : TypescriptMember<TypescriptInterfaceMethod>
{
    public string ReturnType { get; }
    public string Name { get; }
    public IList<TypescriptParameter> Parameters { get; } = new List<TypescriptParameter>();

    public TypescriptInterfaceMethod(string returnType, string name)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        ReturnType = returnType;
        Name = name;
        BeforeSeparator = TypescriptCodeSeparatorType.NewLine;
        AfterSeparator = TypescriptCodeSeparatorType.NewLine;
    }

    public TypescriptInterfaceMethod AddParameter(string type, string name, Action<TypescriptParameter> configure = null)
    {
        var param = new TypescriptParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{indentation} {Name}({string.Join(", ", Parameters.Select(x => x.ToString()))}): {ReturnType};";
    }
}