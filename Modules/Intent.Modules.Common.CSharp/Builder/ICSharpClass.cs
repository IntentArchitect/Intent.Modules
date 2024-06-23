#nullable enable
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpClass : ICSharpDeclaration<ICSharpClass>, IBuildsCSharpMembers, ICSharpReferenceable, ICodeBlock
{
    ICSharpClass? BaseType { get; set; }
    IList<string> BaseTypeTypeParameters { get; }
    IList<string> Interfaces { get; }
    IList<ICSharpField> Fields { get; }
    IList<ICSharpConstructor> Constructors { get; }
    IList<ICSharpProperty> Properties { get; }
    IList<ICSharpClassMethod> Methods { get; }
    IList<ICSharpGenericParameter> GenericParameters { get; }
    IList<ICSharpClass> NestedClasses { get; }
    IList<ICSharpInterface> NestedInterfaces { get; }
    IList<ICSharpGenericTypeConstraint> GenericTypeConstraints { get; }
    IList<ICSharpCodeBlock> CodeBlocks { get; }
    bool IsPartial { get; set; }
    bool IsAbstract { get; set; }
    bool IsStatic { get; set; }
    bool IsSealed { get; set; }
    IDictionary<string, object> Metadata { get; }
    IList<ICSharpAttribute> Attributes { get; }
    ICSharpXmlComments XmlComments { get; }
    ICSharpClass WithBaseType(string type);
    ICSharpClass WithBaseType(string type, IEnumerable<string> genericTypeParameters);
    ICSharpClass WithBaseType(ICSharpClass type);
    ICSharpClass WithBaseType(ICSharpClass type, IEnumerable<string> genericTypeParameters);
    ICSharpClass ExtendsClass(string type);
    ICSharpClass ExtendsClass(string type, IEnumerable<string> genericTypeParameters);
    ICSharpClass ExtendsClass(ICSharpClass @class);
    ICSharpClass ExtendsClass(ICSharpClass @class, IEnumerable<string> genericTypeParameters);
    ICSharpClass ImplementsInterface(string type);
    ICSharpClass ImplementsInterfaces(IEnumerable<string> types);

    /// <summary>
    /// Resolves the property name from the <paramref name="model"/>. Registers this property as representative of the <paramref name="model"/>.
    /// </summary>
    ICSharpClass AddProperty<TModel>(string type, TModel model, Action<ICSharpProperty>? configure = null)
        where TModel : IMetadataModel, IHasName;

    /// <summary>
    /// Resolves the type name and property name from the <paramref name="model"/> using the <see cref="ICSharpFileBuilderTemplate"/>
    /// template that was passed into the <see cref="ICSharpFile"/>. Registers this property as representative of the <paramref name="model"/>.
    /// </summary>
    ICSharpClass AddProperty<TModel>(TModel model, Action<ICSharpProperty>? configure = null)
        where TModel : IMetadataModel, IHasName;

    ICSharpClass InsertProperty(int index, string type, string name, Action<CSharpProperty>? configure = null);
    ICSharpClass AddConstructor(Action<ICSharpConstructor>? configure = null);
    ICSharpClass AddPrimaryConstructor(Action<ICSharpConstructor>? configure = null);

    /// <summary>
    /// Resolves the method name from the <paramref name="model"/>. Registers this method as representative of the <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    ICSharpClass AddMethod<TModel>(string returnType, TModel model, Action<ICSharpClassMethod>? configure = null)
        where TModel : IMetadataModel, IHasName;

    /// <summary>
    /// Resolves the type name and method name from the <paramref name="model"/> using the <see cref="ICSharpFileBuilderTemplate"/>
    /// template that was passed into the <see cref="ICSharpFile"/>. Registers this method as representative of the <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    ICSharpClass AddMethod<TModel>(TModel model, Action<ICSharpClassMethod>? configure = null)
        where TModel : IMetadataModel, IHasName;

    ICSharpClass AddCodeBlock(string codeLine);
    ICSharpClass AddGenericParameter(string typeName);
    ICSharpClass AddGenericParameter(string typeName, out ICSharpGenericParameter param);
    ICSharpClass AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure);
    ICSharpClass AddNestedClass(string name, Action<ICSharpClass>? configure = null);
    ICSharpClass AddNestedInterface(string name, Action<ICSharpInterface>? configure = null);
    ICSharpClass InsertMethod(int index, string returnType, string name, Action<ICSharpClassMethod>? configure = null);
    ICSharpClass WithFieldsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines);
    ICSharpClass WithPropertiesSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines);
    ICSharpClassMethod? FindMethod(string name);
    ICSharpClassMethod? FindMethod(Func<ICSharpClassMethod, bool> matchFunc);
    ICSharpClass Internal();
    ICSharpClass InternalProtected();
    ICSharpClass Protected();
    ICSharpClass Private();
    ICSharpClass Partial();
    ICSharpClass Sealed();
    ICSharpClass Unsealed();
    ICSharpClass Abstract();
    ICSharpClass Static();
    IEnumerable<ICSharpClass> GetParentPath();
    IEnumerable<ICSharpProperty> GetAllProperties();
    string ToString(string indentation);
    ICSharpClass AddMetadata<T>(string key, T value);
    ICSharpClass RepresentsModel(IMetadataModel model);
}