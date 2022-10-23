using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpClassMethod : CSharpDeclaration<CSharpClassMethod>, IHasCSharpStatements
{
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public string AsyncMode { get; private set; } = "";
    public string AccessModifier { get; private set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string ReturnType { get; private set; }
    public string Name { get; private set; }
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public CSharpClassMethod(string returnType, string name)
    {
        ReturnType = returnType;
        Name = name;
    }

    public CSharpClassMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpClassMethod AddStatement(string statement, Action<CSharpStatement> configure = null)
    {
        return AddStatement(new CSharpStatement(statement), configure);
    }

    public CSharpClassMethod AddStatement(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpClassMethod InsertStatement(int index, CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpClassMethod InsertStatements(int index, IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }
        configure?.Invoke(statements);
        return this;
    }

    public CSharpClassMethod AddStatements(string statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpClassMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpClassMethod AddStatements(IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
            statement.Parent = this;
        }
        configure?.Invoke(arrayed);

        return this;
    }


    public CSharpClassMethod Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpClassMethod Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpClassMethod WithoutAccessModifier()
    {
        AccessModifier = "";
        return this;
    }

    public CSharpClassMethod Override()
    {
        OverrideModifier = "override ";
        return this;
    }

    public CSharpClassMethod New()
    {
        OverrideModifier = "new ";
        return this;
    }

    public CSharpClassMethod Virtual()
    {
        OverrideModifier = "virtual ";
        return this;
    }

    public CSharpClassMethod Async()
    {
        AsyncMode = "async ";
        return this;
    }

    public void RemoveStatement(CSharpStatement statement)
    {
        Statements.Remove(statement);
    }

    public string ToString(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{OverrideModifier}{AsyncMode}{ReturnType} {Name}({string.Join(", ", Parameters.Select(x => x.ToString()))})
{indentation}{{{(Statements.Any() ? $@"
{string.Join($@"
", Statements.Select((s, index) => s.MustSeparateFromPrevious && index != 0 ? $@"
{s.GetText($"    {indentation}")}".TrimEnd() : s.GetText($"    {indentation}").TrimEnd()))}" : string.Empty)}
{indentation}}}";
    }
}