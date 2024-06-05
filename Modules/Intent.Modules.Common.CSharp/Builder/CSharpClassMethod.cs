using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpMethodDeclaration : IHasICSharpParameters, ICSharpReferenceable, IHasCSharpStatements
{
    bool IsAsync { get; }
    public ICSharpExpression ReturnType { get; }
}

public class CSharpClassMethod : CSharpMember<CSharpClassMethod>, ICSharpMethodDeclaration
{
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public bool IsAsync { get; private set; } = false;
    protected string AccessModifier { get; private set; } = "public ";
    protected string OverrideModifier { get; private set; } = string.Empty;
    public bool IsAbstract { get; private set; }
    public bool HasExpressionBody { get; private set; }
    public string ReturnType { get; private set; }
    ICSharpExpression ICSharpMethodDeclaration.ReturnType => new CSharpStatement(ReturnType);
    public string Name { get; }
    public List<CSharpParameter> Parameters { get; } = new();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();
    public string ExplicitImplementationFor { get; private set; }
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => this.Parameters;
    public CSharpClass Class { get; }

    /// <summary>
    /// Use <see cref="CSharpClassMethod(string,string,ICSharpCodeContext)"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpClassMethod(string returnType, string name, CSharpClass @class) : this(returnType, name, (ICSharpCodeContext)@class) { }

    public CSharpClassMethod(string returnType, string name, ICSharpCodeContext @class)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Parent = @class;
        Class = @class as CSharpClass;
        File = @class?.File;
        ReturnType = returnType;
        Name = name;
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public CSharpClassMethod IsExplicitImplementationFor(string @interface)
    {
        ExplicitImplementationFor = @interface;
        return this;
    }


    /// <summary>
    /// Resolves the parameter name from the <paramref name="model"/>. Registers this parameter as the representative of the <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public CSharpClassMethod AddParameter<TModel>(string type, TModel model, Action<CSharpParameter> configure = null) where TModel
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
    public CSharpClassMethod AddParameter<TModel>(TModel model, Action<CSharpParameter> configure = null) where TModel
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
    public CSharpClassMethod AddParameters<TModel>(IEnumerable<TModel> models, Action<CSharpParameter> configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        foreach (var model in models)
        {
            AddParameter(model, configure);
        }
        return this;
    }

    public CSharpClassMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpClassMethod InsertParameter(int index, string type, string name, Action<CSharpParameter> configure = null)
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

    public CSharpClassMethod AddOptionalCancellationTokenParameter(string parameterName) =>
        AddParameter(
            type: File.Template.UseType("System.Threading.CancellationToken"),
            name: parameterName,
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

    public CSharpClassMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure(param);
        GenericTypeConstraints.Add(param);
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

    public CSharpClassMethod InsertStatements(int index, IReadOnlyCollection<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
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

    public CSharpClassMethod FindAndReplaceStatement(Func<CSharpStatement, bool> matchFunc, CSharpStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
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

    public CSharpClassMethod WithReturnType(string returnType)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        ReturnType = returnType;
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
        if (ReturnType.StartsWith("Task"))
        {
            ReturnType = ReturnType == "Task" ? "void" : StripTask();
        }
        return this;
    }

    private string StripTask()
    {
        //Task<X> => X
        return ReturnType.Substring(5, ReturnType.Length - 6);
    }

    public CSharpClassMethod Async()
    {
        IsAsync = true;
        var taskType = File.Template?.UseType("System.Threading.Tasks.Task") ?? "Task";
        if (!ReturnType.StartsWith(taskType))
        {
            if (taskType == "System.Threading.Tasks.Task" && ReturnType.StartsWith("Task<"))
            {
                ReturnType = "System.Threading.Tasks." + ReturnType;
            }
            else
            {
                ReturnType = ReturnType == "void" ? taskType : $"{taskType}<{ReturnType}>";
            }
        }
        return this;
    }

    public void RemoveStatement(CSharpStatement statement)
    {
        Statements.Remove(statement);
    }

    public CSharpClassMethod WithExpressionBody(CSharpStatement statement)
    {
        HasExpressionBody = true;
        statement.BeforeSeparator = CSharpCodeSeparatorType.None;
        if (statement is CSharpMethodChainStatement stmt)
        {
            stmt.WithoutSemicolon();
        }
        Statements.Add(statement);
        return this;
    }

    public override string GetText(string indentation)
    {
        var declaration = new StringBuilder();

        declaration.Append(GetComments(indentation));
        declaration.Append(GetAttributes(indentation));
        declaration.Append(indentation);

        if (string.IsNullOrWhiteSpace(ExplicitImplementationFor))
        {
            declaration.Append(AccessModifier);
        }

        declaration.Append(OverrideModifier);
        declaration.Append(IsAsync ? "async " : "");
        declaration.Append(ReturnType);
        declaration.Append(' ');

        if (!string.IsNullOrWhiteSpace(ExplicitImplementationFor))
        {
            declaration.Append(ExplicitImplementationFor);
            declaration.Append('.');
        }

        declaration.Append(Name);
        declaration.Append(GetGenericParameters());
        declaration.Append($"({GetParameters(indentation)})");
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

    private string GetParameters(string indentation)
    {
        // GCB - WTF: why rewrite out whole statement
        if (Parameters.Count > 1 && $"{indentation}{AccessModifier}{OverrideModifier}{(IsAsync ? "async " : "")}{ReturnType} {Name}{GetGenericParameters()}(".Length + Parameters.Sum(x => x.ToString().Length) > 120)
        {
            return $@"
{indentation}    {string.Join($@",
{indentation}    ", Parameters.Select(x => x.ToString()))}";
        }
        else
        {
            return string.Join(", ", Parameters.Select(x => x.ToString()));
        }
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
