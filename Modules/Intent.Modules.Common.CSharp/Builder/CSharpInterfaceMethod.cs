using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceMethod : CSharpMember<CSharpInterfaceMethod>, ICSharpInterfaceMethod, ICSharpMethodDeclaration
{
    private readonly ICSharpInterfaceMethod _wrapper;
    public CSharpType ReturnTypeInfo { get; private set; }
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
        ReturnTypeInfo = CSharpTypeParser.Parse(returnType);
        Name = name;
        Parent = parent;
        File = parent.File;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
    }
    
    public CSharpInterfaceMethod(CSharpType returnType, string name, CSharpInterface parent)
    {
        if (returnType is null)
        {
            throw new ArgumentException("Cannot be null", nameof(returnType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        ReturnType = returnType.ToString();
        ReturnTypeInfo = returnType;
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
    
    /// <summary>
    /// Indicates that this method is async and sets the return type to a <see cref="System.Threading.Tasks.Task"/> or <see cref="System.Threading.Tasks.Task&lt;T&gt;"/>.
    /// </summary>
    public CSharpInterfaceMethod Async()
    {
        return Async(false);
    }

    /// <summary>
    /// Indicates that this method is async and sets the return type to a
    /// <see cref="System.Threading.Tasks.Task"/> / <see cref="System.Threading.Tasks.ValueTask"/> or
    /// <see cref="System.Threading.Tasks.Task&lt;T&gt;"/> / <see cref="System.Threading.Tasks.ValueTask&lt;T&gt;"/>.
    /// </summary>
    /// <param name="asValueTask">If true it will use <see cref="System.Threading.Tasks.ValueTask"/> instead of <see cref="System.Threading.Tasks.Task"/>.</param>
    public CSharpInterfaceMethod Async(bool asValueTask)
    {
        IsAsync = true;
        if (asValueTask)
        {
            if (ReturnTypeInfo.IsTask())
            {
                var genericType = ReturnTypeInfo.GetTaskGenericType();
                if (genericType is null)
                {
                    ReturnTypeInfo = CSharpType.CreateValueTask(File.Template);
                }
                else
                {
                    ReturnTypeInfo = CSharpType.CreateValueTask(genericType, File.Template);
                }
                ReturnType = ReturnTypeInfo.ToString();
            }
            else if (!ReturnTypeInfo.IsValueTask())
            {
                ReturnTypeInfo = ReturnTypeInfo.WrapInValueTask(File.Template);
                ReturnType = ReturnTypeInfo.ToString();
            }
        }
        else
        {
            if (ReturnTypeInfo.IsValueTask())
            {
                var genericType = ReturnTypeInfo.GetValueTaskGenericType();
                if (genericType is null)
                {
                    ReturnTypeInfo = CSharpType.CreateTask(File.Template);
                }
                else
                {
                    ReturnTypeInfo = CSharpType.CreateTask(genericType, File.Template);
                }
                ReturnType = ReturnTypeInfo.ToString();
            }
            else if (!ReturnTypeInfo.IsTask())
            {
                ReturnTypeInfo = ReturnTypeInfo.WrapInTask(File.Template);
                ReturnType = ReturnTypeInfo.ToString();
            }
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

        ReturnTypeInfo = CSharpTypeParser.Parse(returnType);
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

        var declaration = $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{@static}{ReturnTypeInfo} {Name}{GetGenericParameters()}({string.Join(", ", Parameters.Select(x => x.ToString()))}){GetGenericTypeConstraints(indentation)}";
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