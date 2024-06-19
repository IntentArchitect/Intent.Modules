using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructor : CSharpMember<CSharpConstructor>, ICSharpConstructor, IHasCSharpStatements
{
    private readonly ICSharpConstructor _wrapper;
    // This class will be used for "static", "instance" and "primary" constructors.
    //https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/instance-constructors
    
    public CSharpClass Class { get; }
    public string AccessModifier { get; private set; } = "public ";
    public CSharpConstructorCall ConstructorCall { get; private set; }
    public IList<CSharpConstructorParameter> Parameters { get; } = new List<CSharpConstructorParameter>();
    public List<CSharpStatement> Statements { get; } = new();
    public string Name => Class.Name;
    public bool IsPrimaryConstructor { get; private set; }
    IList<CSharpStatement> IHasCSharpStatements.Statements => this.Statements;
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => this.Parameters;

    public CSharpConstructor(CSharpClass @class) : this(@class, false)
    {
    }

    public CSharpConstructor(CSharpClass @class, bool isPrimaryConstructor)
    {
        _wrapper = new CSharpConstructorWrapper(this);
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        Parent = @class;
        Class = @class;
        File = @class.File;
        IsPrimaryConstructor = isPrimaryConstructor;
    }

    /// <summary>
    /// Resolves the parameter name from the <paramref name="model"/>. Registers this parameter as the representative of the <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public CSharpConstructor AddParameter<TModel>(string type, TModel model, Action<CSharpConstructorParameter> configure = null) where TModel
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
    public CSharpConstructor AddParameter<TModel>(TModel model, Action<CSharpConstructorParameter> configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        return AddParameter(File.GetModelType(model), model.Name.ToParameterName(), param =>
        {
            param.RepresentsModel(model);
            configure?.Invoke(param);
        });
    }

    public CSharpConstructor AddParameters<TModel>(IEnumerable<TModel> models, Action<CSharpConstructorParameter> configure = null) where TModel
        : IMetadataModel, IHasName, IHasTypeReference
    {
        foreach (var model in models)
        {
            AddParameter(model, configure);
        }
        return this;
    }

    public CSharpConstructor AddParameter(string type, string name, Action<CSharpConstructorParameter> configure = null)
    {
        var param = new CSharpConstructorParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        IntroduceBackingMembersIfForPrimaryConstructor(param);
        return this;
    }

    public CSharpConstructor InsertParameter(int index, string type, string name, Action<CSharpConstructorParameter> configure = null)
    {
        var param = new CSharpConstructorParameter(type, name, this);
        Parameters.Insert(index, param);
        configure?.Invoke(param);
        IntroduceBackingMembersIfForPrimaryConstructor(param);
        return this;
    }

    public CSharpConstructor AddStatement(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        statement.Parent = this;
        Statements.Add(statement);
        configure?.Invoke(statement);
        return this;
    }

    public CSharpConstructor InsertStatement(int index, string statement, Action<CSharpStatement> configure = null)
    {
        var s = new CSharpStatement(statement);
        s.Parent = this;
        Statements.Insert(index, s);
        configure?.Invoke(s);
        return this;
    }

    public CSharpConstructor AddStatements(string statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpConstructor AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpConstructor AddStatements(IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            statement.Parent = this;
            Statements.Add(statement);
        }
        configure?.Invoke(arrayed);

        return this;
    }

    public CSharpConstructor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    
    public CSharpConstructor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpConstructor Static()
    {
        AccessModifier = "static ";
        return this;
    }

    public CSharpConstructor CallsBase(Action<CSharpConstructorCall> configure = null)
    {
        ConstructorCall = CSharpConstructorCall.Base();
        configure?.Invoke(ConstructorCall);
        return this;
    }

    public CSharpConstructor CallsThis(Action<CSharpConstructorCall> configure = null)
    {
        if (IsPrimaryConstructor)
        {
            throw new InvalidOperationException($"Cannot supply 'this' constructor call to a primary constructor: {Class.TypeDefinitionType} {Class.Name}.");
        }
        ConstructorCall = CSharpConstructorCall.This();
        configure?.Invoke(ConstructorCall);
        return this;
    }
    
    public override string GetText(string indentation)
    {
        if (IsPrimaryConstructor)
        {
            return Parameters.Count == 0 ? string.Empty : $"({ToStringParameters(indentation)})";
        }
        
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{Class.Name}({ToStringParameters(indentation)}){ConstructorCall?.ToString() ?? string.Empty}
{indentation}{{{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }

    private void IntroduceBackingMembersIfForPrimaryConstructor(CSharpConstructorParameter param)
    {
        if (!IsPrimaryConstructor)
        {
            return;
        }

        switch (Class.TypeDefinitionType)
        {
            case CSharpClass.Type.Class:
                var field = CSharpField.CreateFieldOmittedFromRender(param.Type, param.Name, Class, null);
                Class.Fields.Add(field);
                break;
            case CSharpClass.Type.Record:
                var property = CSharpProperty.CreatePropertyOmittedFromRender(param.Type, param.Name, Class);
                Class.Properties.Add(property);
                break;
        }
    }

    private string ToStringParameters(string indentation)
    {
        // This length is more or less the same for an instance and primary ctor declaration on a single line
        var estimatedLength = $"{indentation}{AccessModifier}{Class.Name}(".Length + Parameters.Sum(x => x.ToString().Length);
        
        const int maxLineLength = 120;
        
        if (Parameters.Count > 1 && estimatedLength > maxLineLength)
        {
            return string.Join($@",
{indentation}    ", Parameters.Select(x => x.ToString()));
        }
        else
        {
            return string.Join(", ", Parameters.Select(x => x.ToString()));
        }
    }

    #region ICSharpConstructor implementation

    ICSharpClass ICSharpConstructor.Class => _wrapper.Class;

    ICSharpConstructorCall ICSharpConstructor.ConstructorCall => _wrapper.ConstructorCall;

    ICSharpConstructor ICSharpConstructor.AddParameter<TModel>(string type, TModel model, Action<ICSharpConstructorParameter> configure) => _wrapper.AddParameter(type, model, configure);

    ICSharpConstructor ICSharpConstructor.AddParameter<TModel>(TModel model, Action<ICSharpConstructorParameter> configure) => _wrapper.AddParameter(model, configure);

    ICSharpConstructor ICSharpConstructor.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpConstructorParameter> configure) => _wrapper.AddParameters(models, configure);

    ICSharpConstructor ICSharpConstructor.AddParameter(string type, string name, Action<ICSharpConstructorParameter> configure) => _wrapper.AddParameter(type, name, configure);

    ICSharpConstructor ICSharpConstructor.InsertParameter(int index, string type, string name, Action<ICSharpConstructorParameter> configure) => _wrapper.InsertParameter(index, type, name, configure);

    ICSharpConstructor ICSharpConstructor.AddStatement(ICSharpStatement statement, Action<ICSharpStatement> configure) => _wrapper.AddStatement(statement, configure);

    ICSharpConstructor ICSharpConstructor.InsertStatement(int index, string statement, Action<ICSharpStatement> configure) => _wrapper.InsertStatement(index, statement, configure);

    ICSharpConstructor ICSharpConstructor.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>> configure) => _wrapper.AddStatements(statements, configure);

    ICSharpConstructor ICSharpConstructor.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>> configure) => _wrapper.AddStatements(statements, configure);

    ICSharpConstructor ICSharpConstructor.AddStatements(IEnumerable<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>> configure) => _wrapper.AddStatements(statements, configure);

    ICSharpConstructor ICSharpConstructor.Protected() => _wrapper.Protected();

    ICSharpConstructor ICSharpConstructor.Private() => _wrapper.Private();

    ICSharpConstructor ICSharpConstructor.Static() => _wrapper.Static();

    ICSharpConstructor ICSharpConstructor.CallsBase(Action<ICSharpConstructorCall> configure) => _wrapper.CallsBase(configure);

    ICSharpConstructor ICSharpConstructor.CallsThis(Action<ICSharpConstructorCall> configure) => _wrapper.CallsThis(configure);

    ICSharpConstructor ICSharpDeclaration<ICSharpConstructor>.AddAttribute(string name, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(name, configure);

    ICSharpConstructor ICSharpDeclaration<ICSharpConstructor>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(attribute, configure);

    ICSharpConstructor ICSharpDeclaration<ICSharpConstructor>.WithComments(string xmlComments) => _wrapper.WithComments(xmlComments);

    ICSharpConstructor ICSharpDeclaration<ICSharpConstructor>.WithComments(IEnumerable<string> xmlComments) => _wrapper.WithComments(xmlComments);

    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => ((IHasCSharpStatementsActual)_wrapper).Statements;

    #endregion
}