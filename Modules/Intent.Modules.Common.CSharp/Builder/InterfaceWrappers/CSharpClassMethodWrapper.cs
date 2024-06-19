using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpClassMethodWrapper(CSharpClassMethod wrapped) :
    CSharpMemberWrapper<CSharpClassMethod, ICSharpClassMethod>(wrapped), ICSharpClassMethod
{
    IList<ICSharpGenericParameter> ICSharpClassMethod.GenericParameters => new WrappedList<CSharpGenericParameter, ICSharpGenericParameter>(wrapped.GenericParameters);

    IList<ICSharpGenericTypeConstraint> ICSharpClassMethod.GenericTypeConstraints => new WrappedList<CSharpGenericTypeConstraint, ICSharpGenericTypeConstraint>(wrapped.GenericTypeConstraints);

    string ICSharpClassMethod.ExplicitImplementationFor => wrapped.ExplicitImplementationFor;

    ICSharpClass ICSharpClassMethod.Class => wrapped.Class;

    ICSharpClassMethod ICSharpClassMethod.IsExplicitImplementationFor(string @interface) => wrapped.IsExplicitImplementationFor(@interface);

    ICSharpClassMethod ICSharpClassMethod.AddParameter(string type, string name, Action<ICSharpMethodParameter> configure) => wrapped.AddParameter(type, name, configure);

    ICSharpClassMethod ICSharpClassMethod.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter> configure) => wrapped.AddParameter(type, model, configure);

    ICSharpClassMethod ICSharpClassMethod.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter> configure) => wrapped.AddParameter(model, configure);

    ICSharpClassMethod ICSharpClassMethod.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter> configure) => wrapped.AddParameters(models, configure);

    ICSharpClassMethod ICSharpClassMethod.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter> configure) => wrapped.InsertParameter(index, type, name, configure);

    ICSharpClassMethod ICSharpClassMethod.AddOptionalCancellationTokenParameter() => wrapped.AddOptionalCancellationTokenParameter();

    ICSharpClassMethod ICSharpClassMethod.AddOptionalCancellationTokenParameter(string parameterName) => wrapped.AddOptionalCancellationTokenParameter(parameterName);

    ICSharpClassMethod ICSharpClassMethod.AddGenericParameter(string typeName) => wrapped.AddGenericParameter(typeName);

    ICSharpClassMethod ICSharpClassMethod.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        var result = wrapped.AddGenericParameter(typeName, out var concrete);
        param = concrete;
        return result;
    }

    ICSharpClassMethod ICSharpClassMethod.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure) => wrapped.AddGenericTypeConstraint(genericParameterName, configure);

    ICSharpClassMethod ICSharpClassMethod.AddStatement(string statement, Action<ICSharpStatement> configure) => wrapped.AddStatement(statement, configure);

    ICSharpClassMethod ICSharpClassMethod.AddStatement(ICSharpStatement statement, Action<ICSharpStatement> configure) => wrapped.AddStatement((CSharpStatement)statement, configure);

    ICSharpClassMethod ICSharpClassMethod.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement> configure) => wrapped.InsertStatement(index, (CSharpStatement)statement, configure);

    ICSharpClassMethod ICSharpClassMethod.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>> configure) => wrapped.InsertStatements(index, statements.Cast<CSharpStatement>().ToArray(), configure);

    ICSharpClassMethod ICSharpClassMethod.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>> configure) => wrapped.AddStatements(statements, configure);

    ICSharpClassMethod ICSharpClassMethod.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>> configure) => wrapped.AddStatements(statements, configure);

    ICSharpClassMethod ICSharpClassMethod.AddStatements(IEnumerable<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>> configure) => wrapped.AddStatements(statements.Cast<CSharpStatement>(), configure);

    ICSharpClassMethod ICSharpClassMethod.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith) => wrapped.FindAndReplaceStatement(matchFunc, (CSharpStatement)replaceWith);

    ICSharpClassMethod ICSharpClassMethod.Protected() => wrapped.Protected();

    ICSharpClassMethod ICSharpClassMethod.Private() => wrapped.Private();

    ICSharpClassMethod ICSharpClassMethod.WithReturnType(string returnType) => wrapped.WithReturnType(returnType);

    ICSharpClassMethod ICSharpClassMethod.WithoutAccessModifier() => wrapped.WithoutAccessModifier();

    ICSharpClassMethod ICSharpClassMethod.Override() => wrapped.Override();

    ICSharpClassMethod ICSharpClassMethod.New() => wrapped.New();

    ICSharpClassMethod ICSharpClassMethod.Virtual() => wrapped.Virtual();

    ICSharpClassMethod ICSharpClassMethod.Abstract() => wrapped.Abstract();

    ICSharpClassMethod ICSharpClassMethod.Static() => wrapped.Static();

    ICSharpClassMethod ICSharpClassMethod.Sync() => wrapped.Sync();

    ICSharpClassMethod ICSharpClassMethod.Async() => wrapped.Async();

    ICSharpClassMethod ICSharpClassMethod.WithExpressionBody(ICSharpStatement statement) => wrapped.WithExpressionBody((CSharpStatement)statement);

    void ICSharpClassMethod.RemoveStatement(ICSharpStatement statement) => wrapped.RemoveStatement((CSharpStatement)statement);

    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => ((IHasICSharpParameters)wrapped).Parameters;

    string IHasCSharpName.Name => ((IHasCSharpName)wrapped).Name;

    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => new WrappedList<CSharpStatement, ICSharpStatement>(wrapped.Statements);

    bool ICSharpClassMethod.IsAbstract => wrapped.IsAbstract;

    bool ICSharpClassMethod.HasExpressionBody => wrapped.HasExpressionBody;

    bool ICSharpMethodDeclarationActual.IsAsync => ((ICSharpMethodDeclaration)wrapped).IsAsync;

    ICSharpExpression ICSharpMethodDeclarationActual.ReturnType => ((ICSharpMethodDeclaration)wrapped).ReturnType;
}