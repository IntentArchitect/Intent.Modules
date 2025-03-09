#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;
using static Intent.Modules.Common.CSharp.Settings.CSharpStyleConfiguration;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpClassMethod : CSharpMember<CSharpClassMethod>, ICSharpClassMethodDeclaration, ICSharpMethodDeclaration
{
    private readonly ICSharpClassMethodDeclaration _wrapper;
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public bool IsAsync { get; private set; }
    public bool IsOperator { get; set; }
    internal string AccessModifier { get; private set; } = "public ";
    protected string OverrideModifier { get; private set; } = string.Empty;
    public bool IsAbstract { get; private set; }
    public bool IsStatic => OverrideModifier.Contains("static");
    public bool HasExpressionBody { get; private set; }
    public string? ReturnType => ReturnTypeInfo.ToString();
    public CSharpType ReturnTypeInfo { get; private set; }
    ICSharpExpression ICSharpMethodDeclaration.ReturnType => new CSharpStatement(ReturnType);
    public string Name { get; }
    public List<CSharpParameter> Parameters { get; } = new();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();
    public string? ExplicitImplementationFor { get; private set; }
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => Parameters;
    public CSharpClass Class { get; }

    /// <summary>
    /// Use <see cref="CSharpClassMethod(string,string,ICSharpCodeContext)"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpClassMethod(string returnType, string name, CSharpClass @class)
        : this(returnType, name, (ICSharpCodeContext)@class)
    {
    }

    public CSharpClassMethod(string returnType, string name, ICSharpCodeContext @class)
        : this(CSharpTypeParser.Parse(returnType), name, (CSharpClass)@class)
    {
    }

    public CSharpClassMethod(CSharpType returnType, string name, CSharpClass @class)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapper = new CSharpClassMethodWrapper(this);
        Parent = @class;
        Class = @class;
        File = @class.File;
        ReturnTypeInfo = returnType ?? throw new ArgumentException("Cannot be null", nameof(returnType));
        Name = name;
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public CSharpClassMethod IsExplicitImplementationFor(string? @interface)
    {
        ExplicitImplementationFor = @interface;
        return this;
    }

    /// <summary>
    /// Resolves the parameter name from the <paramref name="model"/>. Registers this parameter as the representative of the <paramref name="model"/>.
    /// </summary>
    public CSharpClassMethod AddParameter<TModel>(string type, TModel model, Action<CSharpParameter>? configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        return AddParameter(type, model.Name.ToParameterName(), param =>
        {
            param.RepresentsModel(model);
            configure?.Invoke(param);
        });
    }

    /// <summary>
    /// Resolves the type name and parameter name from the <paramref name="model"/> using the <see cref="ICSharpFileBuilderTemplate"/>
    /// template that was passed into the <see cref="CSharpFile"/>. Registers this parameter as representative of the <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public CSharpClassMethod AddParameter<TModel>(TModel model, Action<CSharpParameter>? configure = null) where TModel
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
    public CSharpClassMethod AddParameters<TModel>(IEnumerable<TModel> models, Action<CSharpParameter>? configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        foreach (var model in models)
        {
            AddParameter(model, configure);
        }

        return this;
    }

    public CSharpClassMethod AddParameter(string type, string name, Action<CSharpParameter>? configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpClassMethod InsertParameter(int index, string type, string name, Action<CSharpParameter>? configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Insert(index, param);
        configure?.Invoke(param);
        return this;
    }

    /// <summary>
    /// Use <see cref="AddOptionalCancellationTokenParameter()"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpClassMethod AddOptionalCancellationTokenParameter<T>(CSharpTemplateBase<T> template) =>
        AddParameter(
            $"{template.UseType("System.Threading.CancellationToken")}", "cancellationToken",
            parameter => parameter.WithDefaultValue("default"));

    public CSharpClassMethod AddOptionalCancellationTokenParameter() =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: "cancellationToken",
            configure: parameter => parameter.WithDefaultValue("default"));

    public CSharpClassMethod AddOptionalCancellationTokenParameter(string? parameterName) =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: parameterName ?? "cancellationToken",
            configure: parameter => parameter.WithDefaultValue("default"));

    public CSharpClassMethod AddGenericParameter(string typeName)
    {
        var param = new CSharpGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }

    public CSharpClassMethod AddGenericParameter(string typeName, out CSharpGenericParameter param)
    {
        param = new CSharpGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }

    public CSharpClassMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint>? configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure?.Invoke(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    public CSharpClassMethod AddStatement(string statement, Action<CSharpStatement>? configure = null)
    {
        return AddStatement(new CSharpStatement(statement), configure);
    }

    public CSharpClassMethod AddStatement(CSharpStatement statement, Action<CSharpStatement>? configure = null)
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpClassMethod InsertStatement(int index, CSharpStatement statement, Action<CSharpStatement>? configure = null)
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpClassMethod InsertStatements(int index, IReadOnlyCollection<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>>? configure = null)
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }

        configure?.Invoke(statements);
        return this;
    }

    public CSharpClassMethod AddStatements(string statements, Action<IEnumerable<CSharpStatement>>? configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpClassMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>>? configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpClassMethod AddStatements(IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>>? configure = null)
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

    public CSharpClassMethod FindAndReplaceStatement(Func<CSharpStatement, bool> matchFunc, CSharpStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public CSharpClassMethod Operator(bool isOperator = true)
    {
        IsOperator = isOperator;
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

    public CSharpClassMethod WithReturnType(CSharpType returnType)
    {
        ReturnTypeInfo = returnType;

        return this;
    }

    public CSharpClassMethod WithReturnType(string returnType)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        ReturnTypeInfo = CSharpTypeParser.Parse(returnType);
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

    public CSharpClassMethod Abstract()
    {
        OverrideModifier = "abstract ";
        IsAbstract = true;
        return this;
    }

    public CSharpClassMethod Static()
    {
        OverrideModifier = "static ";
        return this;
    }

    public CSharpClassMethod Sync()
    {
        IsAsync = false;
        if (ReturnTypeInfo is CSharpTypeGeneric generic && generic.IsTask())
        {
            ReturnTypeInfo = generic.TypeArgumentList.Single();
        }
        return this;
    }

    /// <summary>
    /// Indicates that this method is async and sets the return type to a <see cref="System.Threading.Tasks.Task"/> or <see cref="System.Threading.Tasks.Task&lt;T&gt;"/>.
    /// </summary>
    public CSharpClassMethod Async() => Async(false);

    /// <summary>
    /// Indicates that this method is async and sets the return type to a
    /// <see cref="System.Threading.Tasks.Task"/> / <see cref="System.Threading.Tasks.ValueTask"/> or
    /// <see cref="System.Threading.Tasks.Task&lt;T&gt;"/> / <see cref="System.Threading.Tasks.ValueTask&lt;T&gt;"/>.
    /// </summary>
    /// <param name="asValueTask">If true it will use <see cref="System.Threading.Tasks.ValueTask"/> instead of <see cref="System.Threading.Tasks.Task"/>.</param>
    public CSharpClassMethod Async(bool asValueTask)
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

    public void RemoveStatement(CSharpStatement statement)
    {
        Statements.Remove(statement);
    }

    public CSharpClassMethod WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure)
        where TCSharpStatement : CSharpStatement
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

    public CSharpClassMethod WithExpressionBody(CSharpStatement statement) => WithExpressionBody(statement, null);

    public override string GetText(string indentation)
    {
        var declaration = new StringBuilder();

        declaration.Append(indentation);
        if (string.IsNullOrWhiteSpace(ExplicitImplementationFor))
        {
            declaration.Append(AccessModifier);
        }

        declaration.Append(OverrideModifier);
        declaration.Append(IsAsync ? "async " : "");
        declaration.Append(ReturnType);
        declaration.Append(' ');

        if (IsOperator)
        {
            declaration.Append("operator ");
        }

        if (!string.IsNullOrWhiteSpace(ExplicitImplementationFor))
        {
            declaration.Append(ExplicitImplementationFor);
            declaration.Append('.');
        }

        declaration.Append(Name);
        declaration.Append(GetGenericParameters());

        // calculate a rough estimate of the line length
        // +1 at the end to cater for backward compatibility where the check used to include the open brace in the length calc
        var estimatedLength = declaration.Length + Parameters.Sum(x => x.ToString().Length) + 1; 

        // this only done after the estimate length is calculated to get a more accurate reading
        declaration.Insert(0, GetAttributes(indentation));
        declaration.Insert(0, GetComments(indentation));

        declaration.Append($"({GetParameters(File.StyleSettings?.ParameterPlacement.AsEnum() ?? ParameterPlacementOptionsEnum.Default, 
            estimatedLength, 120, indentation)})");
        declaration.Append(GetGenericTypeConstraints(indentation));

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

    private string GetParameters(ParameterPlacementOptionsEnum option, int estimatedLength, int maxLineLength, string indentation) =>
        (option, estimatedLength > maxLineLength, Parameters.Count > 1) switch
        {
            // if there is only one parameter
            (_, _, false) or
            (ParameterPlacementOptionsEnum.Default, false, _) or
            (ParameterPlacementOptionsEnum.DependsOnLength, false, true) or
            // if do not modify, then return on one line
            (ParameterPlacementOptionsEnum.SameLine, _, _) => string.Join(", ", Parameters.Select(x => x.ToString())),
            (ParameterPlacementOptionsEnum.Default, true, true) or
            (ParameterPlacementOptionsEnum.DependsOnLength, true, true) or
            (_, _, _) => $@"
{indentation}    {string.Join($@",
{indentation}    ", Parameters.Select(x => x.ToString()))}",
            
        };
    
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

    #region ICSharpMethodDeclarationActual implementation

    ICSharpClassMethodDeclaration ICSharpDeclaration<ICSharpClassMethodDeclaration>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(attribute, configure);

    ICSharpClassMethodDeclaration ICSharpDeclaration<ICSharpClassMethodDeclaration>.AddAttribute(string name, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(name, configure);

    ICSharpClassMethodDeclaration ICSharpDeclaration<ICSharpClassMethodDeclaration>.WithComments(IEnumerable<string> xmlComments) => _wrapper.WithComments(xmlComments);

    ICSharpClassMethodDeclaration ICSharpDeclaration<ICSharpClassMethodDeclaration>.WithComments(string xmlComments) => _wrapper.WithComments(xmlComments);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddGenericParameter(string typeName, out ICSharpGenericParameter param) => _wrapper.AddGenericParameter(typeName, out param);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddGenericParameter(string typeName) => _wrapper.AddGenericParameter(typeName);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure) => _wrapper.AddGenericTypeConstraint(genericParameterName, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddOptionalCancellationTokenParameter(string? parameterName) => _wrapper.AddOptionalCancellationTokenParameter(parameterName);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure) => AddParameter(type, name, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(type, model, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(model, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameters(models, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatement(string statement, Action<ICSharpStatement>? configure) => _wrapper.AddStatement(statement, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => _wrapper.AddStatement(statement, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.Async(bool asValueTask) => _wrapper.Async(asValueTask);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith) => _wrapper.FindAndReplaceStatement(matchFunc, replaceWith);

    IList<ICSharpGenericParameter> ICSharpMethod<ICSharpClassMethodDeclaration>.GenericParameters => _wrapper.GenericParameters;

    IList<ICSharpGenericTypeConstraint> ICSharpMethod<ICSharpClassMethodDeclaration>.GenericTypeConstraints => _wrapper.GenericTypeConstraints;

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure) => _wrapper.InsertParameter(index, type, name, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure) => _wrapper.InsertStatement(index, statement, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.InsertStatements(index, statements, configure);

    void ICSharpMethod<ICSharpClassMethodDeclaration>.RemoveStatement(ICSharpStatement statement) => _wrapper.RemoveStatement(statement);

    ICSharpExpression ICSharpMethod<ICSharpClassMethodDeclaration>.ReturnType => _wrapper.ReturnType;

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.Static() => _wrapper.Static();

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.Sync() => _wrapper.Sync();

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithExpressionBody(string statement, Action<ICSharpStatement>? configure) => _wrapper.WithExpressionBody(statement, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => _wrapper.WithExpressionBody(statement, configure);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithReturnType(ICSharpType returnType) => _wrapper.WithReturnType(returnType);

    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithReturnType(string returnType) => _wrapper.WithReturnType(returnType);

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Abstract() => _wrapper.Abstract();

    ICSharpClass ICSharpClassMethodDeclaration.Class => _wrapper.Class;

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.IsExplicitImplementationFor(string @interface) => _wrapper.IsExplicitImplementationFor(@interface);

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.New() => _wrapper.New();

    ICSharpClassMethodDeclaration ICSharpClassMethodDeclaration.Operator(bool isOperator) => _wrapper.Operator(isOperator);

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Override() => _wrapper.Override();

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Private() => _wrapper.Private();

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Protected() => _wrapper.Protected();

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Virtual() => _wrapper.Virtual();

    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.WithoutAccessModifier() => _wrapper.WithoutAccessModifier();

    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => _wrapper.Statements;

    #endregion
}