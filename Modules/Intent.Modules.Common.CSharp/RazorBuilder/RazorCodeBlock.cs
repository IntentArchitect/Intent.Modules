using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public class RazorCodeBlock : RazorFileNodeBase<RazorCodeBlock>, IRazorFileNode, IBuildsCSharpMembers
{
    public ICSharpExpression Expression { get; set; }
    public IList<ICodeBlock> Declarations { get; } = new List<ICodeBlock>();

    public RazorCodeBlock(RazorFile file) : base(file)
    {
        Parent = file;
    }

    public IBuildsCSharpMembers AddField(string type, string name, Action<CSharpField> configure = null)
    {
        var field = new CSharpField(type, name, this)
        {
            BeforeSeparator = CSharpCodeSeparatorType.NewLine,
            AfterSeparator = CSharpCodeSeparatorType.NewLine
        };
        Declarations.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public IBuildsCSharpMembers AddProperty(string type, string name, Action<CSharpProperty> configure = null)
    {
        var property = new CSharpProperty(type, name, this)
        {
            BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
            AfterSeparator = CSharpCodeSeparatorType.EmptyLines
        };
        Declarations.Add(property);
        configure?.Invoke(property);
        return this;
    }

    public IBuildsCSharpMembers AddMethod(string type, string name, Action<CSharpClassMethod> configure = null)
    {
        var method = new CSharpClassMethod(type, name, this)
        {
            BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
            AfterSeparator = CSharpCodeSeparatorType.EmptyLines
        };
        Declarations.Add(method);
        configure?.Invoke(method);
        return this;
    }

    public IBuildsCSharpMembers AddClass(string name, Action<CSharpClass> configure = null)
    {
        var @class = new CSharpClass(name, RazorFile)
        {
            BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
            AfterSeparator = CSharpCodeSeparatorType.EmptyLines
        };
        Declarations.Add(@class);
        configure?.Invoke(@class);
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{indentation}@code {{
{string.Join("", ChildNodes.Select(x => x.GetText($"{indentation}    ")))}{string.Join(@"
", Declarations.ConcatCode(indentation + "    "))}
{indentation}}}
";
    }
}