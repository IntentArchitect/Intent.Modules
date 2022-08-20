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
    public string AccessModifier { get; private set; } = "public";
    public string BaseType { get; set; }
    public IList<CSharpField> Fields { get; set; } = new List<CSharpField>();
    public IList<CSharpConstructor> Constructors { get; set; } = new List<CSharpConstructor>();
    public IList<CSharpProperty> Properties { get; set; } = new List<CSharpProperty>();
    public IList<CSharpMethod> Methods { get; set; } = new List<CSharpMethod>();

    public CSharpClass WithBaseType(string type)
    {
        BaseType = type;
        return this;
    }

    public CSharpField AddField(string type, string name)
    {
        var field = new CSharpField(type, name);
        Fields.Add(field);
        return field;
    }

    public CSharpProperty AddProperty(string type, string name)
    {
        var property = new CSharpProperty(type, name);
        Properties.Add(property);
        return property;
    }

    public CSharpConstructor AddConstructor()
    {
        var ctor = new CSharpConstructor(this);
        Constructors.Add(ctor);
        return ctor;
    }

    public CSharpMethod AddMethod(string returnType, string name)
    {
        var method = new CSharpMethod(returnType, name);
        Methods.Add(method);
        return method;
    }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string indentation)
    {
        return $@"{indentation}{AccessModifier} class {Name}{GetBaseTypes()}
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
{indentation}{string.Join($@"

{indentation}", members)}";
    }
}