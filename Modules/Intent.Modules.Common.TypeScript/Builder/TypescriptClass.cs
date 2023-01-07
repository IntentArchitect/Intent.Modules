using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptClass : TypescriptDeclaration<TypescriptClass>
{
    private TypescriptCodeSeparatorType _accessorsSeparator = TypescriptCodeSeparatorType.NewLine;
    private TypescriptCodeSeparatorType _fieldsSeparator = TypescriptCodeSeparatorType.NewLine;

    public TypescriptClass(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
    }

    public string Name { get; }
    public TypescriptClass BaseType { get; set; }
    public List<string> Interfaces { get; } = new();
    public List<TypescriptField> Fields { get; } = new();
    public List<TypescriptConstructor> Constructors { get; } = new();
    public List<TypescriptAccessor> Getters { get; } = new();
    public List<TypescriptAccessor> Setters { get; } = new();
    public List<TypescriptMethod> Methods { get; } = new();

    public TypescriptClass WithBaseType(string type)
    {
        return ExtendsClass(type);
    }

    public TypescriptClass WithBaseType(TypescriptClass type)
    {
        return ExtendsClass(type);
    }

    public TypescriptClass ExtendsClass(string type)
    {
        BaseType = new TypescriptClass(type);
        return this;
    }

    public TypescriptClass ExtendsClass(TypescriptClass @class)
    {
        BaseType = @class;
        return this;
    }

    public TypescriptClass ImplementsInterface(string type)
    {
        Interfaces.Add(type);
        return this;
    }

    public TypescriptClass ImplementsInterfaces(IEnumerable<string> types)
    {
        foreach (var type in types)
            Interfaces.Add(type);

        return this;
    }

    public TypescriptClass AddField(string type, string name, Action<TypescriptField> configure = null)
    {
        var field = new TypescriptField(type, name)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public TypescriptClass AddGetter(string type, string name, Action<TypescriptAccessor> configure = null)
    {
        var getter = TypescriptAccessor.Getter(type, name);
        getter.BeforeSeparator = _accessorsSeparator;
        getter.AfterSeparator = _accessorsSeparator;

        Getters.Add(getter);
        configure?.Invoke(getter);
        return this;
    }

    public TypescriptClass InsertGetter(int index, string type, string name, Action<TypescriptAccessor> configure = null)
    {
        var getter = TypescriptAccessor.Getter(type, name);
        getter.BeforeSeparator = _accessorsSeparator;
        getter.AfterSeparator = _accessorsSeparator;

        Getters.Insert(index, getter);
        configure?.Invoke(getter);
        return this;
    }

    public TypescriptClass AddSetter(string type, string name, Action<TypescriptAccessor> configure = null)
    {
        var setter = TypescriptAccessor.Setter(type, name);
        setter.BeforeSeparator = _accessorsSeparator;
        setter.AfterSeparator = _accessorsSeparator;

        Setters.Add(setter);
        configure?.Invoke(setter);
        return this;
    }

    public TypescriptClass InsertSetter(int index, string type, string name, Action<TypescriptAccessor> configure = null)
    {
        var setter = TypescriptAccessor.Setter(type, name);
        setter.BeforeSeparator = _accessorsSeparator;
        setter.AfterSeparator = _accessorsSeparator;

        Setters.Insert(index, setter);
        configure?.Invoke(setter);
        return this;
    }

    public TypescriptClass AddConstructor(Action<TypescriptConstructor> configure = null)
    {
        var ctor = new TypescriptConstructor(this);
        Constructors.Add(ctor);
        configure?.Invoke(ctor);
        return this;
    }

    public TypescriptClass AddMethod(string returnType, string name, Action<TypescriptMethod> configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }

    public TypescriptClass InsertMethod(int index, string returnType, string name, Action<TypescriptMethod> configure = null)
    {
        var method = new TypescriptMethod(returnType, name);
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public TypescriptClass WithFieldsSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        return this;
    }

    public TypescriptClass WithAccessorsSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _accessorsSeparator = separator;
        return this;
    }

    public TypescriptMethod FindMethod(string name)
    {
        return Methods.FirstOrDefault(x => x.Name == name);
    }

    public TypescriptMethod FindMethod(Func<TypescriptMethod, bool> matchFunc)
    {
        return Methods.FirstOrDefault(matchFunc);
    }

    public TypescriptClass Abstract()
    {
        if (IsStatic)
        {
            throw new InvalidOperationException("Cannot make class abstract if it has already been declared as static");
        }

        IsAbstract = true;

        return this;
    }

    public TypescriptClass Static()
    {
        if (IsAbstract)
        {
            throw new InvalidOperationException("Cannot make class static if it has already been declared as abstract");
        }

        IsStatic = true;

        return this;
    }

    public IEnumerable<TypescriptClass> GetParentPath()
    {
        if (BaseType == null)
        {
            return Array.Empty<TypescriptClass>();
        }

        return BaseType.GetParentPath().Concat(new[] { BaseType });
    }

    public IEnumerable<TypescriptAccessor> GetAllGetters()
    {
        return (BaseType?.GetAllGetters() ?? new List<TypescriptAccessor>()).Concat(Getters).ToList();
    }

    public IEnumerable<TypescriptAccessor> GetAllSetters()
    {
        return (BaseType?.GetAllSetters() ?? new List<TypescriptAccessor>()).Concat(Setters).ToList();
    }

    public bool IsAbstract { get; set; }
    public bool IsStatic { get; set; }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string indentation)
    {
        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{(IsStatic ? "static " : "")}{(IsAbstract ? "abstract " : "")}class{Name}{GetBaseTypes()} {{
{indentation}{GetMembers($"{indentation}    ")}
{indentation}}}";
    }

    private string GetBaseTypes()
    {
        var types = new StringBuilder();

        if (BaseType != null)
        {
            types.Append($" extends {BaseType.Name}");
        }

        if (Interfaces.Count > 0)
        {
            types.Append($" implements {string.Join(", ", Interfaces)}");
        }

        return types.ToString();
    }

    private string GetMembers(string indentation)
    {
        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Fields);
        codeBlocks.AddRange(Constructors);
        codeBlocks.AddRange(Getters);
        codeBlocks.AddRange(Setters);
        codeBlocks.AddRange(Methods);

        return $@"{string.Join(@"
", codeBlocks.ConcatCode(indentation))}";
    }
}