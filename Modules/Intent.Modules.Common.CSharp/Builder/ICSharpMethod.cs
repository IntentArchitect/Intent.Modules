#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpMethod<out TCSharpMethod> : IHasICSharpParameters, IHasCSharpStatementsActual, IHasCSharpName, ICSharpCodeContext
    where TCSharpMethod : ICSharpMethod<TCSharpMethod>
{
    TCSharpMethod AddGenericParameter(string typeName, out ICSharpGenericParameter param);
    TCSharpMethod AddGenericParameter(string typeName);
    TCSharpMethod AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure = null);
    TCSharpMethod AddOptionalCancellationTokenParameter(string? parameterName = null);
    TCSharpMethod AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure = null);
    TCSharpMethod AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure = null)
        where TModel : IMetadataModel, IHasName, IHasTypeReference;
    TCSharpMethod AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure = null)
        where TModel : IMetadataModel, IHasName, IHasTypeReference;
    TCSharpMethod AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure = null)
        where TModel : IMetadataModel, IHasName, IHasTypeReference;
    TCSharpMethod AddStatement(string statement, Action<ICSharpStatement>? configure = null);
    TCSharpMethod AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure = null)
        where TCSharpStatement : ICSharpStatement;
    TCSharpMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure = null);
    TCSharpMethod AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure = null);
    TCSharpMethod AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure = null)
        where TCSharpStatement : ICSharpStatement;
    TCSharpMethod Async(bool asValueTask = false);
    TCSharpMethod FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith);
    TCSharpMethod InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure = null);
    IList<ICSharpGenericParameter> GenericParameters { get; }
    IList<ICSharpGenericTypeConstraint> GenericTypeConstraints { get; }
    bool HasExpressionBody { get; }
    TCSharpMethod InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure = null);
    TCSharpMethod InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure = null);
    bool IsAsync { get; }
    bool IsStatic { get; }
    void RemoveStatement(ICSharpStatement statement);
    ICSharpExpression ReturnType { get; }
    TCSharpMethod Static();
    TCSharpMethod Sync();
    TCSharpMethod WithExpressionBody(string statement, Action<ICSharpStatement>? configure = null);
    TCSharpMethod WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure = null)
        where TCSharpStatement : class, ICSharpStatement;
    TCSharpMethod WithReturnType(ICSharpType returnType);
    TCSharpMethod WithReturnType(string returnType);
}