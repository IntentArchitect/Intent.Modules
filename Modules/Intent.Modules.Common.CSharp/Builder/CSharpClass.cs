using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.Builder;
using JetBrains.Annotations;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpClass : CSharpDeclaration<CSharpClass>
{
    private CSharpCodeSeparatorType _propertiesSeparator = CSharpCodeSeparatorType.NewLine;
    private CSharpCodeSeparatorType _fieldsSeparator = CSharpCodeSeparatorType.NewLine;

    public CSharpClass(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
    }
    public string Name { get; private set; }
    protected string AccessModifier { get; private set; } = "public ";
    public CSharpClass BaseType { get; set; }
    public IList<string> Interfaces { get; } = new List<string>();
    public IList<CSharpField> Fields { get; } = new List<CSharpField>();
    public IList<CSharpConstructor> Constructors { get; } = new List<CSharpConstructor>();
    public IList<CSharpProperty> Properties { get; } = new List<CSharpProperty>();
    public IList<CSharpClassMethod> Methods { get; } = new List<CSharpClassMethod>();
    public IList<CSharpGenericParameter> GenericParameters { get; } = new List<CSharpGenericParameter>();
    public IList<CSharpClass> NestedClasses { get; } = new List<CSharpClass>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();

    public CSharpClass WithBaseType(string type)
    {
        return ExtendsClass(type);
    }

    public CSharpClass WithBaseType(CSharpClass type)
    {
        return ExtendsClass(type);
    }

    public CSharpClass ExtendsClass(string type)
    {
        BaseType = new CSharpClass(type);
        return this;
    }

    public CSharpClass ExtendsClass(CSharpClass @class)
    {
        BaseType = @class;
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

    public CSharpClass AddField(string type, string name, Action<CSharpField> configure = null)
    {
        var field = new CSharpField(type, name)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public CSharpClass AddProperty(string type, string name, Action<CSharpProperty> configure = null)
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

    public CSharpClass InsertProperty(int index, string type, string name, Action<CSharpProperty> configure = null)
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


    public CSharpClass AddConstructor(Action<CSharpConstructor> configure = null)
    {
        var ctor = new CSharpConstructor(this);
        Constructors.Add(ctor);
        configure?.Invoke(ctor);
        return this;
    }

    public CSharpClass AddMethod(string returnType, string name, Action<CSharpClassMethod> configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
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

    public CSharpClass AddNestedClass(string name, Action<CSharpClass> configure = null)
    {
        var @class = new CSharpClass(name);
        configure?.Invoke(@class);
        NestedClasses.Add(@class);
        return this;
    }

    public CSharpClass InsertMethod(int index, string returnType, string name, Action<CSharpClassMethod> configure = null)
    {
        var method = new CSharpClassMethod(returnType, name);
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public CSharpClass WithFieldsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        return this;
    }

    public CSharpClass WithPropertiesSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        return this;
    }

    public CSharpClassMethod FindMethod(string name)
    {
        return Methods.FirstOrDefault(x => x.Name == name);
    }

    public CSharpClassMethod FindMethod(Func<CSharpClassMethod, bool> matchFunc)
    {
        return Methods.FirstOrDefault(matchFunc);
    }

    public CSharpClass Internal()
    {
        AccessModifier = "internal ";
        return this;
    }
    public CSharpClass InternalProtected()
    {
        AccessModifier = "internal protected ";
        return this;
    }

    public CSharpClass Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpClass Private()
    {
        AccessModifier = "private ";
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
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{(IsSealed ? "sealed " : "")}{(IsStatic ? "static " : "")}{(IsAbstract ? "abstract " : "")}{(IsPartial ? "partial " : "")}class {Name}{GetGenericParameters()}{GetBaseTypes()}{GetGenericTypeConstraints(indentation)}
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

        return $"<{string.Join(", ", GenericParameters)}>";
    }

    private string GetBaseTypes()
    {
        var types = new List<string>();
        if (BaseType != null)
        {
            types.Add(BaseType.Name);
        }

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
        codeBlocks.AddRange(Constructors);
        codeBlocks.AddRange(Properties);
        codeBlocks.AddRange(Methods);
        codeBlocks.AddRange(NestedClasses.Select(s => new CSharpClassCodeBlock(s)));
        

        return $@"{string.Join($@"
", codeBlocks.ConcatCode(indentation))}";
    }

    private class CSharpClassCodeBlock : ICodeBlock
    {
        private readonly CSharpClass _class;

        public CSharpClassCodeBlock(CSharpClass @class)
        {
            _class = @class;
            BeforeSeparator = CSharpCodeSeparatorType.NewLine;
            AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        }
        public CSharpCodeSeparatorType BeforeSeparator { get; set; }
        public CSharpCodeSeparatorType AfterSeparator { get; set; }
        public string GetText(string indentation)
        {
            return _class.ToString(indentation);
        }
    }

//    private string GetMembers(string indentation)
//    {
//        var codeBlocks = new List<string>();

//        if (Fields.Any())
//            codeBlocks.Add(Fields.ConcatCode(indentation));
//        if (Constructors.Any())
//            codeBlocks.Add(Constructors.ConcatCode(indentation));
//        if (Properties.Any())
//            codeBlocks.Add(Properties.ConcatCode(indentation));
//        if (Methods.Any())
//            codeBlocks.Add(Methods.ConcatCode(indentation));

//        return !codeBlocks.Any() ? "" : $@"
//{string.Join($@"
//", codeBlocks)}";
//    }
}