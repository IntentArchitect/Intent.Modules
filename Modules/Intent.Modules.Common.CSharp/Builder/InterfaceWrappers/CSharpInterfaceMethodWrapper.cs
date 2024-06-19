using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpInterfaceMethodWrapper(CSharpInterfaceMethod wrapped) :
    CSharpMemberWrapper<CSharpInterfaceMethod, ICSharpInterfaceMethod>(wrapped), ICSharpInterfaceMethod
{
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => wrapped.Parameters;

    string IHasCSharpName.Name => wrapped.Name;

    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => new WrappedList<CSharpStatement, ICSharpStatement>(wrapped.Statements);

    bool ICSharpMethodDeclarationActual.IsAsync => wrapped.IsAsync;

    ICSharpExpression ICSharpMethodDeclarationActual.ReturnType => ((ICSharpMethodDeclaration)wrapped).ReturnType;

    bool ICSharpInterfaceMethod.IsAbstract
    {
        get => wrapped.IsAbstract;
        set => wrapped.IsAbstract = value;
    }

    bool ICSharpInterfaceMethod.IsStatic
    {
        get => wrapped.IsStatic;
        set => wrapped.IsStatic = value;
    }

    bool ICSharpInterfaceMethod.HasExpressionBody => wrapped.HasExpressionBody;

    IList<ICSharpGenericParameter> ICSharpInterfaceMethod.GenericParameters => new WrappedList<CSharpGenericParameter, ICSharpGenericParameter>(wrapped.GenericParameters);

    IList<ICSharpGenericTypeConstraint> ICSharpInterfaceMethod.GenericTypeConstraints => new WrappedList<CSharpGenericTypeConstraint, ICSharpGenericTypeConstraint>(wrapped.GenericTypeConstraints);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithDefaultImplementation() => wrapped.WithDefaultImplementation();

    ICSharpInterfaceMethod ICSharpInterfaceMethod.Async() => wrapped.Async();

    ICSharpInterfaceMethod ICSharpInterfaceMethod.Static() => wrapped.Static();

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddParameter(string type, string name, Action<ICSharpParameter> configure) => wrapped.AddParameter(type, name, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.InsertParameter(int index, string type, string name, Action<ICSharpParameter> configure) => wrapped.InsertParameter(index, type, name, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddGenericParameter(string typeName) => wrapped.AddGenericParameter(typeName);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        var result = wrapped.AddGenericParameter(typeName, out var concrete);
        param = concrete;
        return result;
    }

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure) => wrapped.AddGenericTypeConstraint(genericParameterName, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddStatement(string statement, Action<ICSharpStatement> configure) => wrapped.AddStatement(statement, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.AddStatement<TStatement>(TStatement statement, Action<TStatement> configure) => wrapped.AddStatement(statement as CSharpStatement, x => configure(x as TStatement));

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithExpressionBody(string statement, Action<ICSharpStatement> configure) => wrapped.WithExpressionBody(statement, configure);

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithExpressionBody<TStatement>(TStatement statement, Action<TStatement> configure) => wrapped.WithExpressionBody(statement as CSharpStatement, x => configure(x as TStatement));

    ICSharpInterfaceMethod ICSharpInterfaceMethod.WithReturnType(string returnType) => wrapped.WithReturnType(returnType);
}