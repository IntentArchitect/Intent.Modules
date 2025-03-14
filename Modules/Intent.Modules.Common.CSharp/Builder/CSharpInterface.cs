using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterface : CSharpDeclaration<CSharpInterface>, ICSharpInterface
{
    private readonly ICSharpInterface _wrapper;
    private CSharpCodeSeparatorType _fieldsSeparator = CSharpCodeSeparatorType.NewLine;
    private CSharpCodeSeparatorType _propertiesSeparator = CSharpCodeSeparatorType.NewLine;
    private CSharpCodeSeparatorType _methodsSeparator = CSharpCodeSeparatorType.NewLine;

    public CSharpInterface(string name) : this(
        name: name,
        file: null,
        parent: null)
    {
    }

    public CSharpInterface(string name, ICSharpFile file) : this(
        name: name,
        file: file,
        parent: file)
    {
    }

    public CSharpInterface(string name, ICSharpFile file, ICSharpCodeContext parent)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapper = new CSharpInterfaceWrapper(this);
        Name = name.ToCSharpIdentifier();
        File = file;
        Parent = parent;
    }

    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;

    public string Name { get; }
    internal string AccessModifier { get; private set; } = "public ";
    public IList<string> Interfaces { get; set; } = new List<string>();
    public IList<CSharpInterfaceField> Fields { get; } = new List<CSharpInterfaceField>();
    public IList<CSharpInterfaceProperty> Properties { get; } = new List<CSharpInterfaceProperty>();
    public IList<CSharpInterfaceMethod> Methods { get; } = new List<CSharpInterfaceMethod>();
    public IList<CSharpInterfaceGenericParameter> GenericParameters { get; } = new List<CSharpInterfaceGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();
    public IList<CSharpCodeBlock> CodeBlocks { get; } = new List<CSharpCodeBlock>();

    public CSharpInterface ExtendsInterface(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        Interfaces.Add(type);
        return this;
    }

    public CSharpInterface ImplementsInterfaces(IEnumerable<string> types)
    {
        foreach (var type in types)
            Interfaces.Add(type);

        return this;
    }

    public CSharpInterface ImplementsInterfaces(params string[] types)
    {
        foreach (var type in types)
            Interfaces.Add(type);

        return this;
    }

    public CSharpInterface AddField(string type, string name, Action<CSharpInterfaceField> configure = null)
    {
        var field = new CSharpInterfaceField(type, name, this)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public CSharpInterface AddProperty(string type, string name, Action<CSharpInterfaceProperty> configure = null)
    {
        var property = new CSharpInterfaceProperty(type, name, this)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Add(property);
        configure?.Invoke(property);
        return this;
    }

    public CSharpInterface InsertProperty(int index, string type, string name, Action<CSharpInterfaceProperty> configure = null)
    {
        var property = new CSharpInterfaceProperty(type, name, this)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Insert(index, property);
        configure?.Invoke(property);
        return this;
    }

    /// <summary>
    /// Resolves the type name and method name from the <paramref name="model"/> using the <see cref="ICSharpFileBuilderTemplate"/>
    /// template that was passed into the <see cref="CSharpFile"/>. Registers this method as representative of the <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public CSharpInterface AddMethod<TModel>(TModel model, Action<CSharpInterfaceMethod> configure = null)
        where TModel : IMetadataModel, IHasName
    {
        return AddMethod(File.GetModelType(model), model.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs), method =>
        {
            method.RepresentsModel(model);
            configure?.Invoke(method);
        });
    }
    
    public CSharpInterface AddMethod(CSharpType returnType, string name, Action<CSharpInterfaceMethod> configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }

    public CSharpInterface AddMethod(string returnType, string name, Action<CSharpInterfaceMethod> configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }

    public CSharpInterface AddCodeBlock(string codeLine)
    {
        CodeBlocks.Add(new CSharpCodeBlock(codeLine));
        return this;
    }

    public CSharpInterface AddGenericParameter(string typeName, Action<CSharpInterfaceGenericParameter> configure = null)
    {
        var param = new CSharpInterfaceGenericParameter(typeName);
        configure?.Invoke(param);
        GenericParameters.Add(param);
        return this;
    }

    public CSharpInterface AddGenericParameter(string typeName, out CSharpInterfaceGenericParameter param, Action<CSharpInterfaceGenericParameter> configure = null)
    {
        param = new CSharpInterfaceGenericParameter(typeName);
        configure?.Invoke(param);
        GenericParameters.Add(param);
        return this;
    }

    public CSharpInterface AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    public CSharpInterface InsertMethod(int index, string returnType, string name, Action<CSharpInterfaceMethod> configure = null)
    {
        var method = new CSharpInterfaceMethod(returnType, name, this)
        {
            BeforeSeparator = _methodsSeparator,
            AfterSeparator = _methodsSeparator
        };
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }
    
    public CSharpInterface InsertMethod(int index, CSharpType returnType, string name, Action<CSharpInterfaceMethod> configure = null)
    {
        var method = new CSharpInterfaceMethod(returnType, name, this)
        {
            BeforeSeparator = _methodsSeparator,
            AfterSeparator = _methodsSeparator
        };
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public CSharpInterface WithFieldsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        return this;
    }

    public CSharpInterface WithPropertiesSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        return this;
    }

    public CSharpInterface WithMethodsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _methodsSeparator = separator;
        return this;
    }

    public CSharpInterface WithMembersSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        _methodsSeparator = separator;
        return this;
    }

    public CSharpInterface Internal()
    {
        AccessModifier = "internal ";
        return this;
    }
    public CSharpInterface InternalProtected()
    {
        AccessModifier = "internal protected ";
        return this;
    }

    public CSharpInterface Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpInterface Private()
    {
        AccessModifier = "private ";
        return this;
    }
    public CSharpInterface Partial()
    {
        IsPartial = true;
        return this;
    }

    public bool IsPartial { get; set; }

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{(IsPartial ? "partial " : "")}interface {Name}{GetGenericParameters()}{GetBaseTypes()}{GetGenericTypeConstraints(indentation)}
{indentation}{{{GetMembers($"{indentation}    ")}
{indentation}}}";
    }

    private string GetGenericTypeConstraints(string indentation)
    {
        if (!GenericTypeConstraints.Any())
        {
            return string.Empty;
        }

        string newLine = $@"
{indentation}    ";
        return newLine + string.Join(newLine, GenericTypeConstraints);
    }

    private string GetGenericParameters()
    {
        if (!GenericParameters.Any())
        {
            return string.Empty;
        }

        return $"<{string.Join(", ", GenericParameters.Select(s => s.GetText()))}>";
    }

    private string GetBaseTypes()
    {
        var types = new List<string>();
        foreach (var @interface in Interfaces)
        {
            types.Add(@interface);
        }

        return types.Any() ? $" : {string.Join(", ", types)}" : "";
    }

    private string GetMembers(string indentation)
    {
        var elementOrder = File.StyleSettings?.ElementOrder.ToArray() ?? [];

        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Fields.OrderBy(f => Array.IndexOf(elementOrder, f.AccessModifier.Trim())));
        codeBlocks.AddRange(Properties.OrderBy(c => Array.IndexOf(elementOrder, c.AccessModifier.Trim())));
        codeBlocks.AddRange(Methods.OrderBy(m => Array.IndexOf(elementOrder, m.AccessModifier.Trim())).GroupBy(m => m.Name).SelectMany(g => g));
        codeBlocks.AddRange(CodeBlocks);

        return !codeBlocks.Any() ? "" : $@"{string.Join(@"
