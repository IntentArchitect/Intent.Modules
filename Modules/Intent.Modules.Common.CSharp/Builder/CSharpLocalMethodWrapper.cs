#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpLocalMethodWrapper(CSharpLocalMethod wrapped) : ICSharpLocalFunction
{
    ICSharpCodeContext ICSharpCodeContext.AddMetadata(string key, object value) => wrapped.AddMetadata(key, value);
    ICSharpFile ICSharpCodeContext.File => wrapped.File;
    object ICSharpCodeContext.GetMetadata(string key) => wrapped.GetMetadata(key);
    public void RemoveMetadata(string key) => wrapped.RemoveMetadata(key);
    T ICSharpCodeContext.GetMetadata<T>(string key) => wrapped.GetMetadata<T>(key);
    IHasCSharpName ICSharpCodeContext.GetReferenceForModel(IMetadataModel model) => wrapped.GetReferenceForModel(model);
    IHasCSharpName ICSharpCodeContext.GetReferenceForModel(string modelId) => wrapped.GetReferenceForModel(modelId);
    bool ICSharpCodeContext.HasMetadata(string key) => wrapped.HasMetadata(key);
    ICSharpCodeContext ICSharpCodeContext.Parent => (ICSharpCodeContext)wrapped.Parent;
    void ICSharpCodeContext.RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable) => wrapped.RegisterReferenceable(modelId, cSharpReferenceable);
    void ICSharpCodeContext.RepresentsModel(IMetadataModel model) => wrapped.RepresentsModel(model);
    bool ICSharpCodeContext.TryGetMetadata(string key, out object value) => wrapped.TryGetMetadata(key, out value);
    bool ICSharpCodeContext.TryGetMetadata<T>(string key, out T value) => wrapped.TryGetMetadata(key, out value);
    bool ICSharpCodeContext.TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference) => wrapped.TryGetReferenceForModel(model, out reference);
    bool ICSharpCodeContext.TryGetReferenceForModel(string modelId, out IHasCSharpName reference) => wrapped.TryGetReferenceForModel(modelId, out reference);
    IHasCSharpStatements ICSharpLocalFunction.Parent => wrapped.Parent;
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        var result = wrapped.AddGenericParameter(typeName, out var concrete);
        param = concrete;
        return result;
    }
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddGenericParameter(string typeName) => wrapped.AddGenericParameter(typeName);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure) => wrapped.AddGenericTypeConstraint(genericParameterName, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddOptionalCancellationTokenParameter(string? parameterName) => wrapped.AddOptionalCancellationTokenParameter(parameterName);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(type, name, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(type, model, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(model, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameters(models, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatement(string statement, Action<ICSharpStatement>? configure) => wrapped.AddStatement(statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => wrapped.AddStatement(statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.AddStatements(statements, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.AddStatements(statements, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure)
    {
        return wrapped.AddStatements(statements.Cast<CSharpStatement>(), items => configure?.Invoke(items.Cast<TCSharpStatement>()));
    }
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.Async(bool asValueTask) => wrapped.Async(asValueTask);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith) => wrapped.FindAndReplaceStatement(matchFunc, (CSharpStatement)replaceWith);
    IList<ICSharpGenericParameter> ICSharpMethod<ICSharpLocalFunction>.GenericParameters => new WrappedList<CSharpGenericParameter, ICSharpGenericParameter>(wrapped.GenericParameters);
    IList<ICSharpGenericTypeConstraint> ICSharpMethod<ICSharpLocalFunction>.GenericTypeConstraints => new WrappedList<CSharpGenericTypeConstraint, ICSharpGenericTypeConstraint>(wrapped.GenericTypeConstraints);
    bool ICSharpMethod<ICSharpLocalFunction>.HasExpressionBody => wrapped.HasExpressionBody;
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure) => wrapped.InsertParameter(index, type, name, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure) => wrapped.InsertStatement(index, statement, configure);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.InsertStatements(index, statements, configure);
    bool ICSharpMethod<ICSharpLocalFunction>.IsAsync => wrapped.IsAsync;
    bool ICSharpMethod<ICSharpLocalFunction>.IsStatic => wrapped.IsStatic;
    void ICSharpMethod<ICSharpLocalFunction>.RemoveStatement(ICSharpStatement statement) => wrapped.RemoveStatement((CSharpStatement)statement);
    ICSharpExpression ICSharpMethod<ICSharpLocalFunction>.ReturnType => new CSharpStatement(wrapped.ReturnType);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.Static() => wrapped.Static();
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.Sync() => wrapped.Sync();
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithExpressionBody(string statement, Action<ICSharpStatement>? configure)
    {
        return wrapped.WithExpressionBody<CSharpStatement>(new CSharpStatement(statement), configure);
    }
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure)
    {
        if (statement is not CSharpStatement concreteStatement)
        {
            throw new Exception($"{nameof(statement)} is not {nameof(CSharpStatement)}");
        }

        return wrapped.WithExpressionBody(concreteStatement, tCSharpStatement =>
        {
            if (tCSharpStatement is not TCSharpStatement concreteCSharpStatement)
            {
                throw new Exception($"{nameof(tCSharpStatement)} is not {nameof(CSharpStatement)}");
            }

            configure?.Invoke(concreteCSharpStatement);
        });
    }
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithReturnType(ICSharpType returnType) => wrapped.WithReturnType(returnType);
    ICSharpLocalFunction ICSharpMethod<ICSharpLocalFunction>.WithReturnType(string returnType) => wrapped.WithReturnType(returnType);
    string IHasCSharpName.Name => wrapped.Name;
    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => new WrappedList<CSharpStatement, ICSharpStatement>(wrapped.Statements);
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => wrapped.Parameters;

    bool IHasCSharpStatementsActual.IsCodeBlock => false;
}