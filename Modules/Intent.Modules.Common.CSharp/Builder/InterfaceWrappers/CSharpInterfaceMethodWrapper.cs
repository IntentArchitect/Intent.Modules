#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpInterfaceMethodWrapper(CSharpInterfaceMethod wrapped) :
    CSharpMemberWrapper<CSharpInterfaceMethod, ICSharpInterfaceMethodDeclaration>(wrapped), ICSharpInterfaceMethodDeclaration
{
    ICSharpInterface ICSharpInterfaceMethodDeclaration.Interface => wrapped.Interface;
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        var result = wrapped.AddGenericParameter(typeName, out var concrete);
        param = concrete;
        return result;
    }
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddGenericParameter(string typeName) => wrapped.AddGenericParameter(typeName);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure) => wrapped.AddGenericTypeConstraint(genericParameterName, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddOptionalCancellationTokenParameter(string? parameterName) => wrapped.AddOptionalCancellationTokenParameter(parameterName);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(type, name, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(type, model, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(model, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameters(models, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatement(string statement, Action<ICSharpStatement>? configure) => wrapped.AddStatement(statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => wrapped.AddStatement(statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.AddStatements(statements, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.AddStatements(statements, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure) => wrapped.AddStatements(statements.Cast<CSharpStatement>(), items => configure?.Invoke(items.Cast<TCSharpStatement>()));
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.Async(bool asValueTask) => wrapped.Async(asValueTask);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith) => wrapped.FindAndReplaceStatement(matchFunc, (CSharpStatement)replaceWith);
    IList<ICSharpGenericParameter> ICSharpMethod<ICSharpInterfaceMethodDeclaration>.GenericParameters => new WrappedList<CSharpGenericParameter, ICSharpGenericParameter>(wrapped.GenericParameters);
    IList<ICSharpGenericTypeConstraint> ICSharpMethod<ICSharpInterfaceMethodDeclaration>.GenericTypeConstraints => new WrappedList<CSharpGenericTypeConstraint, ICSharpGenericTypeConstraint>(wrapped.GenericTypeConstraints);
    bool ICSharpMethod<ICSharpInterfaceMethodDeclaration>.HasExpressionBody => wrapped.HasExpressionBody;
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure) => wrapped.InsertParameter(index, type, name, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure) => wrapped.InsertStatement(index, (CSharpStatement)statement, configure);
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.InsertStatements(index, statements.Cast<CSharpStatement>().ToArray(), configure);
    bool ICSharpMethod<ICSharpInterfaceMethodDeclaration>.IsAsync => ((ICSharpMethodDeclaration)wrapped).IsAsync;
    bool ICSharpMethod<ICSharpInterfaceMethodDeclaration>.IsStatic => wrapped.IsStatic;
    void ICSharpMethod<ICSharpInterfaceMethodDeclaration>.RemoveStatement(ICSharpStatement statement) => wrapped.RemoveStatement((CSharpStatement)statement);
    ICSharpExpression ICSharpMethod<ICSharpInterfaceMethodDeclaration>.ReturnType => ((ICSharpMethodDeclaration)wrapped).ReturnType;
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.Static() => wrapped.Static();
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.Sync() => wrapped.Sync();
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithExpressionBody(string statement, Action<ICSharpStatement>? configure)
    {
        return wrapped.WithExpressionBody<CSharpStatement>(new CSharpStatement(statement), configure);
    }
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure)
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
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithReturnType(ICSharpType returnType)
    {
        if (returnType is not CSharpType cSharpType)
        {
            throw new Exception($"{nameof(returnType)} is not {nameof(CSharpType)}");
        }

        return wrapped.WithReturnType(cSharpType);
    }
    ICSharpInterfaceMethodDeclaration ICSharpMethod<ICSharpInterfaceMethodDeclaration>.WithReturnType(string returnType) => wrapped.WithReturnType(returnType);
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Abstract() => wrapped.Abstract();
    string ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.ExplicitImplementationFor => wrapped.ExplicitImplementationFor;
    bool ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.IsAbstract => wrapped.IsAbstract;
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.IsExplicitImplementationFor(string @interface) => wrapped.IsExplicitImplementationFor(@interface);
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.New() => wrapped.New();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Override() => wrapped.Override();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Partial() => wrapped.Partial();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Internal() => wrapped.Internal();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Private() => wrapped.Private();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Protected() => wrapped.Protected();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.ProtectedInternal() => wrapped.ProtectedInternal();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Public() => wrapped.Public();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.Virtual() => wrapped.Virtual();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.WithoutAccessModifier() => wrapped.WithoutAccessModifier();
    ICSharpInterfaceMethodDeclaration ICSharpMethodDeclaration<ICSharpInterfaceMethodDeclaration>.WithoutMethodModifier() => wrapped.WithoutMethodModifier();
    string IHasCSharpName.Name => wrapped.Name;
    bool IHasCSharpStatementsActual.IsCodeBlock => ((IHasCSharpStatementsActual)wrapped).IsCodeBlock;
    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => new WrappedList<CSharpStatement, ICSharpStatement>(wrapped.Statements);
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => wrapped.Parameters;
}