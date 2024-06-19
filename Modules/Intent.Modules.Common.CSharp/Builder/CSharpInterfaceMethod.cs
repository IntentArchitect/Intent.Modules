using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceMethod : CSharpMember<CSharpInterfaceMethod>, ICSharpInterfaceMethod, ICSharpMethodDeclaration
{
    private readonly ICSharpInterfaceMethod _wrapper;
    public string ReturnType { get; private set; }
    ICSharpExpression ICSharpMethodDeclaration.ReturnType => new CSharpStatement(ReturnType);
    ICSharpExpression ICSharpMethodDeclarationActual.ReturnType => new CSharpStatement(ReturnType);
    public string Name { get; }
    public bool IsAsync { get; private set; } = false;
    public bool IsAbstract { get; set; } = true;
    public bool IsStatic { get; set; }
    public bool HasExpressionBody { get; private set; }
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();

    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => this.Parameters;

    public CSharpInterfaceMethod(string returnType, string name, CSharpInterface parent)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapper = new CSharpInterfaceMethodWrapper(this);
        ReturnType = returnType;
        Name = name;
        Parent = parent;
        File = parent.File;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public CSharpInterfaceMethod WithDefaultImplementation()
    {
        IsAbstract = false;

        return this;
    }

    public CSharpInterfaceMethod Async()
    {
        IsAsync = true;
        var taskType = File.Template?.UseType("System.Threading.Tasks.Task") ?? "Task";
        if (!ReturnType.StartsWith(taskType))
        {
            ReturnType = ReturnType == "void" ? taskType : $"{taskType}<{ReturnType}>";
        }
        return this;
    }

    public CSharpInterfaceMethod Static()
    {
        IsStatic = true;

        return this;
    }

    public CSharpInterfaceMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }
    
    public CSharpInterfaceMethod InsertParameter(int index, string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Insert(index, param);
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

    public CSharpInterfaceMethod WithReturnType(string returnType)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        ReturnType = returnType;
        return this;
    }

    /// <summary>
    /// Use <see cref="AddOptionalCancellationTokenParameter()"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpInterfaceMethod AddOptionalCancellationTokenParameter<T>(CSharpTemplateBase<T> template) =>
        AddParameter(
            $"{template.UseType("System.Threading.CancellationToken")}", "cancellationToken",
            parameter => parameter.WithDefaultValue("default"));

    public CSharpInterfaceMethod AddOptionalCancellationTokenParameter() =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: "cancellationToken",
            configure: parameter => parameter.WithDefaultValue("default"));

    public CSharpInterfaceMethod AddOptionalCancellationTokenParameter(string parameterName) =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: parameterName,
            configure: parameter => parameter.WithDefaultValue("default"));

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

    #region ICSharpInterfaceMethod implementation

    IList<ICSharpGenericParameter> ICSharpInterfaceMethod.GenericParameters => _wrapper.GenericParameters;

    IList<ICSharpGenericTypeConstraint> ICSharpInterfaceMethod.GenericTypeConstraints => _wrapper.GenericTypeConstraints;

    ICSharpInterfaceMethod ICSharpInterfaceMethod.Async() => _wrapper.Async();

    ICSharpInterfaceMethod ICSharpInterfaceMethod.Static() => _wrapper.Static();

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddParameter(string type, string name, Action<ICSharpParameter> configure) => _wrapper.AddParameter(type, name, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.InsertParameter(int index, string type, string name, Action<ICSharpParameter> configure) => _wrapper.InsertParameter(index, type, name, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddGenericParameter(string typeName) => _wrapper.AddGenericParameter(typeName);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddGenericParameter(string typeName, out ICSharpGenericParameter param) => _wrapper.AddGenericParameter(typeName, out param);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure) => _wrapper.AddGenericTypeConstraint(genericParameterName, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddStatement(string statement, Action<ICSharpStatement> configure) => _wrapper.AddStatement(statement, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddStatement<TStatement>(TStatement statement, Action<TStatement> configure) => _wrapper.AddStatement(statement, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithExpressionBody(string statement, Action<ICSharpStatement> configure) => _wrapper.WithExpressionBody(statement, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithExpressionBody<TStatement>(TStatement statement, Action<TStatement> configure) => _wrapper.WithExpressionBody(statement, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithReturnType(string returnType) => _wrapper.WithReturnType(returnType);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithDefaultImplementation() => _wrapper.WithDefaultImplementation();

    ICSharpInterfaceMethod ICSharpDeclaration<ICSharpInterfaceMethod>.AddAttribute(string name, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(name, configure);

    ICSharpInterfaceMethod ICSharpDeclaration<ICSharpInterfaceMethod>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(attribute, configure);

    ICSharpInterfaceMethod ICSharpDeclaration<ICSharpInterfaceMethod>.WithComments(string xmlComments) => _wrapper.WithComments(xmlComments);

    ICSharpInterfaceMethod ICSharpDeclaration<ICSharpInterfaceMethod>.WithComments(IEnumerable<string> xmlComments) => _wrapper.WithComments(xmlComments);

    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => _wrapper.Statements;

    #endregion
}