#nullable enable
using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorCodeBlock : IRazorFileNodeBase<IRazorCodeBlock>, IBuildsCSharpMembers
{
    ICSharpExpression? Expression { get; set; }
    IList<ICodeBlock> Declarations { get; }
    IBuildsCSharpMembers AddField(string type, string name, Action<ICSharpField>? configure = null);
    IBuildsCSharpMembers AddProperty(string type, string name, Action<ICSharpProperty>? configure = null);
    IBuildsCSharpMembers AddMethod(string type, string name, Action<ICSharpClassMethod>? configure = null);
    IBuildsCSharpMembers AddClass(string name, Action<ICSharpClass>? configure = null);
}