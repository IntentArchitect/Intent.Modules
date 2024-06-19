#nullable enable
using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorCodeBlock : IRazorFileNodeBase<IRazorCodeBlock>, IBuildsCSharpMembersActual
{
    ICSharpExpression? Expression { get; set; }
    IList<ICodeBlock> Declarations { get; }
    IBuildsCSharpMembersActual AddField(string type, string name, Action<ICSharpField>? configure = null);
    IBuildsCSharpMembersActual AddProperty(string type, string name, Action<ICSharpProperty>? configure = null);
    IBuildsCSharpMembersActual AddMethod(string type, string name, Action<ICSharpClassMethod>? configure = null);
    IBuildsCSharpMembersActual AddClass(string name, Action<ICSharpClass>? configure = null);
}