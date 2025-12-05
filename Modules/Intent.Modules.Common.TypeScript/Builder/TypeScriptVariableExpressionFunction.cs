using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypeScriptVariableExpressionFunction : TypescriptVariableValue, IHasTypescriptStatements
{
    public List<TypescriptStatement> Statements { get; } = new();

    public List<TypescriptParameter> Parameters { get; } = new();

    public TypeScriptVariableExpressionFunction()
    {
        BeforeSeparator = TypescriptCodeSeparatorType.EmptyLines;
        AfterSeparator = TypescriptCodeSeparatorType.EmptyLines;
    }

    public TypeScriptVariableExpressionFunction AddParameter(string name, string type, Action<TypescriptParameter> configure = null)
    {
        var param = new TypescriptParameter(name, type);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public TypeScriptVariableExpressionFunction AddStatement(string statement, Action<TypescriptStatement> configure = null)
    {
        return AddStatement(new TypescriptStatement(statement), configure);
    }

    public TypeScriptVariableExpressionFunction AddStatement(TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public TypeScriptVariableExpressionFunction InsertStatement(int index, TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public TypeScriptVariableExpressionFunction InsertStatements(int index, IReadOnlyCollection<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }
        configure?.Invoke(statements);
        return this;
    }

    public TypeScriptVariableExpressionFunction AddStatements(string statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public TypeScriptVariableExpressionFunction AddStatements(IEnumerable<string> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new TypescriptStatement(x)), configure);
    }

    public TypeScriptVariableExpressionFunction AddStatements(IEnumerable<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
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

    public TypeScriptVariableExpressionFunction FindAndReplaceStatement(Func<TypescriptStatement, bool> matchFunc, TypescriptStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public override string ToString()
    {
        return GetText("");
    }

    public override string GetText(string indentation)
    {
        indentation += Indentation;

        var text = $@"{GetComments(indentation)}{GetDecorators(indentation)}({string.Join(", ", Parameters.Select(x => x.ToString()))}) => {{{Statements.ConcatCode("")}";
        text = $"{text.Replace(Environment.NewLine, $"{Environment.NewLine}{indentation}")}";
        return $@"{text}
}}";

    }
}