", codeBlocks.ConcatCode(indentation))}";
    }

    public string GetText(string indentation)
    {
        return ToString(indentation);
    }

    #region ICSharpInterface implementation

    IList<ICSharpInterfaceProperty> ICSharpInterface.Properties => _wrapper.Properties;

    IList<ICSharpInterfaceMethodDeclaration> ICSharpInterface.Methods => _wrapper.Methods;

    IList<ICSharpInterfaceGenericParameter> ICSharpInterface.GenericParameters => _wrapper.GenericParameters;

    IList<ICSharpGenericTypeConstraint> ICSharpInterface.GenericTypeConstraints => _wrapper.GenericTypeConstraints;

    IList<ICSharpCodeBlock> ICSharpInterface.CodeBlocks => _wrapper.CodeBlocks;

    IList<ICSharpInterfaceField> ICSharpInterface.Fields => _wrapper.Fields;

    ICSharpInterface ICSharpInterface.ImplementsInterfaces(IEnumerable<string> types)
    {
        return _wrapper.ImplementsInterfaces(types);
    }

    ICSharpInterface ICSharpInterface.ImplementsInterfaces(params string[] types)
    {
        return _wrapper.ImplementsInterfaces(types);
    }

    ICSharpInterface ICSharpInterface.AddField(string type, string name, Action<ICSharpInterfaceField> configure)
    {
        return _wrapper.AddField(type, name, configure);
    }

    ICSharpInterface ICSharpInterface.AddProperty(string type, string name, Action<ICSharpInterfaceProperty> configure)
    {
        return _wrapper.AddProperty(type, name, configure);
    }

    ICSharpInterface ICSharpInterface.InsertProperty(int index, string type, string name, Action<ICSharpInterfaceProperty> configure)
    {
        return _wrapper.InsertProperty(index, type, name, configure);
    }

    ICSharpInterface ICSharpInterface.AddMethod<TModel>(TModel model, Action<ICSharpInterfaceMethodDeclaration> configure)
    {
        return _wrapper.AddMethod(model, configure);
    }

    ICSharpInterface ICSharpInterface.AddMethod(string returnType, string name, Action<ICSharpInterfaceMethodDeclaration> configure)
    {
        return _wrapper.AddMethod(returnType, name, configure);
    }

    ICSharpInterface ICSharpInterface.AddCodeBlock(string codeLine)
    {
        return _wrapper.AddCodeBlock(codeLine);
    }

    ICSharpInterface ICSharpInterface.AddGenericParameter(string typeName, Action<ICSharpInterfaceGenericParameter> configure)
    {
        return _wrapper.AddGenericParameter(typeName, configure);
    }

    ICSharpInterface ICSharpInterface.AddGenericParameter(string typeName, out ICSharpInterfaceGenericParameter param,
        Action<ICSharpInterfaceGenericParameter> configure)
    {
        return _wrapper.AddGenericParameter(typeName, out param, configure);
    }

    ICSharpInterface ICSharpInterface.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure)
    {
        return _wrapper.AddGenericTypeConstraint(genericParameterName, configure);
    }

    ICSharpInterface ICSharpInterface.InsertMethod(int index, string returnType, string name, Action<ICSharpInterfaceMethodDeclaration> configure)
    {
        return _wrapper.InsertMethod(index, returnType, name, configure);
    }

    ICSharpInterface ICSharpInterface.WithFieldsSeparated(CSharpCodeSeparatorType separator)
    {
        return _wrapper.WithFieldsSeparated(separator);
    }

    ICSharpInterface ICSharpInterface.WithPropertiesSeparated(CSharpCodeSeparatorType separator)
    {
        return _wrapper.WithPropertiesSeparated(separator);
    }

    ICSharpInterface ICSharpInterface.WithMethodsSeparated(CSharpCodeSeparatorType separator)
    {
        return _wrapper.WithMethodsSeparated(separator);
    }

    ICSharpInterface ICSharpInterface.WithMembersSeparated(CSharpCodeSeparatorType separator)
    {
        return _wrapper.WithMembersSeparated(separator);
    }

    ICSharpInterface ICSharpInterface.Internal()
    {
        return _wrapper.Internal();
    }

    ICSharpInterface ICSharpInterface.InternalProtected()
    {
        return _wrapper.InternalProtected();
    }

    ICSharpInterface ICSharpInterface.Protected()
    {
        return _wrapper.Protected();
    }

    ICSharpInterface ICSharpInterface.Private()
    {
        return _wrapper.Private();
    }

    ICSharpInterface ICSharpInterface.Partial()
    {
        return _wrapper.Partial();
    }

    ICSharpInterface ICSharpInterface.ExtendsInterface(string type)
    {
        return _wrapper.ExtendsInterface(type);
    }

    ICSharpInterface ICSharpDeclaration<ICSharpInterface>.AddAttribute(string name, Action<ICSharpAttribute> configure)
    {
        return _wrapper.AddAttribute(name, configure);
    }

    ICSharpInterface ICSharpDeclaration<ICSharpInterface>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure)
    {
        return _wrapper.AddAttribute(attribute, configure);
    }

    ICSharpInterface ICSharpDeclaration<ICSharpInterface>.WithComments(string xmlComments)
    {
        return _wrapper.WithComments(xmlComments);
    }

    ICSharpInterface ICSharpDeclaration<ICSharpInterface>.WithComments(IEnumerable<string> xmlComments)
    {
        return _wrapper.WithComments(xmlComments);
    }

    #endregion
}