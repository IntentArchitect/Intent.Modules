#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

internal class RazorCodeBlock : CSharpClass, IRazorCodeBlock
{
    public RazorCodeBlock(IRazorFile file) : base("RazorClass", file)
    {
        RazorFile = file;
        Parent = file;
    }

    public IRazorFile RazorFile { get; }

    //public IBuildsCSharpMembers InsertField(int index, string type, string name, Action<ICSharpField> configure = null)
    //{
    //    var field = new CSharpField(type, name, this)
    //    {
    //        BeforeSeparator = CSharpCodeSeparatorType.NewLine,
    //        AfterSeparator = CSharpCodeSeparatorType.NewLine
    //    };
    //    Declarations.Insert(index, field);
    //    configure?.Invoke(field);
    //    return this;
    //}

    //public IBuildsCSharpMembers AddField(string type, string name, Action<ICSharpField> configure = null)
    //{
    //    return InsertField(Declarations.Count, type, name, configure);
    //}

    //public IBuildsCSharpMembers InsertProperty(int index, string type, string name, Action<ICSharpProperty> configure = null)
    //{
    //    var property = new CSharpProperty(type, name, this)
    //    {
    //        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
    //        AfterSeparator = CSharpCodeSeparatorType.EmptyLines
    //    };
    //    Declarations.Insert(index, property);
    //    configure?.Invoke(property);
    //    return this;
    //}

    //public IBuildsCSharpMembers AddProperty(string type, string name, Action<ICSharpProperty> configure = null)
    //{
    //    return InsertProperty(Declarations.Count, type, name, configure);
    //}

    //public IBuildsCSharpMembers InsertMethod(int index, string type, string name, Action<ICSharpClassMethod> configure = null)
    //{
    //    var method = new CSharpClassMethod(type, name, this)
    //    {
    //        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
    //        AfterSeparator = CSharpCodeSeparatorType.EmptyLines
    //    };
    //    Declarations.Insert(index, method);
    //    configure?.Invoke(method);
    //    return this;
    //}

    //public IBuildsCSharpMembers AddMethod(string type, string name, Action<ICSharpClassMethod> configure = null)
    //{
    //    return InsertMethod(Declarations.Count, type, name, configure);
    //}

    //public IBuildsCSharpMembers AddClass(string name, Action<ICSharpClass> configure = null)
    //{
    //    var @class = new CSharpClass(name, File)
    //    {
    //        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
    //        AfterSeparator = CSharpCodeSeparatorType.EmptyLines
    //    };
    //    Declarations.Add(@class);
    //    configure?.Invoke(@class);
    //    return this;
    //}

    ICSharpTemplate IBuildsCSharpMembers.Template => File.Template;

    //public int IndexOf(ICodeBlock codeBlock)
    //{
    //    return Declarations.IndexOf(codeBlock);
    //}

    public override string GetText(string indentation)
    {
        return $@"{indentation}@code {{
{string.Join(@"
", Declarations.ConcatCode(indentation + "    "))}
{indentation}}}
";
    }

    public void AddChildNode(IRazorFileNode node)
    {
        throw new NotImplementedException();
    }

    public void InsertChildNode(int index, IRazorFileNode node)
    {
        throw new NotImplementedException();
    }

    public IList<IRazorFileNode> ChildNodes { get; }
    public IRazorFileNode? Parent { get; set; }
}