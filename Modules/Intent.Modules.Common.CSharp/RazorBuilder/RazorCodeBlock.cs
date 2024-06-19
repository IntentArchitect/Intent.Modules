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

    public IBuildsCSharpMembersActual AddField(string type, string name, Action<ICSharpField>? configure = null)
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

    public IBuildsCSharpMembersActual AddProperty(string type, string name, Action<ICSharpProperty>? configure = null)
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

    public IBuildsCSharpMembersActual AddMethod(string type, string name, Action<ICSharpClassMethod>? configure = null)
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

    public IBuildsCSharpMembersActual AddClass(string name, Action<ICSharpClass>? configure = null)
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