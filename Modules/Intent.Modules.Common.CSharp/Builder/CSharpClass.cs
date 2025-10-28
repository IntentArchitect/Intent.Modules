#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;
using static Intent.Modules.Common.CSharp.Builder.CSharpClass;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using System.Xml;
using System.Security.Claims;
using System.Reflection.Metadata;
using Intent.Modules.Common.Types.Api;
using System.Reflection;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using System.Xml.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpClass : CSharpDeclaration<CSharpClass>, ICSharpClass
{
    private readonly ICSharpClass _wrapper;
    private readonly Type _type;
    private CSharpCodeSeparatorType _propertiesSeparator = CSharpCodeSeparatorType.NewLine;
    private CSharpCodeSeparatorType _fieldsSeparator = CSharpCodeSeparatorType.NewLine;

    internal Type TypeDefinitionType => _type;

    protected internal CSharpClass(string name, Type type) : this(
        name: name,
        type: type,
        file: null,
        parent: null)
    {
    }

    protected internal CSharpClass(string name, Type type, ICSharpFile file) : this(
        name: name,
        type: type,
        file: file,
        parent: file)
    {
    }

    protected internal CSharpClass(string name, Type type, ICSharpFile? file, ICSharpCodeContext? parent)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapper = new CSharpClassWrapper(this);
        _type = type;
        Name = name;
        File = file;
        Parent = parent;
        
        _propertiesSeparator = GetConfiguredSeparator(_propertiesSeparator);
        _fieldsSeparator = GetConfiguredSeparator(_fieldsSeparator);
    }

    public CSharpClass(string name) : this(name, Type.Class)
    {
    }

    public CSharpClass(string name, ICSharpFile file) : this(name, Type.Class, file)
    {
    }

    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public string Name { get; }
    internal string AccessModifier { get; private set; } = "public ";
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public CSharpClass? BaseType { get; set; }
    public IList<string> BaseTypeTypeParameters { get; } = new List<string>();
    public IList<string> Interfaces { get; } = new List<string>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();

    public IList<CSharpField> Fields { get; } = new List<CSharpField>();
    public IList<CSharpConstructor> Constructors { get; } = new List<CSharpConstructor>();
    public IList<CSharpProperty> Properties { get; } = new List<CSharpProperty>();
    public IList<CSharpClassMethod> Methods { get; } = new List<CSharpClassMethod>();
    public IList<CSharpClass> NestedClasses { get; } = new List<CSharpClass>();
    public IList<CSharpInterface> NestedInterfaces { get; } = new List<CSharpInterface>();
    public IList<CSharpCodeBlock> CodeBlocks { get; } = new List<CSharpCodeBlock>();

    public IList<ICodeBlock> Declarations
    {
        get
        {
            var elementOrder = File.StyleSettings?.ElementOrder.ToArray() ?? [];

            var codeBlocks = new List<ICodeBlock>();
            codeBlocks.AddRange(Fields.Where(p => !p.IsOmittedFromRender).OrderBy(f => Array.IndexOf(elementOrder, $"{f.AccessModifier.Trim()} {(f.IsReadOnly ? "readonly" : string.Empty)}{(f.IsConstant ? "const" : string.Empty)}".Trim())));
            codeBlocks.AddRange(Constructors.Where(p => !p.IsPrimaryConstructor).OrderBy(c => Array.IndexOf(elementOrder, c.AccessModifier.Trim())));
            codeBlocks.AddRange(Properties.Where(p => !p.IsOmittedFromRender).OrderBy(c => Array.IndexOf(elementOrder, c.AccessModifier.Trim())));
            codeBlocks.AddRange(Methods.OrderBy(m => Array.IndexOf(elementOrder, m.AccessModifier.Trim())).GroupBy(m => m.Name).SelectMany(g => g));
            codeBlocks.AddRange(NestedClasses.OrderBy(c => Array.IndexOf(elementOrder, c.AccessModifier.Trim())).GroupBy(m => m.Name).SelectMany(g => g));
            codeBlocks.AddRange(NestedInterfaces);
            codeBlocks.AddRange(CodeBlocks);
            return codeBlocks;
        }
    }

    public CSharpClass WithBaseType(string type)
    {
        return ExtendsClass(type, []);
    }

    public CSharpClass WithBaseType(string type, IEnumerable<string> genericTypeParameters)
    {
        return ExtendsClass(type, genericTypeParameters);
    }

    public CSharpClass WithBaseType(CSharpClass type)
    {
        return ExtendsClass(type, []);
    }

    public CSharpClass WithBaseType(CSharpClass type, IEnumerable<string> genericTypeParameters)
    {
        return ExtendsClass(type, genericTypeParameters);
    }

    public CSharpClass ExtendsClass(string type)
    {
        return ExtendsClass(new CSharpClass(type), []);
    }

    public CSharpClass ExtendsClass(string type, IEnumerable<string> genericTypeParameters)
    {
        return ExtendsClass(new CSharpClass(type), genericTypeParameters);
    }

    public CSharpClass ExtendsClass(CSharpClass @class)
    {
        return ExtendsClass(@class, []);
    }

    public CSharpClass ExtendsClass(CSharpClass @class, IEnumerable<string> genericTypeParameters)
    {
        BaseType = @class;
        foreach (var genericTypeParameter in genericTypeParameters)
        {
            BaseTypeTypeParameters.Add(genericTypeParameter);
        }

        return this;
    }

    public CSharpClass ImplementsInterface(string type)
    {
        Interfaces.Add(type);
        return this;
    }

    public CSharpClass ImplementsInterfaces(IEnumerable<string> types)
    {
        foreach (var type in types)
            Interfaces.Add(type);

        return this;
    }

    public IBuildsCSharpMembers InsertField(int index, string type, string name, Action<ICSharpField>? configure = null)
    {
        var field = new CSharpField(type, name, this)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Fields.Insert(index, field);
        configure?.Invoke(field);
        return this;
    }

    IBuildsCSharpMembers IBuildsCSharpMembers.AddField(string type, string name, Action<ICSharpField>? configure) => AddField(type, name, configure);
    IBuildsCSharpMembers IBuildsCSharpMembers.InsertProperty(int index, string type, string name, Action<ICSharpProperty> configure)
    {
        return InsertProperty(index, type, name, configure);
    }

    public CSharpClass AddField(string type, string name, Action<CSharpField>? configure = null)
    {
        var field = new CSharpField(type, name, this)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    IBuildsCSharpMembers IBuildsCSharpMembers.AddProperty(string type, string name, Action<ICSharpProperty>? configure) => AddProperty(type, name, configure);

    ICSharpClass ICSharpClass.AddProperty(string type, string name, Action<ICSharpProperty>? configure) => AddProperty(type, name, configure);

    public CSharpClass AddProperty(string type, string name, Action<CSharpProperty>? configure = null)
    {
        var property = new CSharpProperty(type, name, this)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Add(property);
        configure?.Invoke(property);
        return this;
    }

    /// <summary>
    /// Resolves the property name from the <paramref name="model"/>. Registers this property as representative of the <paramref name="model"/>.
    /// </summary>
    public CSharpClass AddProperty<TModel>(string type, TModel model, Action<CSharpProperty>? configure = null)
        where TModel : IMetadataModel, IHasName
    {
        return AddProperty(type, model.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper), prop =>
        {
            prop.RepresentsModel(model);
            configure?.Invoke(prop);
        });
    }

    /// <summary>
    /// Resolves the type name and property name from the <paramref name="model"/> using the <see cref="ICSharpFileBuilderTemplate"/>
    /// template that was passed into the <see cref="CSharpFile"/>. Registers this property as representative of the <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public CSharpClass AddProperty<TModel>(TModel model, Action<CSharpProperty>? configure = null)
        where TModel : IMetadataModel, IHasName
    {
        return AddProperty(File.GetModelType(model), model.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper), prop =>
        {
            prop.RepresentsModel(model);
            configure?.Invoke(prop);
        });
    }

    public CSharpClass InsertProperty(int index, string type, string name, Action<CSharpProperty>? configure = null)
    {
        var property = new CSharpProperty(type, name, this)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Insert(index, property);
        configure?.Invoke(property);
        return this;
    }

    public CSharpClass AddConstructor(Action<CSharpConstructor>? configure = null)
    {
        var ctor = new CSharpConstructor(this);
        Constructors.Add(ctor);
        configure?.Invoke(ctor);
        return this;
    }

    public CSharpClass AddPrimaryConstructor(Action<CSharpConstructor>? configure = null)
    {
        var ctor = new CSharpConstructor(this, true);
        Constructors.Add(ctor);
        configure?.Invoke(ctor);
        return this;
    }

    /// <summary>
    /// Will add a null-forgiving constructor to the class. This will add a parameterless constructor, which sets all relevent and qualifying
    /// properties on the class to null!. The constructor will only be added if there are qualifiying properties
    /// </summary>
    /// <returns>This class</returns>
    public CSharpClass AddNullForgivingConstructor(Action<CSharpConstructor>? configure = null)
    {
        List<CSharpStatement> ctorStatements = [];
        foreach (var property in Properties)
        {
            if (!string.IsNullOrWhiteSpace(property.InitialValue))
            {
                continue;
            }

            bool addStatement = false;

            if (property.RepresentedModel is IHasTypeReference || property.RepresentedModel is ITypeReference)
            {
                var typeReference = (property.RepresentedModel as IHasTypeReference)?.TypeReference ?? property.RepresentedModel as ITypeReference;

                // based on above checks, this should not be null
                if (typeReference != null)
                {
                    var typeInfo = File.Template?.GetTypeInfo(typeReference);
                    if (typeInfo != null)
                    {
                        addStatement = NeedsNullabilityAssignment(typeInfo);
                    }
                    else
                    {
                        addStatement = NeedsNullabilityAssignment(property.Type);
                    }
                }
            }
            else
            {
                addStatement = NeedsNullabilityAssignment(property.Type);
            }

            if (addStatement)
            {
                ctorStatements.Add(new CSharpStatement($"// IntentMerge"));
                ctorStatements.Add(new CSharpStatement($"{property.Name} = null!;"));
            }
        }

        if (ctorStatements.Count > 0)
        {
            var ctor = new CSharpConstructor(this, false);
            configure?.Invoke(ctor);
            ctor.AddStatements(ctorStatements);
            Constructors.Add(ctor);
        }

        return this;
    }

    IBuildsCSharpMembers IBuildsCSharpMembers.InsertMethod(int index, string returnType, string name, Action<ICSharpClassMethodDeclaration> configure) => InsertMethod(index, returnType, name, configure);
    public CSharpClass InsertMethod(int index, string returnType, string name, Action<CSharpClassMethod>? configure = null)
    {
        var method = new CSharpClassMethod(returnType, name, this);
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public CSharpClass InsertMethod(int index, CSharpType returnType, string name, Action<CSharpClassMethod>? configure = null)
    {
        var method = new CSharpClassMethod(returnType, name, this);
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    ICSharpClass ICSharpClass.AddMethod(string returnType, string name, Action<ICSharpClassMethodDeclaration>? configure)
    {
        return AddMethod(returnType, name, configure);
    }
    
    IBuildsCSharpMembers IBuildsCSharpMembers.AddMethod(string returnType, string name, Action<ICSharpClassMethodDeclaration>? configure) => AddMethod(returnType, name, configure);
    public CSharpClass AddMethod(string returnType, string name, Action<CSharpClassMethod>? configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }



    /// <summary>
    /// Resolves the method name from the <paramref name="model"/>. Registers this method as representative of the <paramref name="model"/>.
    /// </summary>
    public CSharpClass AddMethod<TModel>(string returnType, TModel model, Action<CSharpClassMethod>? configure = null)
        where TModel : IMetadataModel, IHasName
    {
        return AddMethod(returnType, model.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs), prop =>
        {
            prop.RepresentsModel(model);
            configure?.Invoke(prop);
        });
    }

    public CSharpClass AddMethod<TModel>(TModel returnType, Action<CSharpClassMethod>? configure = null)
        where TModel : IMetadataModel, IHasName
    {
        return AddMethod(File.GetModelType(returnType), returnType.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs), prop =>
        {
            prop.RepresentsModel(returnType);
            configure?.Invoke(prop);
        });
    }

    public CSharpClass AddMethod(CSharpType returnType, string name, Action<CSharpClassMethod>? configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }

    public CSharpClass AddCodeBlock(string codeLine)
    {
        CodeBlocks.Add(new CSharpCodeBlock(codeLine));
        return this;
    }

    public CSharpClass AddGenericParameter(string typeName)
    {
        var param = new CSharpGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }

    public CSharpClass AddGenericParameter(string typeName, out CSharpGenericParameter param)
    {
        param = new CSharpGenericParameter(typeName);
        GenericParameters.Add(param);
        return this;
    }

    public CSharpClass AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    IBuildsCSharpMembers IBuildsCSharpMembers.AddClass(string name, Action<ICSharpClass>? configure) => AddNestedClass(name, configure);
    ICSharpTemplate IBuildsCSharpMembers.Template => File.Template;

    public int IndexOf(ICodeBlock codeBlock)
    {
        switch (codeBlock)
        {
            case CSharpField field:
                return Fields.IndexOf(field);
            case CSharpConstructor ctor:
                return Constructors.IndexOf(ctor);
            case CSharpProperty property:
                return Properties.IndexOf(property);
            case CSharpClassMethod method:
                return Methods.IndexOf(method);
            case CSharpClass @class:
                return NestedClasses.IndexOf(@class);
            case CSharpInterface @interface:
                return NestedInterfaces.IndexOf(@interface);
            default:
                throw new ArgumentOutOfRangeException(nameof(codeBlock));
        }
    }

    public CSharpClass AddNestedClass<TModel>(TModel model, Action<CSharpClass>? configure = null)
         where TModel
        : IMetadataModel, IHasName
    {
        return AddNestedClass(model, model.Name.ToPascalCase(), configure);
    }

    public CSharpClass AddNestedClass<TModel>(TModel model, string name, Action<CSharpClass>? configure = null)
         where TModel
        : IMetadataModel, IHasName
    {
        TemplateInstanceRegistry.Register(new NestedClassTypeInfoResolver(this.File.Template, model, name));
        return AddNestedClass(name, configure);
    }

    public CSharpClass AddNestedClass(string name, Action<CSharpClass>? configure = null)
    {
        var @class = new CSharpClass(
            name: name,
            type: Type.Class,
            file: File,
            parent: this);

        configure?.Invoke(@class);
        NestedClasses.Add(@class);
        return this;
    }

    public CSharpClass AddNestedRecord(string name, Action<ICSharpClass>? configure = null)
    {
        var @class = new CSharpRecord(
            name: name,
            file: File,
            parent: this);

        configure?.Invoke(@class);
        NestedClasses.Add(@class);
        return this;
    }

    public CSharpClass AddNestedInterface(string name, Action<CSharpInterface>? configure = null)
    {
        var @interface = new CSharpInterface(
            name: name,
            file: File,
            parent: this);

        configure?.Invoke(@interface);
        NestedInterfaces.Add(@interface);
        return this;
    }

    public CSharpClass WithFieldsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = GetConfiguredSeparator(separator);
        return this;
    }

    public CSharpClass WithPropertiesSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = GetConfiguredSeparator(separator);
        return this;
    }
    
    public CSharpClassMethod? FindMethod(string name)
    {
        return Methods.FirstOrDefault(x => x.Name == name);
    }

    public CSharpClassMethod? FindMethod(Func<CSharpClassMethod, bool> matchFunc)
    {
        return Methods.FirstOrDefault(matchFunc);
    }

    public CSharpClass Internal()
    {
        AccessModifier = "internal ";
        return this;
    }

    /// <summary>
    /// Obsolete. Use <see cref="ProtectedInternal"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpClass InternalProtected()
    {
        AccessModifier = "internal protected ";
        return this;
    }

    public CSharpClass Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpClass Protected()
    {
        AccessModifier = "protected ";
        return this;
    }

    public CSharpClass ProtectedInternal()
    {
        AccessModifier = "protected internal ";
        return this;
    }

    public CSharpClass Public()
    {
        AccessModifier = "public ";
        return this;
    }

    public CSharpClass Partial()
    {
        IsPartial = true;
        return this;
    }

    public CSharpClass Sealed()
    {
        IsSealed = true;
        return this;
    }

    public CSharpClass Unsealed()
    {
        IsSealed = false;
        return this;
    }


    public CSharpClass Abstract()
    {
        if (IsStatic)
        {
            throw new InvalidOperationException("Cannot make class abstract if it has already been declared as static");
        }

        IsAbstract = true;

        return this;
    }

    public CSharpClass Static()
    {
        if (IsAbstract)
        {
            throw new InvalidOperationException("Cannot make class static if it has already been declared as abstract");
        }

        IsStatic = true;

        return this;
    }

    public IEnumerable<CSharpClass> GetParentPath()
    {
        if (BaseType == null)
        {
            return Array.Empty<CSharpClass>();
        }

        return BaseType.GetParentPath().Concat(new[] { BaseType });
    }

    public IEnumerable<CSharpProperty> GetAllProperties()
    {
        return (BaseType?.GetAllProperties() ?? new List<CSharpProperty>()).Concat(Properties).ToList();
    }

    public bool IsPartial { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsStatic { get; set; }
    public bool IsSealed { get; set; }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string indentation)
    {
        var sb = new StringBuilder();

        sb.Append(GetComments(indentation));
        sb.Append(GetAttributes(indentation));
        sb.Append(indentation);
        sb.Append(AccessModifier);

        if (IsSealed)
        {
            sb.Append("sealed ");
        }

        if (IsStatic)
        {
            sb.Append("static ");
        }

        if (IsAbstract)
        {
            sb.Append("abstract ");
        }

        if (IsPartial)
        {
            sb.Append("partial ");
        }

        var primaryConstructor = GetPrimaryConstructor();

        sb.Append(_type.ToString().ToLowerInvariant());
        sb.Append(' ');
        sb.Append(Name);
        sb.Append(GetGenericParameters());
        if (primaryConstructor is not null)
        {
            sb.Append(primaryConstructor.GetText(indentation));
        }
        sb.Append(GetBaseTypes());
        sb.Append(GetGenericTypeConstraints(indentation));

        var members = GetMembers($"{indentation}    ");
        if (primaryConstructor is not null && string.IsNullOrEmpty(members))
        {
            sb.Append(';');
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine();
            sb.Append(indentation);
            sb.Append('{');
            sb.Append(members);
            sb.AppendLine();
            sb.Append(indentation);
            sb.Append('}');
        }

        return sb.ToString();
    }

    public virtual string GetText(string indentation)
    {
        return ToString(indentation);
    }
    
    private CSharpCodeSeparatorType GetConfiguredSeparator(CSharpCodeSeparatorType defaultSeparator)
    {
        var setting = File?.StyleSettings?.BlankLineBetweenMembers;
        return setting != null && setting.IsAlways()
            ? CSharpCodeSeparatorType.EmptyLines
            : defaultSeparator;
    }
    
    private CSharpConstructor? GetPrimaryConstructor()
    {
        var primaryConstructors = Constructors.Where(p => p.IsPrimaryConstructor).ToArray();
        return primaryConstructors.Length switch
        {
            0 => null,
            1 => primaryConstructors.First(),
            _ => throw new InvalidOperationException($"Cannot have more than one primary constructor for {_type} {Name}")
        };
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

        return $"<{string.Join(", ", GenericParameters)}>";
    }

    private string GetBaseTypes()
    {
        var types = new List<string>();
        if (BaseType is not null)
        {
            var baseType = BaseType.Name;

            var primaryCtor = GetPrimaryConstructor();
            var baseCallParams = primaryCtor?.ConstructorCall?.Arguments;
            if (baseCallParams?.Count > 0)
            {
                baseType += $"({string.Join(", ", baseCallParams)})";
            }

            if (BaseTypeTypeParameters.Any())
            {
                baseType += $"<{string.Join(", ", BaseTypeTypeParameters)}>";
            }

            types.Add(baseType);
        }

        types.AddRange(Interfaces);

        return types.Any() ? $" : {string.Join(", ", types)}" : "";
    }

    private string GetMembers(string indentation)
    {
        var codeBlocks = Declarations;

        return $"{string.Join(Environment.NewLine, codeBlocks.ConcatCode(indentation))}";
    }

    /// <summary>
    /// Used to determine if a type can get the nullable assignment or not (null!)
    /// </summary>
    /// <param name="typeInfo">The resolved tyupe info</param>
    /// <returns>Flag indicating if a nullability assignment is required</returns>
    private static bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
    {
        return !(typeInfo.IsPrimitive
                 || typeInfo.IsNullable == true
                 || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.IsEnumModel())
                 || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.SpecializationType == "Generic Type"));
    }

    /// <summary>
    /// This is a fallback way to determine nullability and is not 100% accurate
    /// The NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo) should ideally be used, which means when adding
    /// a property to this class, the overload which takes in a "model" should be used
    /// </summary>
    /// <param name="type">The string representation of the type</param>
    /// <returns>Flag indicating if a nullability assignment is required</returns>
    private static bool NeedsNullabilityAssignment(string type)
    {
        return !(CSharpType.NonNullableValueTypes.Contains(type) || type.EndsWith('?') || CSharpType.CollectionMap.Keys.Any(k => type.StartsWith(k)));
    }

    protected internal enum Type
    {
        Class,
        Record
    }

    #region ICSharpClass implementation

    ICSharpClass? ICSharpClass.BaseType
    {
        get => _wrapper.BaseType;
        set => _wrapper.BaseType = value;
    }

    IList<ICSharpField> ICSharpClass.Fields => _wrapper.Fields;

    IList<ICSharpConstructor> ICSharpClass.Constructors => _wrapper.Constructors;

    IList<ICSharpProperty> ICSharpClass.Properties => _wrapper.Properties;

    IList<ICSharpClassMethodDeclaration> ICSharpClass.Methods => _wrapper.Methods;

    IList<ICSharpGenericParameter> ICSharpClass.GenericParameters => _wrapper.GenericParameters;

    IList<ICSharpClass> ICSharpClass.NestedClasses => _wrapper.NestedClasses;

    IList<ICSharpInterface> ICSharpClass.NestedInterfaces => _wrapper.NestedInterfaces;

    IList<ICSharpGenericTypeConstraint> ICSharpClass.GenericTypeConstraints => _wrapper.GenericTypeConstraints;

    IList<ICSharpCodeBlock> ICSharpClass.CodeBlocks => _wrapper.CodeBlocks;

    IList<ICSharpAttribute> ICSharpClass.Attributes => _wrapper.Attributes;

    ICSharpXmlComments ICSharpClass.XmlComments => _wrapper.XmlComments;

    ICSharpClass ICSharpClass.WithBaseType(string type, IEnumerable<string> genericTypeParameters)
    {
        return _wrapper.WithBaseType(type, genericTypeParameters);
    }

    ICSharpClass ICSharpClass.WithBaseType(ICSharpClass type)
    {
        return _wrapper.WithBaseType(type);
    }

    ICSharpClass ICSharpClass.WithBaseType(ICSharpClass type, IEnumerable<string> genericTypeParameters)
    {
        return _wrapper.WithBaseType(type, genericTypeParameters);
    }

    ICSharpClass ICSharpClass.ExtendsClass(string type)
    {
        return _wrapper.ExtendsClass(type);
    }

    ICSharpClass ICSharpClass.ExtendsClass(string type, IEnumerable<string> genericTypeParameters)
    {
        return _wrapper.ExtendsClass(type, genericTypeParameters);
    }

    ICSharpClass ICSharpClass.ExtendsClass(ICSharpClass @class)
    {
        return _wrapper.ExtendsClass(@class);
    }

    ICSharpClass ICSharpClass.ExtendsClass(ICSharpClass @class, IEnumerable<string> genericTypeParameters)
    {
        return _wrapper.ExtendsClass(@class, genericTypeParameters);
    }

    ICSharpClass ICSharpClass.ImplementsInterface(string type)
    {
        return _wrapper.ImplementsInterface(type);
    }

    ICSharpClass ICSharpClass.ImplementsInterfaces(IEnumerable<string> types)
    {
        return _wrapper.ImplementsInterfaces(types);
    }

    ICSharpClass ICSharpClass.AddProperty<TModel>(string type, TModel model, Action<ICSharpProperty>? configure)
    {
        return _wrapper.AddProperty(type, model, configure);
    }

    ICSharpClass ICSharpClass.AddProperty<TModel>(TModel model, Action<ICSharpProperty>? configure)
    {
        return _wrapper.AddProperty(model, configure);
    }

    ICSharpClass ICSharpClass.InsertProperty(int index, string type, string name, Action<CSharpProperty>? configure)
    {
        return _wrapper.InsertProperty(index, type, name, configure);
    }

    ICSharpClass ICSharpClass.AddConstructor(Action<ICSharpConstructor>? configure)
    {
        return _wrapper.AddConstructor(configure);
    }

    ICSharpClass ICSharpClass.AddPrimaryConstructor(Action<ICSharpConstructor>? configure)
    {
        return _wrapper.AddPrimaryConstructor(configure);
    }

    ICSharpClass ICSharpClass.AddMethod<TModel>(string returnType, TModel model, Action<ICSharpClassMethodDeclaration>? configure)
    {
        return _wrapper.AddMethod(returnType, model, configure);
    }

    ICSharpClass ICSharpClass.AddMethod<TModel>(TModel model, Action<ICSharpClassMethodDeclaration>? configure)
    {
        return _wrapper.AddMethod(model, configure);
    }

    ICSharpClass ICSharpClass.AddCodeBlock(string codeLine)
    {
        return _wrapper.AddCodeBlock(codeLine);
    }

    ICSharpClass ICSharpClass.AddGenericParameter(string typeName)
    {
        return _wrapper.AddGenericParameter(typeName);
    }

    ICSharpClass ICSharpClass.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        return _wrapper.AddGenericParameter(typeName, out param);
    }

    ICSharpClass ICSharpClass.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint> configure)
    {
        return _wrapper.AddGenericTypeConstraint(genericParameterName, configure);
    }

    ICSharpClass ICSharpClass.AddNestedClass(string name, Action<ICSharpClass>? configure)
    {
        return _wrapper.AddNestedClass(name, configure);
    }
    
    ICSharpClass ICSharpClass.AddNestedRecord(string name, Action<ICSharpClass>? configure)
    {
        return _wrapper.AddNestedRecord(name, configure);
    }

    ICSharpClass ICSharpClass.AddNestedInterface(string name, Action<ICSharpInterface>? configure)
    {
        return _wrapper.AddNestedInterface(name, configure);
    }

    ICSharpClass ICSharpClass.InsertMethod(int index, string returnType, string name, Action<ICSharpClassMethodDeclaration>? configure)
    {
        return _wrapper.InsertMethod(index, returnType, name, configure);
    }

    ICSharpClass ICSharpClass.WithFieldsSeparated(CSharpCodeSeparatorType separator)
    {
        return _wrapper.WithFieldsSeparated(separator);
    }

    ICSharpClass ICSharpClass.WithPropertiesSeparated(CSharpCodeSeparatorType separator)
    {
        return _wrapper.WithPropertiesSeparated(separator);
    }

    ICSharpClassMethodDeclaration? ICSharpClass.FindMethod(string name)
    {
        return _wrapper.FindMethod(name);
    }

    ICSharpClassMethodDeclaration? ICSharpClass.FindMethod(Func<ICSharpClassMethodDeclaration, bool> matchFunc)
    {
        return _wrapper.FindMethod(matchFunc);
    }

    ICSharpClass ICSharpClass.Internal()
    {
        return _wrapper.Internal();
    }

    ICSharpClass ICSharpClass.InternalProtected()
    {
        return _wrapper.InternalProtected();
    }

    ICSharpClass ICSharpClass.Private()
    {
        return _wrapper.Private();
    }

    ICSharpClass ICSharpClass.Protected()
    {
        return _wrapper.Protected();
    }

    ICSharpClass ICSharpClass.ProtectedInternal()
    {
        return _wrapper.ProtectedInternal();
    }

    ICSharpClass ICSharpClass.Public()
    {
        return _wrapper.Public();
    }

    ICSharpClass ICSharpClass.Partial()
    {
        return _wrapper.Partial();
    }

    ICSharpClass ICSharpClass.Sealed()
    {
        return _wrapper.Sealed();
    }

    ICSharpClass ICSharpClass.Unsealed()
    {
        return _wrapper.Unsealed();
    }

    ICSharpClass ICSharpClass.Abstract()
    {
        return _wrapper.Abstract();
    }

    ICSharpClass ICSharpClass.Static()
    {
        return _wrapper.Static();
    }

    IEnumerable<ICSharpClass> ICSharpClass.GetParentPath()
    {
        return _wrapper.GetParentPath();
    }

    IEnumerable<ICSharpProperty> ICSharpClass.GetAllProperties()
    {
        return _wrapper.GetAllProperties();
    }

    ICSharpClass ICSharpClass.WithBaseType(string type)
    {
        return _wrapper.WithBaseType(type);
    }

    ICSharpClass ICSharpClass.AddMetadata<T>(string key, T value)
    {
        return _wrapper.AddMetadata(key, value);
    }

    ICSharpClass ICSharpClass.RepresentsModel(IMetadataModel model)
    {
        return _wrapper.RepresentsModel(model);
    }

    IEnumerable<ICSharpAttribute> ICSharpDeclaration<ICSharpClass>.Attributes => Attributes;

    ICSharpClass ICSharpDeclaration<ICSharpClass>.AddAttribute(string name, Action<ICSharpAttribute> configure)
    {
        return _wrapper.AddAttribute(name, configure);
    }

    ICSharpClass ICSharpDeclaration<ICSharpClass>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure)
    {
        return _wrapper.AddAttribute(attribute, configure);
    }

    ICSharpClass ICSharpDeclaration<ICSharpClass>.WithComments(string xmlComments)
    {
        return _wrapper.WithComments(xmlComments);
    }

    ICSharpClass ICSharpDeclaration<ICSharpClass>.WithComments(IEnumerable<string> xmlComments)
    {
        return _wrapper.WithComments(xmlComments);
    }

    ICSharpClass ICSharpClass.AddNullForgivingConstructor(Action<CSharpConstructor>? configure)
    {
        return _wrapper.AddNullForgivingConstructor(configure);
    }

    #endregion
}