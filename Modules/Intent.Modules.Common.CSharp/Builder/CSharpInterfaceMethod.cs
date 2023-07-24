using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceMethod : CSharpMember<CSharpInterfaceMethod>, IHasCSharpStatements, IHasICSharpParameters
{
    public string ReturnType { get; }
    public string Name { get; }
    public bool IsAbstract { get; set; } = true;
    public bool IsStatic { get; set; }
    public bool HasExpressionBody { get; private set; }
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();

    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => this.Parameters;

    public CSharpInterfaceMethod(string returnType, string name)
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
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public CSharpInterfaceMethod WithDefaultImplementation()
    {
        IsAbstract = false;

        return this;
    }

    public CSharpInterfaceMethod Static()
    {
        IsStatic = true;

        return this;
    }

    public CSharpInterfaceMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }
    
    public CSharpInterfaceMethod AddGenericParameter(string typeName)
    {
        var param = new CSharpGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }
    
    public CSharpInterfaceMethod AddGenericParameter(string typeName, out CSharpGenericParameter param)
    {
        param = new CSharpGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }
    
    public CSharpInterfaceMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    public CSharpInterfaceMethod AddStatement(string statement, Action<CSharpStatement> configure = null)
    {
        return AddStatement(new CSharpStatement(statement), configure);
    }

    public CSharpInterfaceMethod AddStatement<TStatement>(TStatement statement, Action<TStatement> configure = null)
        where TStatement : CSharpStatement
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpInterfaceMethod WithExpressionBody(string statement, Action<CSharpStatement> configure = null)
    {
        return WithExpressionBody(new CSharpStatement(statement), configure);
    }

    public CSharpInterfaceMethod WithExpressionBody<TStatement>(TStatement statement, Action<TStatement> configure = null)
        where TStatement : CSharpStatement
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

    public CSharpInterfaceMethod AddOptionalCancellationTokenParameter<T>(CSharpTemplateBase<T> template) =>
        AddParameter(
            $"{template.UseType("System.Threading.CancellationToken")}", "cancellationToken",
            parameter => parameter.WithDefaultValue("default"));

    public override string GetText(string indentation)
    {
        var @static = IsStatic
            ? $"static {(IsAbstract ? "abstract " : string.Empty)}"
            : string.Empty;

        var declaration = $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{@static}{ReturnType} {Name}{GetGenericParameters()}({string.Join(", ", Parameters.Select(x => x.ToString()))}){GetGenericTypeConstraints(indentation)}";
        if (IsAbstract && Statements.Count == 0)
        {
            return $@"{declaration};";
        }

        if (HasExpressionBody)
        {
            var expressionBody = Statements.ConcatCode($"{indentation}    ");
            if (expressionBody.Contains("\n"))
            {
                return $@"{declaration} => 
{indentation}    {expressionBody};";
            }
            return $@"{declaration} => {expressionBody};";
        }

        return $@"{declaration}
{indentation}{{{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }

    private string GetGenericTypeConstraints(string indentation)
    {
        if (!GenericTypeConstraints.Any())
        {
            return string.Empty;
        }

        string newLine = $@"
{indentation}    ";
        return newLine + string.Join(newLine, GenericTypeConstraints);
    }
    
    private string GetGenericParameters()
    {
        if (!GenericParameters.Any())
        {
            return string.Empty;
        }

        return $"<{string.Join(", ", GenericParameters)}>";
    }
}