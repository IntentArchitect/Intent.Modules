#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using static Intent.Modules.Common.CSharp.Settings.CSharpStyleConfiguration;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpLocalMethod : CSharpStatement, IHasCSharpStatements, ICSharpLocalFunction
{
    private readonly ICSharpLocalFunction _wrapper;

    public CSharpLocalMethod(string returnType, string name, CSharpFile file)
        : this(returnType, name, (ICSharpFile)file)
    {
    }

    public CSharpLocalMethod(string returnType, string name, ICSharpFile file)
        : this(CSharpTypeParser.Parse(returnType), name, file)
    {
    }

    public CSharpLocalMethod(CSharpType returnType, string name, ICSharpFile file) : base(string.Empty)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapper = new CSharpLocalMethodWrapper(this);

        Name = name;
        File = file;
        ReturnTypeInfo = returnType ?? throw new ArgumentException("Cannot be null", nameof(returnType));

        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public bool IsAsync { get; private set; }
    public bool IsStatic { get; private set; }
    public bool HasExpressionBody { get; private set; }
    public string? ReturnType => ReturnTypeInfo.ToString();
    public ICSharpType ReturnTypeInfo { get; private set; }
    public string Name { get; }
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();
    public CSharpLocalMethod AddParameter(string type, string name, Action<CSharpParameter>? configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    /// <summary>
    /// Resolves the parameter name from the <paramref name="model"/>. Registers this parameter as the representative of the <paramref name="model"/>.
    /// </summary>
    public CSharpLocalMethod AddParameter<TModel>(string type, TModel model, Action<CSharpParameter>? configure = null) where TModel
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
    public CSharpLocalMethod AddParameter<TModel>(TModel model, Action<CSharpParameter>? configure = null) where TModel
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
    public CSharpLocalMethod AddParameters<TModel>(IEnumerable<TModel> models, Action<CSharpParameter>? configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        foreach (var model in models)
        {
            AddParameter(model, configure);
        }

        return this;
    }

    /// <summary>
    /// Use <see cref="AddOptionalCancellationTokenParameter(string)"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpLocalMethod AddOptionalCancellationTokenParameter<T>(CSharpTemplateBase<T> template) =>
        AddParameter(
            $"{template.UseType("System.Threading.CancellationToken")}", "cancellationToken",
            parameter => parameter.WithDefaultValue("default"));

    public CSharpLocalMethod AddOptionalCancellationTokenParameter() =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: "cancellationToken",
            configure: parameter => parameter.WithDefaultValue("default"));

    public CSharpLocalMethod AddOptionalCancellationTokenParameter(string? parameterName) =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: parameterName ?? "cancellationToken",
            configure: parameter => parameter.WithDefaultValue("default"));

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

    public CSharpLocalMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint>? configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure?.Invoke(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    public CSharpLocalMethod AddStatement(string statement, Action<CSharpStatement>? configure = null)
    {
        return AddStatement(new CSharpStatement(statement), configure);
    }

    public CSharpLocalMethod AddStatement<T>(T statement, Action<T>? configure = null)
        where T : CSharpStatement
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpLocalMethod InsertStatement<T>(int index, T statement, Action<T>? configure = null)
        where T : CSharpStatement
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpLocalMethod InsertStatements<T>(int index, IReadOnlyCollection<T> statements, Action<IEnumerable<T>>? configure = null)
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

    public CSharpLocalMethod AddStatements(string statements, Action<IEnumerable<CSharpStatement>>? configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpLocalMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>>? configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpLocalMethod AddStatements<T>(IEnumerable<T> statements, Action<IEnumerable<T>>? configure = null)
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

    public CSharpLocalMethod WithReturnType(ICSharpType returnType)
    {
        ReturnTypeInfo = returnType;

        return this;
    }

    public CSharpLocalMethod WithReturnType(string returnType) => WithReturnType(CSharpTypeParser.Parse(returnType));

    public CSharpLocalMethod Static()
    {
        IsStatic = true;
        return this;
    }

    public CSharpLocalMethod Sync()
    {
        IsAsync = false;
        if (ReturnTypeInfo is CSharpTypeGeneric generic && generic.IsTask())
        {
            ReturnTypeInfo = generic.TypeArgumentList.Single();
        }
        return this;
    }

    public void RemoveStatement(CSharpStatement statement)
    {
        Statements.Remove(statement);
    }

    public CSharpLocalMethod WithExpressionBody<T>(T statement, Action<T>? configure = null)
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

    public CSharpLocalMethod Async() => Async(false);

    public CSharpLocalMethod Async(bool asValueTask)
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
                    ReturnTypeInfo = CSharpType.CreateValueTask((CSharpType)genericType, File.Template);
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
                    ReturnTypeInfo = CSharpType.CreateTask((CSharpType)genericType, File.Template);
                }
            }
            else if (!ReturnTypeInfo.IsTask())
            {
                ReturnTypeInfo = ReturnTypeInfo.WrapInTask(File.Template);
            }
        }

        return this;
    }

    public CSharpLocalMethod InsertParameter(int index, string type, string name, Action<CSharpParameter>? configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Insert(index, param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpLocalMethod InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure = null)
    {
        Statements.Insert(index, (CSharpStatement)statement);
        statement.Parent = this;
        configure?.Invoke(statement);
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

        sb.Append(ReturnTypeInfo);
        sb.Append(' ');
        sb.Append(Name);

        if (GenericParameters.Any())
        {
            sb.Append("<");
            sb.Append(string.Join(", ", GenericParameters));
            sb.Append(">");
        }

        var estimatedLength = sb.Length + Parameters.Sum(x => x.ToString()!.Length);
        var parameterList = GetParameters(File?.StyleSettings?.ParameterPlacement.AsEnum() ?? ParameterPlacementOptionsEnum.Default, estimatedLength, 120, indentation);
             
        sb.Append($"({parameterList})");

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

    //    private string GetParameters(string indentation, int currentLineLength)
    //    {
    //        if (Parameters.Count > 1 && currentLineLength + Parameters.Sum(x => x.ToString()!.Length) > 120)
    //        {
    //            return $@"
    //{indentation}    {string.Join($@",
    //{indentation}    ", Parameters.Select(x => x.ToString()))}";
    //        }

    //        return string.Join(", ", Parameters.Select(x => x.ToString()));
    //    }

    public override string ToString()
    {
        return GetText(indentation: string.Empty);
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => false;

    #region ICSharpLocalFunction implementation

    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddGenericParameter(string typeName, out ICSharpGenericParameter param) => _wrapper.AddGenericParameter(typeName, out param);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddGenericParameter(string typeName) => _wrapper.AddGenericParameter(typeName);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure) => _wrapper.AddGenericTypeConstraint(genericParameterName, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddOptionalCancellationTokenParameter(string? parameterName) => _wrapper.AddOptionalCancellationTokenParameter(parameterName);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(type, name, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(type, model, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameter(model, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure) => _wrapper.AddParameters(models, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatement(string statement, Action<ICSharpStatement>? configure) => _wrapper.AddStatement(statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => _wrapper.AddStatement(statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure) => _wrapper.AddStatements(statements, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.Async(bool asValueTask) => _wrapper.Async(asValueTask);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith) => _wrapper.FindAndReplaceStatement(matchFunc, replaceWith);
    IList<ICSharpGenericParameter> ICSharpMethod<ICSharpLocalFunction>.GenericParameters => _wrapper.GenericParameters;
    IList<ICSharpGenericTypeConstraint> ICSharpMethod<ICSharpLocalFunction>.GenericTypeConstraints => _wrapper.GenericTypeConstraints;
    bool ICSharpMethod<ICSharpLocalFunction>.HasExpressionBody => _wrapper.HasExpressionBody;
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure) => _wrapper.InsertParameter(index, type, name, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure) => _wrapper.InsertStatement(index, statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure) => _wrapper.InsertStatements(index, statements, configure);
    bool ICSharpMethod<ICSharpLocalFunction>.IsAsync => _wrapper.IsAsync;
    bool ICSharpMethod<ICSharpLocalFunction>.IsStatic => _wrapper.IsStatic;
    void ICSharpMethod<ICSharpLocalFunction>.RemoveStatement(ICSharpStatement statement) => _wrapper.RemoveStatement(statement);
    ICSharpExpression ICSharpMethod<ICSharpLocalFunction>.ReturnType => _wrapper.ReturnType;
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.Static() => _wrapper.Static();
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.Sync() => _wrapper.Sync();
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithExpressionBody(string statement, Action<ICSharpStatement>? configure) => _wrapper.WithExpressionBody(statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => _wrapper.WithExpressionBody(statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithReturnType(ICSharpType returnType) => _wrapper.WithReturnType(returnType);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithReturnType(string returnType) => _wrapper.WithReturnType(returnType);
    string IHasCSharpName.Name => _wrapper.Name;
    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => _wrapper.Statements;
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => _wrapper.Parameters;

    #endregion
}