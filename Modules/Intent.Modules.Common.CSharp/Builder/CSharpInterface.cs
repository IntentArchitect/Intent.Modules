using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterface : CSharpDeclaration<CSharpInterface>, ICSharpReferenceable, ICodeBlock
{
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

        Name = name.ToCSharpIdentifier();
        File = file;
        Parent = parent;
    }

    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;

    public string Name { get; }
    protected string AccessModifier { get; private set; } = "public ";
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
        var field = new CSharpInterfaceField(type, name)
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
        return AddMethod(File.GetModelType(model), model.Name.ToPropertyName(), method =>
        {
            method.RepresentsModel(model);
            configure?.Invoke(method);
        });
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
        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Fields);
        codeBlocks.AddRange(Properties);
        codeBlocks.AddRange(Methods);
        codeBlocks.AddRange(CodeBlocks);

        return !codeBlocks.Any() ? "" : $@"{string.Join(@"
", codeBlocks.ConcatCode(indentation))}";
    }

    public string GetText(string indentation)
    {
        return ToString(indentation);
    }
}