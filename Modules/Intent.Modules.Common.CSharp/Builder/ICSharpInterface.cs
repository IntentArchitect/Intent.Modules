using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpInterface : ICSharpDeclaration<ICSharpInterface>, ICSharpReferenceable, ICodeBlock
{
    IList<string> Interfaces { get; set; }
    IList<ICSharpInterfaceField> Fields { get; }
    IList<ICSharpInterfaceProperty> Properties { get; }
    IList<ICSharpInterfaceMethod> Methods { get; }
    IList<ICSharpInterfaceGenericParameter> GenericParameters { get; }
    IList<ICSharpGenericTypeConstraint> GenericTypeConstraints { get; }
    IList<ICSharpCodeBlock> CodeBlocks { get; }
    ICSharpInterface ExtendsInterface(string type);
    ICSharpInterface ImplementsInterfaces(IEnumerable<string> types);
    ICSharpInterface ImplementsInterfaces(params string[] types);
    ICSharpInterface AddField(string type, string name, Action<ICSharpInterfaceField> configure = null);
    ICSharpInterface AddProperty(string type, string name, Action<ICSharpInterfaceProperty> configure = null);
    ICSharpInterface InsertProperty(int index, string type, string name, Action<ICSharpInterfaceProperty> configure = null);
    ICSharpInterface AddMethod<TModel>(TModel model, Action<ICSharpInterfaceMethod> configure = null) where TModel : IMetadataModel, IHasName;
    ICSharpInterface AddMethod(string returnType, string name, Action<ICSharpInterfaceMethod> configure = null);
    ICSharpInterface AddCodeBlock(string codeLine);
    ICSharpInterface AddGenericParameter(string typeName, Action<ICSharpInterfaceGenericParameter> configure = null);
    ICSharpInterface AddGenericParameter(string typeName, out ICSharpInterfaceGenericParameter param, Action<ICSharpInterfaceGenericParameter> configure = null);
    ICSharpInterface AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure);
    ICSharpInterface InsertMethod(int index, string returnType, string name, Action<ICSharpInterfaceMethod> configure = null);
    ICSharpInterface WithFieldsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines);
    ICSharpInterface WithPropertiesSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines);
    ICSharpInterface WithMethodsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines);
    ICSharpInterface WithMembersSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines);
    ICSharpInterface Internal();
    ICSharpInterface InternalProtected();
    ICSharpInterface Protected();
    ICSharpInterface Private();
    ICSharpInterface Partial();
    bool IsPartial { get; set; }
}