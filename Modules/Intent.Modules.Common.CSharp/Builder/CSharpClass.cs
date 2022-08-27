using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpClass
{
    public CSharpClass(string name)
    {
        Name = name.ToCSharpIdentifier();
    }
    public string Name { get; private set; }
    public string AccessModifier { get; private set; } = "public ";
    public string BaseType { get; set; }
    public IList<string> Interfaces { get; set; } = new List<string>();
    public IList<CSharpField> Fields { get; set; } = new List<CSharpField>();
    public IList<CSharpConstructor> Constructors { get; set; } = new List<CSharpConstructor>();
    public IList<CSharpProperty> Properties { get; set; } = new List<CSharpProperty>();
    public IList<CSharpMethod> Methods { get; set; } = new List<CSharpMethod>();

    public CSharpClass WithBaseType(string type)
    {
        return ExtendsClass(type);
    }

    public CSharpClass ExtendsClass(string type)
    {
        BaseType = type;
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
        var field = new CSharpField(type, name);
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public CSharpClass AddProperty(string type, string name, Action<CSharpProperty> configure = null)
    {
        var property = new CSharpProperty(type, name, this);
        Properties.Add(property);
        configure?.Invoke(property);
        return this;
    }

    public CSharpClass InsertProperty(int index, string type, string name, Action<CSharpProperty> configure = null)
    {
        var property = new CSharpProperty(type, name, this);
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

    public CSharpClass AddMethod(string returnType, string name, Action<CSharpMethod> configure, params string[] statements)
    {
        return InsertMethod(Methods.Count, returnType, name, configure, statements);
    }

    public CSharpClass InsertMethod(int index, string returnType, string name, Action<CSharpMethod> configure, params string[] statements)
    {
        var method = new CSharpMethod(returnType, name);
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
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

    public bool IsPartial { get; set; }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string indentation)
    {
        return $@"{indentation}{AccessModifier}{(IsPartial ? "partial " : "")}class {Name}{GetBaseTypes()}
{indentation}{{{GetMembers($"{indentation}    ")}
{indentation}}}";
    }

    private string GetBaseTypes()
    {
        var types = new List<string>();
        if (BaseType != null)
        {
            types.Add(BaseType);
        }

        foreach (var @interface in Interfaces)
        {
            types.Add(@interface);
        }

        return types.Any() ? $" : {string.Join(", ", types)}" : "";
    }

    private string GetMembers(string indentation)
    {
        var members = new List<string>();

        members.AddRange(Fields.Select(x => x.ToString(indentation)));
        members.AddRange(Properties.Select(x => x.ToString(indentation)));
        members.AddRange(Constructors.Select(x => x.ToString(indentation)));
        members.AddRange(Methods.Select(x => x.ToString(indentation)));

        return !members.Any() ? "" : $@"
{string.Join($@"

", members)}";
    }
}