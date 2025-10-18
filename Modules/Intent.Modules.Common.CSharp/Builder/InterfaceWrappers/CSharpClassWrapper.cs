#nullable enable
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpClassWrapper(CSharpClass wrapped) :
    CSharpDeclarationWrapper<CSharpClass, ICSharpClass>(wrapped), ICSharpClass
{
    public CSharpCodeSeparatorType AfterSeparator
    {
        get => ((ICodeBlock)wrapped).AfterSeparator;
        set => ((ICodeBlock)wrapped).AfterSeparator = value;
    }

    public string GetText(string indentation)
    {
        return ((ICodeBlock)wrapped).GetText(indentation);
    }

    public CSharpCodeSeparatorType BeforeSeparator
    {
        get => ((ICodeBlock)wrapped).BeforeSeparator;
        set => ((ICodeBlock)wrapped).BeforeSeparator = value;
    }

    ICSharpClass? ICSharpClass.BaseType
    {
        get => wrapped.BaseType;
        set => wrapped.BaseType = (CSharpClass)value!;
    }

    IList<string> ICSharpClass.BaseTypeTypeParameters => wrapped.BaseTypeTypeParameters;

    IList<string> ICSharpClass.Interfaces => wrapped.Interfaces;

    IList<ICSharpField> ICSharpClass.Fields => new WrappedList<CSharpField, ICSharpField>(wrapped.Fields);

    IList<ICSharpConstructor> ICSharpClass.Constructors => new WrappedList<CSharpConstructor, ICSharpConstructor>(wrapped.Constructors);

    IList<ICSharpProperty> ICSharpClass.Properties => new WrappedList<CSharpProperty, ICSharpProperty>(wrapped.Properties);

    IList<ICSharpClassMethodDeclaration> ICSharpClass.Methods => new WrappedList<CSharpClassMethod, ICSharpClassMethodDeclaration>(wrapped.Methods);

    IList<ICSharpGenericParameter> ICSharpClass.GenericParameters => new WrappedList<CSharpGenericParameter, ICSharpGenericParameter>(wrapped.GenericParameters);

    IList<ICSharpClass> ICSharpClass.NestedClasses => new WrappedList<CSharpClass, ICSharpClass>(wrapped.NestedClasses);

    IList<ICSharpInterface> ICSharpClass.NestedInterfaces => new WrappedList<CSharpInterface, ICSharpInterface>(wrapped.NestedInterfaces);

    IList<ICSharpGenericTypeConstraint> ICSharpClass.GenericTypeConstraints => new WrappedList<CSharpGenericTypeConstraint, ICSharpGenericTypeConstraint>(wrapped.GenericTypeConstraints);

    IList<ICSharpCodeBlock> ICSharpClass.CodeBlocks => new WrappedList<CSharpCodeBlock, ICSharpCodeBlock>(wrapped.CodeBlocks);

    bool ICSharpClass.IsPartial
    {
        get => wrapped.IsPartial;
        set => wrapped.IsPartial = value;
    }

    bool ICSharpClass.IsAbstract
    {
        get => wrapped.IsAbstract;
        set => wrapped.IsAbstract = value;
    }

    bool ICSharpClass.IsStatic
    {
        get => wrapped.IsStatic;
        set => wrapped.IsStatic = value;
    }

    bool ICSharpClass.IsSealed
    {
        get => wrapped.IsSealed;
        set => wrapped.IsSealed = value;
    }

    IDictionary<string, object> ICSharpClass.Metadata => wrapped.Metadata;

    IList<ICSharpAttribute> ICSharpClass.Attributes => new WrappedList<CSharpAttribute, ICSharpAttribute>(wrapped.Attributes);

    ICSharpXmlComments ICSharpClass.XmlComments => wrapped.XmlComments;

    ICSharpClass ICSharpClass.WithBaseType(string type) => wrapped.WithBaseType(type);

    ICSharpClass ICSharpClass.WithBaseType(string type, IEnumerable<string> genericTypeParameters) => wrapped.WithBaseType(type, genericTypeParameters);

    ICSharpClass ICSharpClass.WithBaseType(ICSharpClass type) => wrapped.WithBaseType((CSharpClass)type);

    ICSharpClass ICSharpClass.WithBaseType(ICSharpClass type, IEnumerable<string> genericTypeParameters) => wrapped.WithBaseType((CSharpClass)type, genericTypeParameters);

    ICSharpClass ICSharpClass.ExtendsClass(string type) => wrapped.ExtendsClass(type);

    ICSharpClass ICSharpClass.ExtendsClass(string type, IEnumerable<string> genericTypeParameters) => wrapped.ExtendsClass(type, genericTypeParameters);

    ICSharpClass ICSharpClass.ExtendsClass(ICSharpClass @class) => wrapped.ExtendsClass((CSharpClass)@class);

    ICSharpClass ICSharpClass.ExtendsClass(ICSharpClass @class, IEnumerable<string> genericTypeParameters) => wrapped.ExtendsClass((CSharpClass)@class, genericTypeParameters);

    ICSharpClass ICSharpClass.ImplementsInterface(string type) => wrapped.ImplementsInterface(type);

    ICSharpClass ICSharpClass.ImplementsInterfaces(IEnumerable<string> types) => wrapped.ImplementsInterfaces(types);

    ICSharpClass ICSharpClass.AddMethod<TModel>(string returnType, TModel model, Action<ICSharpClassMethodDeclaration>? configure) => wrapped.AddMethod(returnType, model, configure);

    ICSharpClass ICSharpClass.AddMethod<TModel>(TModel model, Action<ICSharpClassMethodDeclaration>? configure) => wrapped.AddMethod(model, configure);

    ICSharpClass ICSharpClass.AddCodeBlock(string codeLine) => wrapped.AddCodeBlock(codeLine);

    ICSharpClass ICSharpClass.AddGenericParameter(string typeName) => wrapped.AddGenericParameter(typeName);

    ICSharpClass ICSharpClass.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        var result = wrapped.AddGenericParameter(typeName, out var concrete);
        param = concrete;
        return result;
    }

    ICSharpClass ICSharpClass.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure) => wrapped.AddGenericTypeConstraint(genericParameterName, configure);

    ICSharpClass ICSharpClass.AddNestedClass(string name, Action<ICSharpClass>? configure) => wrapped.AddNestedClass(name, configure);
    
    ICSharpClass ICSharpClass.AddNestedRecord(string name, Action<ICSharpClass>? configure) => wrapped.AddNestedRecord(name, configure);

    ICSharpClass ICSharpClass.AddNestedInterface(string name, Action<ICSharpInterface>? configure) => wrapped.AddNestedInterface(name, configure);

    ICSharpClass ICSharpClass.InsertMethod(int index, string returnType, string name, Action<ICSharpClassMethodDeclaration>? configure) => wrapped.InsertMethod(index, returnType, name, configure);

    ICSharpClass ICSharpClass.WithFieldsSeparated(CSharpCodeSeparatorType separator) => wrapped.WithFieldsSeparated(separator);

    ICSharpClass ICSharpClass.WithPropertiesSeparated(CSharpCodeSeparatorType separator) => wrapped.WithPropertiesSeparated(separator);

    ICSharpClassMethodDeclaration? ICSharpClass.FindMethod(string name) => wrapped.FindMethod(name);

    ICSharpClassMethodDeclaration? ICSharpClass.FindMethod(Func<ICSharpClassMethodDeclaration, bool> matchFunc) => wrapped.FindMethod(matchFunc);

    ICSharpClass ICSharpClass.Internal() => wrapped.Internal();

    [Obsolete]
    ICSharpClass ICSharpClass.InternalProtected() => wrapped.InternalProtected();

    ICSharpClass ICSharpClass.Private() => wrapped.Private();

    ICSharpClass ICSharpClass.Protected() => wrapped.Protected();

    ICSharpClass ICSharpClass.ProtectedInternal() => wrapped.ProtectedInternal();

    ICSharpClass ICSharpClass.Public() => wrapped.Protected();

    ICSharpClass ICSharpClass.Partial() => wrapped.Partial();

    ICSharpClass ICSharpClass.Sealed() => wrapped.Sealed();

    ICSharpClass ICSharpClass.Unsealed() => wrapped.Unsealed();

    ICSharpClass ICSharpClass.Abstract() => wrapped.Abstract();

    ICSharpClass ICSharpClass.Static() => wrapped.Static();

    IEnumerable<ICSharpClass> ICSharpClass.GetParentPath() => wrapped.GetParentPath();

    IEnumerable<ICSharpProperty> ICSharpClass.GetAllProperties() => wrapped.GetAllProperties();

    string ICSharpClass.ToString(string indentation) => wrapped.ToString(indentation);

    ICSharpClass ICSharpClass.AddMetadata<T>(string key, T value) => wrapped.AddMetadata(key, value);

    ICSharpClass ICSharpClass.RepresentsModel(IMetadataModel model) => wrapped.RepresentsModel(model);

    ICSharpClass ICSharpClass.AddProperty(string type, string name, Action<ICSharpProperty>? configure) => wrapped.AddProperty(type, name, configure);

    ICSharpClass ICSharpClass.AddProperty<TModel>(string type, TModel model, Action<ICSharpProperty>? configure) => wrapped.AddProperty(type, model, configure);

    ICSharpClass ICSharpClass.AddProperty<TModel>(TModel model, Action<ICSharpProperty>? configure) => wrapped.AddProperty(model, configure);

    ICSharpClass ICSharpClass.InsertProperty(int index, string type, string name, Action<CSharpProperty>? configure) => wrapped.InsertProperty(index, type, name, configure);

    ICSharpClass ICSharpClass.AddConstructor(Action<ICSharpConstructor>? configure) => wrapped.AddConstructor(configure);

    ICSharpClass ICSharpClass.AddPrimaryConstructor(Action<ICSharpConstructor>? configure) => wrapped.AddPrimaryConstructor(configure);

    ICSharpClass ICSharpClass.AddNullForgivingConstructor(Action<CSharpConstructor>? configure) => wrapped.AddNullForgivingConstructor(configure);

    IBuildsCSharpMembers IBuildsCSharpMembers.AddClass(string name, Action<ICSharpClass>? configure) => ((IBuildsCSharpMembers)wrapped).AddClass(name, configure);

    IBuildsCSharpMembers IBuildsCSharpMembers.AddField(string type, string name, Action<ICSharpField>? configure) => ((IBuildsCSharpMembers)wrapped).AddField(type, name, configure);

    IBuildsCSharpMembers IBuildsCSharpMembers.AddMethod(string returnType, string name, Action<ICSharpClassMethodDeclaration>? configure) => ((IBuildsCSharpMembers)wrapped).AddMethod(returnType, name, configure);

    IBuildsCSharpMembers IBuildsCSharpMembers.AddProperty(string type, string name, Action<ICSharpProperty>? configure) => ((IBuildsCSharpMembers)wrapped).AddProperty(type, name, configure);

    IBuildsCSharpMembers IBuildsCSharpMembers.InsertField(int index, string type, string name, Action<ICSharpField>? configure) => ((IBuildsCSharpMembers)wrapped).InsertField(index, type, name, configure);

    IBuildsCSharpMembers IBuildsCSharpMembers.InsertMethod(int index, string returnType, string name, Action<ICSharpClassMethodDeclaration>? configure) => ((IBuildsCSharpMembers)wrapped).InsertMethod(index, returnType, name, configure);

    IBuildsCSharpMembers IBuildsCSharpMembers.InsertProperty(int index, string type, string name, Action<ICSharpProperty>? configure) => ((IBuildsCSharpMembers)wrapped).InsertProperty(index, type, name, configure);

    ICSharpTemplate IBuildsCSharpMembers.Template => wrapped.File.Template;

    IList<ICodeBlock> IBuildsCSharpMembers.Declarations => ((IBuildsCSharpMembers)wrapped).Declarations;

    int IBuildsCSharpMembers.IndexOf(ICodeBlock codeBlock) => ((IBuildsCSharpMembers)wrapped).IndexOf(codeBlock);

    string IHasCSharpName.Name => wrapped.Name;
}