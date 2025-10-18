using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpInterfaceWrapper(CSharpInterface wrapped) :
    CSharpDeclarationWrapper<CSharpInterface, ICSharpInterface>(wrapped), ICSharpInterface
{
    string IHasCSharpName.Name => wrapped.Name;

    CSharpCodeSeparatorType ICodeBlock.BeforeSeparator
    {
        get => wrapped.BeforeSeparator;
        set => wrapped.BeforeSeparator = value;
    }

    CSharpCodeSeparatorType ICodeBlock.AfterSeparator
    {
        get => wrapped.AfterSeparator;
        set => wrapped.AfterSeparator = value;
    }

    string ICodeBlock.GetText(string indentation)
    {
        return wrapped.GetText(indentation);
    }

    IList<string> ICSharpInterface.Interfaces
    {
        get => wrapped.Interfaces;
        set => wrapped.Interfaces = value;
    }

    IList<ICSharpInterfaceField> ICSharpInterface.Fields => new WrappedList<CSharpInterfaceField, ICSharpInterfaceField>(wrapped.Fields);

    IList<ICSharpInterfaceProperty> ICSharpInterface.Properties => new WrappedList<CSharpInterfaceProperty, ICSharpInterfaceProperty>(wrapped.Properties);

    IList<ICSharpInterfaceMethodDeclaration> ICSharpInterface.Methods => new WrappedList<CSharpInterfaceMethod, ICSharpInterfaceMethodDeclaration>(wrapped.Methods);

    IList<ICSharpInterfaceGenericParameter> ICSharpInterface.GenericParameters => new WrappedList<CSharpInterfaceGenericParameter, ICSharpInterfaceGenericParameter>(wrapped.GenericParameters);

    IList<ICSharpGenericTypeConstraint> ICSharpInterface.GenericTypeConstraints => new WrappedList<CSharpGenericTypeConstraint, ICSharpGenericTypeConstraint>(wrapped.GenericTypeConstraints);

    IList<ICSharpCodeBlock> ICSharpInterface.CodeBlocks => new WrappedList<CSharpCodeBlock, ICSharpCodeBlock>(wrapped.CodeBlocks);

    ICSharpInterface ICSharpInterface.ExtendsInterface(string type) => wrapped.ExtendsInterface(type);

    ICSharpInterface ICSharpInterface.ImplementsInterfaces(IEnumerable<string> types) => wrapped.ImplementsInterfaces(types);

    ICSharpInterface ICSharpInterface.ImplementsInterfaces(params string[] types) => wrapped.ImplementsInterfaces(types);

    ICSharpInterface ICSharpInterface.AddField(string type, string name, Action<ICSharpInterfaceField> configure) => wrapped.AddField(type, name, configure);

    ICSharpInterface ICSharpInterface.AddProperty(string type, string name, Action<ICSharpInterfaceProperty> configure) => wrapped.AddProperty(type, name, configure);

    ICSharpInterface ICSharpInterface.InsertProperty(int index, string type, string name, Action<ICSharpInterfaceProperty> configure) => wrapped.InsertProperty(index, type, name, configure);

    ICSharpInterface ICSharpInterface.AddMethod<TModel>(TModel model, Action<ICSharpInterfaceMethodDeclaration> configure) => wrapped.AddMethod(model, configure);

    ICSharpInterface ICSharpInterface.AddMethod(string returnType, string name, Action<ICSharpInterfaceMethodDeclaration> configure) => wrapped.AddMethod(returnType, name, configure);

    ICSharpInterface ICSharpInterface.AddCodeBlock(string codeLine) => wrapped.AddCodeBlock(codeLine);

    ICSharpInterface ICSharpInterface.AddGenericParameter(string typeName, Action<ICSharpInterfaceGenericParameter> configure) => wrapped.AddGenericParameter(typeName, configure);

    ICSharpInterface ICSharpInterface.AddGenericParameter(string typeName, out ICSharpInterfaceGenericParameter param, Action<ICSharpInterfaceGenericParameter> configure)
    {
        var result = wrapped.AddGenericParameter(typeName, out var concrete, configure);
        param = concrete;
        return result;
    }

    ICSharpInterface ICSharpInterface.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure) => wrapped.AddGenericTypeConstraint(genericParameterName, configure);

    ICSharpInterface ICSharpInterface.InsertMethod(int index, string returnType, string name, Action<ICSharpInterfaceMethodDeclaration> configure) => wrapped.InsertMethod(index, returnType, name, configure);

    ICSharpInterface ICSharpInterface.WithFieldsSeparated(CSharpCodeSeparatorType separator) => wrapped.WithFieldsSeparated(separator);

    ICSharpInterface ICSharpInterface.WithPropertiesSeparated(CSharpCodeSeparatorType separator) => wrapped.WithPropertiesSeparated(separator);

    ICSharpInterface ICSharpInterface.WithMethodsSeparated(CSharpCodeSeparatorType separator) => wrapped.WithMethodsSeparated(separator);

    ICSharpInterface ICSharpInterface.WithMembersSeparated(CSharpCodeSeparatorType separator) => wrapped.WithMembersSeparated(separator);

    ICSharpInterface ICSharpInterface.Internal() => wrapped.Internal();

    ICSharpInterface ICSharpInterface.InternalProtected() => wrapped.InternalProtected();

    ICSharpInterface ICSharpInterface.Protected() => wrapped.Protected();

    ICSharpInterface ICSharpInterface.ProtectedInternal() => wrapped.ProtectedInternal();

    ICSharpInterface ICSharpInterface.Public() => wrapped.Public();

    ICSharpInterface ICSharpInterface.Private() => wrapped.Private();

    ICSharpInterface ICSharpInterface.Partial() => wrapped.Partial();

    bool ICSharpInterface.IsPartial
    {
        get => wrapped.IsPartial;
        set => wrapped.IsPartial = value;
    }
}