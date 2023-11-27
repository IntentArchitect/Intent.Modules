using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpLocalMethod : CSharpStatement, IHasCSharpStatements, IHasICSharpParameters
{
    public CSharpLocalMethod(string returnType, string name, CSharpFile file) : base(string.Empty)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
        File = file;
        ReturnType = returnType;

        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    protected bool IsAsync { get; private set; }
    public bool IsStatic { get; private set; }
    public bool HasExpressionBody { get; private set; }
    public string ReturnType { get; private set; }
    public string Name { get; }
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => Parameters;

    public CSharpLocalMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpLocalMethod AddOptionalCancellationTokenParameter<T>(CSharpTemplateBase<T> template) =>
        AddParameter(
            $"{template.UseType("System.Threading.CancellationToken")}", "cancellationToken",
            parameter => parameter.WithDefaultValue("default"));

    public CSharpLocalMethod AddGenericParameter(string typeName)
    {
        return AddGenericParameter(typeName, out _);
    }

    public CSharpLocalMethod AddGenericParameter(string typeName, out CSharpGenericParameter param)
    {
        param = new CSharpGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }

    public CSharpLocalMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    public CSharpLocalMethod AddStatement(string statement, Action<CSharpStatement> configure = null)
    {
        return AddStatement(new CSharpStatement(statement), configure);
    }

    public CSharpLocalMethod AddStatement<T>(T statement, Action<T> configure = null)
        where T : CSharpStatement
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpLocalMethod InsertStatement<T>(int index, T statement, Action<T> configure = null)
        where T : CSharpStatement
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpLocalMethod InsertStatements<T>(int index, IReadOnlyCollection<T> statements, Action<IEnumerable<T>> configure = null)
        where T : CSharpStatement
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }
        configure?.Invoke(statements);
        return this;
    }

    public CSharpLocalMethod AddStatements(string statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpLocalMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpLocalMethod AddStatements<T>(IEnumerable<T> statements, Action<IEnumerable<T>> configure = null)
        where T : CSharpStatement
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

    public CSharpLocalMethod FindAndReplaceStatement(Func<CSharpStatement, bool> matchFunc, CSharpStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public CSharpLocalMethod WithReturnType(string returnType)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        ReturnType = returnType;
        return this;
    }

    public CSharpLocalMethod Static()
    {
        IsStatic = true;
        return this;
    }

    public void RemoveStatement(CSharpStatement statement)
    {
        Statements.Remove(statement);
    }

    public CSharpLocalMethod WithExpressionBody<T>(T statement, Action<T> configure = null)
        where T : CSharpStatement
    {
        HasExpressionBody = true;
        statement.BeforeSeparator = CSharpCodeSeparatorType.None;

        if (statement is CSharpMethodChainStatement stmt)
        {
            stmt.WithoutSemicolon();
        }

        Statements.Add(statement);

        configure?.Invoke(statement);

        return this;
    }

    public CSharpLocalMethod Async()
    {
        IsAsync = true;

        if (!ReturnType.StartsWith("Task"))
        {
            ReturnType = ReturnType == "void" ? "Task" : $"Task<{ReturnType}>";
        }

        return this;
    }

    public override string GetText(string indentation)
    {
        var sb = new StringBuilder();

        sb.Append(indentation);

        if (IsStatic)
        {
            sb.Append("static ");
        }

        if (IsAsync)
        {
            sb.Append("async ");
        }

        sb.Append(ReturnType);
        sb.Append(' ');
        sb.Append(Name);

        if (GenericParameters.Any())
        {
            sb.Append("<");
            sb.Append(string.Join(", ", GenericParameters));
            sb.Append(">");
        }

        sb.Append($"({GetParameters(indentation, currentLineLength: sb.Length)})");

        foreach (var genericTypeConstraint in GenericTypeConstraints)
        {
            sb.AppendLine();
            sb.Append(indentation);
            sb.Append(genericTypeConstraint);
        }

        if (HasExpressionBody)
        {
            var expressionBody = Statements.ConcatCode($"{indentation}    ");
            if (expressionBody.Contains("\n"))
            {
                return $@"{sb} => 
{indentation}    {expressionBody};";
            }

            return $"{sb} => {expressionBody};";
        }

        return $@"{sb}
{indentation}{{{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }

    private string GetParameters(string indentation, int currentLineLength)
    {
        if (Parameters.Count > 1 && currentLineLength + Parameters.Sum(x => x.ToString()!.Length) > 120)
        {
            return $@"
{indentation}    {string.Join($@",
{indentation}    ", Parameters.Select(x => x.ToString()))}";
        }

        return string.Join(", ", Parameters.Select(x => x.ToString()));
    }

    public override string ToString()
    {
        return GetText(indentation: string.Empty);
    }
}