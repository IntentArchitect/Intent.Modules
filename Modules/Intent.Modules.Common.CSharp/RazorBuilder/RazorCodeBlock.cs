#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

internal class RazorCodeBlock : RazorFileNodeBase<RazorCodeBlock, IRazorCodeBlock>, IRazorCodeBlock
{
    public ICSharpExpression? Expression { get; set; }
    public IList<ICodeBlock> Declarations { get; } = new List<ICodeBlock>();

    public RazorCodeBlock(IRazorFile file) : base(file)
    {
        Parent = file;
    }

    public IBuildsCSharpMembers InsertField(int index, string type, string name, Action<CSharpField> configure = null)
    {
        var field = new CSharpField(type, name, this)
        {
            BeforeSeparator = CSharpCodeSeparatorType.NewLine,
            AfterSeparator = CSharpCodeSeparatorType.NewLine
        };
        Declarations.Insert(index, field);
        configure?.Invoke(field);
        return this;
    }

    public IBuildsCSharpMembers AddField(string type, string name, Action<CSharpField> configure = null)
    {
        return InsertField(Declarations.Count, type, name, configure);
    }

    public IBuildsCSharpMembers InsertProperty(int index, string type, string name, Action<CSharpProperty> configure = null)
    {
        var property = new CSharpProperty(type, name, this)
        {
            BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
            AfterSeparator = CSharpCodeSeparatorType.EmptyLines
        };
        Declarations.Insert(index, property);
        configure?.Invoke(property);
        return this;
    }

    public IBuildsCSharpMembers AddProperty(string type, string name, Action<CSharpProperty> configure = null)
    {
        return InsertProperty(Declarations.Count, type, name, configure);
    }

    public IBuildsCSharpMembers InsertMethod(int index, string type, string name, Action<CSharpClassMethod> configure = null)
    {
        var method = new CSharpClassMethod(type, name, this)
        {
            BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
            AfterSeparator = CSharpCodeSeparatorType.EmptyLines
        };
        Declarations.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public IBuildsCSharpMembers AddMethod(string type, string name, Action<CSharpClassMethod> configure = null)
    {
        return InsertMethod(Declarations.Count, type, name, configure);
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

    public int IndexOf(ICodeBlock codeBlock)
    {
        return Declarations.IndexOf(codeBlock);
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