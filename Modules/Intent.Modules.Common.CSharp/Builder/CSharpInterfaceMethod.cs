#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceMethod : CSharpMember<CSharpInterfaceMethod>, ICSharpInterfaceMethodDeclaration, ICSharpMethodDeclaration
{
    private readonly ICSharpInterfaceMethodDeclaration _wrapper;
    public CSharpInterface Interface => (CSharpInterface)Parent;
    public CSharpType ReturnTypeInfo { get; private set; }
    public string? ReturnType => ReturnTypeInfo.ToString();
    protected string OverrideModifier { get; private set; } = string.Empty;
    protected string AccessModifier { get; private set; } = string.Empty;
    ICSharpExpression ICSharpMethodDeclaration.ReturnType => new CSharpStatement(ReturnType);
    public string Name { get; }
    public bool IsAsync { get; private set; }
    public bool IsAbstract { get; set; } = true;
    public bool IsStatic { get; set; }
    public void RemoveStatement(CSharpStatement statement) => Statements.Remove(statement);
    public bool HasExpressionBody { get; private set; }
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();
    public string? ExplicitImplementationFor { get; private set; }

    public CSharpInterfaceMethod IsExplicitImplementationFor(string @interface)
    {
        ExplicitImplementationFor = @interface;
        return this;
    }

    public CSharpInterfaceMethod New()
    {
        OverrideModifier = "new ";
        return this;
    }

    public CSharpInterfaceMethod Override()
    {
        OverrideModifier = "override ";
        return this;
    }

    public CSharpInterfaceMethod Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpInterfaceMethod Protected()
    {
        AccessModifier = "protected ";
        return this;
    }

    public CSharpInterfaceMethod Virtual() => this;

    public CSharpInterfaceMethod WithoutAccessModifier()
    {
        AccessModifier = string.Empty;
        return this;
    }

    public CSharpInterfaceMethod(string returnType, string name, CSharpInterface parent)
        : this(CSharpTypeParser.Parse(returnType), name, parent)
    {
    }

    public CSharpInterfaceMethod(CSharpType returnType, string name, CSharpInterface parent)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapper = new CSharpInterfaceMethodWrapper(this);
        ReturnTypeInfo = returnType ?? throw new ArgumentException("Cannot be null", nameof(returnType));
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
            }
            else if (!ReturnTypeInfo.IsValueTask())
            {
                ReturnTypeInfo = ReturnTypeInfo.WrapInValueTask(File.Template);
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
            }
            else if (!ReturnTypeInfo.IsTask())
            {
                ReturnTypeInfo = ReturnTypeInfo.WrapInTask(File.Template);
            }
        }
        return this;
    }

    public CSharpInterfaceMethod Static()
    {
        IsStatic = true;

        return this;
    }

    public CSharpInterfaceMethod Sync()
    {
        IsAsync = false;
        if (ReturnTypeInfo is CSharpTypeGeneric generic && generic.IsTask())
        {
            ReturnTypeInfo = generic.TypeArgumentList.Single();
        }
        return this;
    }

    public CSharpInterfaceMethod AddParameter(string type, string name, Action<CSharpParameter>? configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    /// <summary>
    /// Resolves the parameter name from the <paramref name="model"/>. Registers this parameter as the representative of the <paramref name="model"/>.
    /// </summary>
    public CSharpInterfaceMethod AddParameter<TModel>(string type, TModel model, Action<CSharpParameter>? configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        return AddParameter(type, model.Name.ToParameterName(), param =>
        {
            param.RepresentsModel(model);
            configure?.Invoke(param);
        });
    }

    public CSharpInterfaceMethod AddParameter<TModel>(TModel model, Action<CSharpParameter>? configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        return AddParameter(File.GetModelType(model), model.Name.ToParameterName(), param =>
        {
            param.RepresentsModel(model);
            configure?.Invoke(param);
        });
    }

    /// <summary>
    /// Calls <see cref="AddParameter{TModel}(TModel,System.Action{Intent.Modules.Common.CSharp.Builder.CSharpParameter})"/> for each of the supplied <paramref name="models"/>
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="models"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public CSharpInterfaceMethod AddParameters<TModel>(IEnumerable<TModel> models, Action<CSharpParameter>? configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        foreach (var model in models)
        {
            AddParameter(model, configure);
        }

        return this;
    }

    public CSharpInterfaceMethod FindAndReplaceStatement(Func<CSharpStatement, bool> matchFunc, CSharpStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public CSharpInterfaceMethod InsertParameter(int index, string type, string name, Action<CSharpParameter>? configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Insert(index, param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpInterfaceMethod InsertStatements(int index, IReadOnlyCollection<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>>? configure = null)
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }

        configure?.Invoke(statements);
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

    public CSharpInterfaceMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint>? configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure?.Invoke(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    public CSharpInterfaceMethod AddStatement(string statement, Action<CSharpStatement>? configure = null)
    {
        return AddStatement(new CSharpStatement(statement), configure);
    }

    public CSharpInterfaceMethod AddStatement<TStatement>(TStatement statement, Action<TStatement>? configure = null)
        where TStatement : CSharpStatement
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpInterfaceMethod WithExpressionBody(string statement, Action<CSharpStatement>? configure = null)
    {
        return WithExpressionBody(new CSharpStatement(statement), configure);
    }

    public CSharpInterfaceMethod WithExpressionBody<TStatement>(TStatement statement, Action<TStatement>? configure = null)
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

    public CSharpInterfaceMethod AddOptionalCancellationTokenParameter(string? parameterName) =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: parameterName ?? "cancellationToken",
            configure: parameter => parameter.WithDefaultValue("default"));

    public CSharpInterfaceMethod WithReturnType(CSharpType returnType)
    {
        ReturnTypeInfo = returnType;

        return this;
    }

    public CSharpInterfaceMethod Abstract() => this;

    public override string GetText(string indentation)
    {
        var @static = IsStatic
            ? $"static {(IsAbstract ? "abstract " : string.Empty)}"
            : string.Empty;

        var declaration = $"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{@static}{ReturnTypeInfo} {Name}{GetGenericParameters()}({string.Join(", ", Parameters.Select(x => x.ToString()))}){GetGenericTypeConstraints(indentation)}";
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
            return $"{declaration} => {expressionBody};";
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

    bool IHasCSharpStatementsActual.IsCodeBlock => true;

    #region ICSharpInterfaceMethod implementation

    ICSharpInterfaceMethodDeclaration ICSharpDeclaration<ICSharpInterfaceMethodDeclaration>.AddAttribute(string name, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(name, configure);
    ICSharpInterfaceMethodDeclaration ICSharpDeclaration<ICSharpInterfaceMethodDeclaration>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(attribute, configure);
    ICSharpInterfaceMethodDeclaration ICSharpDeclaration<ICSharpInterfaceMethodDeclaration>.WithComments(string xmlComments) => _wrapper.WithComments(xmlComments);
    ICSharpInterfaceMethodDeclaration ICSharpDeclaration<ICSharpInterfaceMethodDeclaration>.WithComments(IEnumerable<string> xmlComments) => _wrapper.WithComments(xmlComments);
    ICSharpInterface ICSharpInterfaceMethodDeclaration.Interface => _wrapper.Interface;
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddGenericParameter(string typeName, out ICSharpGenericParameter param) => _wrapper.AddGenericParameter(typeName, out param);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddGenericParameter(string typeName) => _wrapper.AddGenericParameter(typeName);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure) => _wrapper.AddGenericTypeConstraint(genericParameterName, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddOptionalCancellationTokenParameter(string? parameterName) => _wrapper.AddOptionalCancellationTokenParameter(parameterName);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(type, name, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(type, model, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(model, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameters(models, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatement(string statement, Action<ICSharpStatement>? configure) => _wrapper.AddStatement(statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => _wrapper.AddStatement(statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.Async(bool asValueTask) => _wrapper.Async(asValueTask);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith) => _wrapper.FindAndReplaceStatement(matchFunc, replaceWith);
    IList<ICSharpGenericParameter> ICSharpMethod<ICSharpInterfaceMethodDeclaration>.GenericParameters => _wrapper.GenericParameters;
    IList<ICSharpGenericTypeConstraint> ICSharpMethod<ICSharpInterfaceMethodDeclaration>.GenericTypeConstraints => _wrapper.GenericTypeConstraints;
    bool ICSharpMethod<ICSharpInterfaceMethodDeclaration>.HasExpressionBody => _wrapper.HasExpressionBody;
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure) => _wrapper.InsertParameter(index, type, name, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure) => _wrapper.InsertStatement(index, statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.InsertStatements(index, statements, configure);
    bool ICSharpMethod<ICSharpInterfaceMethodDeclaration>.IsAsync => _wrapper.IsAsync;
    bool ICSharpMethod<ICSharpInterfaceMethodDeclaration>.IsStatic => _wrapper.IsStatic;
    void ICSharpMethod<ICSharpInterfaceMethodDeclaration>.RemoveStatement(ICSharpStatement statement) => _wrapper.RemoveStatement(statement);
    ICSharpExpression ICSharpMethod<ICSharpInterfaceMethodDeclaration>.ReturnType => _wrapper.ReturnType;
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.Static() => _wrapper.Static();
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.Sync() => _wrapper.Sync();
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithExpressionBody(string statement, Action<ICSharpStatement>? configure) => _wrapper.WithExpressionBody(statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => _wrapper.WithExpressionBody(statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithReturnType(ICSharpType returnType) => _wrapper.WithReturnType(returnType);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithReturnType(string returnType) => _wrapper.WithReturnType(returnType);
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Abstract() => _wrapper.Abstract();
    string? ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.ExplicitImplementationFor => _wrapper.ExplicitImplementationFor;
    bool ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.IsAbstract => _wrapper.IsAbstract;
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.IsExplicitImplementationFor(string @interface) => _wrapper.IsExplicitImplementationFor(@interface);
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.New() => _wrapper.New();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Override() => _wrapper.Override();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Private() => _wrapper.Private();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Protected() => _wrapper.Protected();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Virtual() => _wrapper.Virtual();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.WithoutAccessModifier() => _wrapper.WithoutAccessModifier();
    string IHasCSharpName.Name => _wrapper.Name;
    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => _wrapper.Statements;
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => _wrapper.Parameters;

    #endregion
}