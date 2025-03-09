#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpClassMethodWrapper(CSharpClassMethod wrapped) :
    CSharpMemberWrapper<CSharpClassMethod, ICSharpClassMethodDeclaration>(wrapped), ICSharpClassMethodDeclaration
{
    ICSharpClass ICSharpClassMethodDeclaration.Class => wrapped.Class;
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        var result = wrapped.AddGenericParameter(typeName, out var concrete);
        param = concrete;
        return result;
    }
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddGenericParameter(string typeName) => wrapped.AddGenericParameter(typeName);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure) => wrapped.AddGenericTypeConstraint(genericParameterName, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddOptionalCancellationTokenParameter(string? parameterName) => wrapped.AddOptionalCancellationTokenParameter(parameterName);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(type, name, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(type, model, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameter(model, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure) => wrapped.AddParameters(models, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatement(string statement, Action<ICSharpStatement>? configure) => wrapped.AddStatement(statement, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure) => wrapped.AddStatement(statement, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.AddStatements(statements, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.AddStatements(statements, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure)
    {
        return wrapped.AddStatements(statements.Cast<CSharpStatement>(), items => configure?.Invoke(items.Cast<TCSharpStatement>()));
    }
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.Async(bool asValueTask) => wrapped.Async(asValueTask);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith) => wrapped.FindAndReplaceStatement(matchFunc, (CSharpStatement)replaceWith);
    IList<ICSharpGenericParameter> ICSharpMethod<ICSharpClassMethodDeclaration>.GenericParameters => new WrappedList<CSharpGenericParameter, ICSharpGenericParameter>(wrapped.GenericParameters);
    IList<ICSharpGenericTypeConstraint> ICSharpMethod<ICSharpClassMethodDeclaration>.GenericTypeConstraints => new WrappedList<CSharpGenericTypeConstraint, ICSharpGenericTypeConstraint>(wrapped.GenericTypeConstraints);
    bool ICSharpMethod<ICSharpClassMethodDeclaration>.HasExpressionBody => wrapped.HasExpressionBody;
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure) => wrapped.InsertParameter(index, type, name, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure) => wrapped.InsertStatement(index, (CSharpStatement)statement, configure);
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure) => wrapped.InsertStatements(index, statements.Cast<CSharpStatement>().ToArray(), configure);
    bool ICSharpMethod<ICSharpClassMethodDeclaration>.IsAsync => ((ICSharpMethodDeclaration)wrapped).IsAsync;
    bool ICSharpMethod<ICSharpClassMethodDeclaration>.IsStatic => wrapped.IsStatic;
    void ICSharpMethod<ICSharpClassMethodDeclaration>.RemoveStatement(ICSharpStatement statement) => wrapped.RemoveStatement((CSharpStatement)statement);
    ICSharpExpression ICSharpMethod<ICSharpClassMethodDeclaration>.ReturnType => ((ICSharpMethodDeclaration)wrapped).ReturnType;
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.Static() => wrapped.Static();
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.Sync() => wrapped.Sync();
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithExpressionBody(string statement, Action<ICSharpStatement>? configure)
    {
        return wrapped.WithExpressionBody<CSharpStatement>(new CSharpStatement(statement), configure);
    }
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure)
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
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithReturnType(ICSharpType returnType)
    {
        if (returnType is not CSharpType cSharpType)
        {
            throw new Exception($"{nameof(returnType)} is not {nameof(CSharpType)}");
        }

        return wrapped.WithReturnType(cSharpType);
    }
    ICSharpClassMethodDeclaration ICSharpMethod<ICSharpClassMethodDeclaration>.WithReturnType(string returnType) => wrapped.WithReturnType(returnType);
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Abstract() => wrapped.Abstract();
    string ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.ExplicitImplementationFor => wrapped.ExplicitImplementationFor;
    bool ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.IsAbstract => wrapped.IsAbstract;
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.IsExplicitImplementationFor(string @interface) => wrapped.IsExplicitImplementationFor(@interface);
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.New() => wrapped.New();
    ICSharpClassMethodDeclaration ICSharpClassMethodDeclaration.Operator(bool isOperator) => wrapped.Operator(isOperator);
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Override() => wrapped.Override();
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Private() => wrapped.Private();
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Protected() => wrapped.Protected();
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.Virtual() => wrapped.Virtual();
    ICSharpClassMethodDeclaration ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>.WithoutAccessModifier() => wrapped.WithoutAccessModifier();
    string IHasCSharpName.Name => wrapped.Name;
    bool IHasCSharpStatementsActual.IsCodeBlock => ((IHasCSharpStatementsActual)wrapped).IsCodeBlock;
    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => new WrappedList<CSharpStatement, ICSharpStatement>(wrapped.Statements);
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => wrapped.Parameters;
}