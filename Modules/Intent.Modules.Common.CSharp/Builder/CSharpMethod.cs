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
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public string ReturnType { get; private set; }
    public string Name { get; private set; }
    public string AccessModifier { get; private set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string AsyncMode { get; private set; } = "";
    public IList<CSharpAttribute> Attributes { get; } = new List<CSharpAttribute>();
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public CSharpXmlComments XmlComments { get; } = new CSharpXmlComments();
    public IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();
    public CSharpMethod(string returnType, string name)
    {
        ReturnType = returnType;
        Name = name;
    }

    public CSharpMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpMethod AddStatement(string statement, Action<CSharpStatement> configure = null)
    {
        var s = new CSharpStatement(statement);
        Statements.Add(s);
        configure?.Invoke(s);
        return this;
    }

    public CSharpMethod InsertStatement(int index, string statement, Action<CSharpStatement> configure = null)
    {
        var s = new CSharpStatement(statement);
        Statements.Insert(index, s);
        configure?.Invoke(s);
        return this;
    }

    public CSharpMethod AddStatements(string statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpMethod AddStatements(IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
        }
        configure?.Invoke(arrayed);

        return this;
    }

    public CSharpMethod AddAttribute(string name, Action<CSharpAttribute> configure = null)
    {
        var param = new CSharpAttribute(name);
        Attributes.Add(param);
        configure?.Invoke(param);
        return this;
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

    public CSharpMethod WithComments(string xmlComments)
    {
        XmlComments.AddStatements(xmlComments);
        return this;
    }

    public CSharpMethod WithComments(IEnumerable<string> xmlComments)
    {
        XmlComments.AddStatements(xmlComments);
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
        return $@"{(!XmlComments.IsEmpty() ? $@"{XmlComments.ToString(indentation)}
" : string.Empty)}{(Attributes.Any() ? $@"{indentation}{string.Join($@"
{indentation}", Attributes)}
" : string.Empty)}{indentation}{AccessModifier}{OverrideModifier}{AsyncMode}{ReturnType} {Name}({string.Join(", ", Parameters.Select(x => x.ToString()))})
{indentation}{{{(Statements.Any() ? $@"
{string.Join($@"
", Statements.Select((s, index) => s.MustSeparateFromPrevious && index != 0 ? $@"
{indentation}    {s}".TrimEnd() : $"{indentation}    {s}".TrimEnd()))}" : string.Empty)}
{indentation}}}";
    }
}