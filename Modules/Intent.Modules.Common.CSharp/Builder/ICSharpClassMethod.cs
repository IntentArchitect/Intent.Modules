#nullable enable
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpClassMethod : ICSharpMember<ICSharpClassMethod>, ICSharpMethodDeclarationActual
{
    bool IsAbstract { get; }
    bool HasExpressionBody { get; }
    IList<ICSharpGenericParameter> GenericParameters { get; }
    IList<ICSharpGenericTypeConstraint> GenericTypeConstraints { get; }
    string ExplicitImplementationFor { get; }
    ICSharpClass Class { get; }
    ICSharpClassMethod IsExplicitImplementationFor(string @interface);
    ICSharpClassMethod AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure = null);
    ICSharpClassMethod AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure = null)
        where TModel : IMetadataModel, IHasName, IHasTypeReference;
    ICSharpClassMethod AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure = null)
        where TModel : IMetadataModel, IHasName, IHasTypeReference;
    ICSharpClassMethod AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure = null)
        where TModel : IMetadataModel, IHasName, IHasTypeReference;
    ICSharpClassMethod InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure = null);
    ICSharpClassMethod AddOptionalCancellationTokenParameter();
    ICSharpClassMethod AddOptionalCancellationTokenParameter(string parameterName);
    ICSharpClassMethod AddGenericParameter(string typeName);
    ICSharpClassMethod AddGenericParameter(string typeName, out ICSharpGenericParameter param);
    ICSharpClassMethod AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure);
    ICSharpClassMethod AddStatement(string statement, Action<ICSharpStatement>? configure = null);
    ICSharpClassMethod AddStatement(ICSharpStatement statement, Action<ICSharpStatement>? configure = null);
    ICSharpClassMethod InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure = null);
    ICSharpClassMethod InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure = null);
    ICSharpClassMethod AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure = null);
    ICSharpClassMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure = null);
    ICSharpClassMethod AddStatements(IEnumerable<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure = null);
    ICSharpClassMethod FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith);
    ICSharpClassMethod Protected();
    ICSharpClassMethod Private();
    ICSharpClassMethod WithReturnType(string returnType);
    ICSharpClassMethod WithoutAccessModifier();
    ICSharpClassMethod Override();
    ICSharpClassMethod New();
    ICSharpClassMethod Virtual();
    ICSharpClassMethod Abstract();
    ICSharpClassMethod Static();
    ICSharpClassMethod Sync();
    ICSharpClassMethod Async();
    ICSharpClassMethod WithExpressionBody(ICSharpStatement statement);
    void RemoveStatement(ICSharpStatement statement);
}