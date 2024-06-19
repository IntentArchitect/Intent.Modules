using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpInterfaceMethod : ICSharpMember<ICSharpInterfaceMethod>, ICSharpMethodDeclarationActual
{
    bool IsAbstract { get; set; }
    bool IsStatic { get; set; }
    bool HasExpressionBody { get; }
    IList<ICSharpGenericParameter> GenericParameters { get; }
    IList<ICSharpGenericTypeConstraint> GenericTypeConstraints { get; }
    ICSharpInterfaceMethod WithDefaultImplementation();
    ICSharpInterfaceMethod Async();
    ICSharpInterfaceMethod Static();
    ICSharpInterfaceMethod AddParameter(string type, string name, Action<ICSharpParameter> configure = null);
    ICSharpInterfaceMethod InsertParameter(int index, string type, string name, Action<ICSharpParameter> configure = null);
    ICSharpInterfaceMethod AddGenericParameter(string typeName);
    ICSharpInterfaceMethod AddGenericParameter(string typeName, out ICSharpGenericParameter param);
    ICSharpInterfaceMethod AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure);
    ICSharpInterfaceMethod AddStatement(string statement, Action<ICSharpStatement> configure = null);
    ICSharpInterfaceMethod AddStatement<TStatement>(TStatement statement, Action<TStatement> configure = null)
        where TStatement : class, ICSharpStatement;
    ICSharpInterfaceMethod WithExpressionBody(string statement, Action<ICSharpStatement> configure = null);
    ICSharpInterfaceMethod WithExpressionBody<TStatement>(TStatement statement, Action<TStatement> configure = null)
        where TStatement : class, ICSharpStatement;
    ICSharpInterfaceMethod WithReturnType(string returnType);
}