using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpMethod
{
    private readonly IList<CSharpParameter> _parameters = new List<CSharpParameter>();
    private readonly IList<string> _statements = new List<string>();
    public string ReturnType { get; private set; }
    public string Name { get; private set; }

    public string AccessModifier { get; private set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string AsyncMode { get; private set; } = "";
    public IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();
    public CSharpMethod(string returnType, string name)
    {
        ReturnType = returnType;
        Name = name;
    }

    public CSharpMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name);
        _parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpMethod AddStatement(string statement)
    {
        _statements.Add(statement);
        return this;
    }

    public CSharpMethod InsertStatement(int index, string statement)
    {
        _statements.Insert(index, statement);
        return this;
    }

    public CSharpMethod AddStatements(IEnumerable<string> statements)
    {
        foreach (var statement in statements) 
            _statements.Add(statement);

        return this;
    }

    public CSharpAttribute AddAttribute(string name)
    {

    }

    public CSharpMethod Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpMethod Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpMethod Override()
    {
        OverrideModifier = "override ";
        return this;
    }

    public CSharpMethod New()
    {
        OverrideModifier = "new ";
        return this;
    }

    public CSharpMethod Virtual()
    {
        OverrideModifier = "virtual ";
        return this;
    }

    public CSharpMethod Async()
    {
        AsyncMode = "async ";
        return this;
    }

    public CSharpMethod AddMetadata(string key, string value)
    {
        Metadata.Add(key, value);
        return this;
    }

    public bool TryGetMetadata(string key, out string value)
    {
        return Metadata.TryGetValue(key, out value);
    }
    public string ToString(string indentation)
    {
        return $@"{indentation}{AccessModifier}{OverrideModifier}{AsyncMode}{ReturnType} {Name}({string.Join(", ", _parameters.Select(x => x.ToString()))})
{indentation}{{{(_statements.Any() ? $@"
{string.Join($@"
", _statements.Select(x => $"{indentation}    {x}".TrimEnd()))}" : string.Empty)}
{indentation}}}";
    }
}