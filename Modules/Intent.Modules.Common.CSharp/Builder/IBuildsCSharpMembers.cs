using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface IBuildsCSharpMembers : ICSharpCodeContext
{
    IList<ICodeBlock> Declarations { get; }
    IBuildsCSharpMembers AddField(string type, string name, Action<CSharpField> configure = null);
    IBuildsCSharpMembers AddProperty(string type, string name, Action<CSharpProperty> configure = null);
    IBuildsCSharpMembers AddMethod(string returnType, string name, Action<CSharpClassMethod> configure = null);
    IBuildsCSharpMembers AddClass(string name, Action<CSharpClass> configure = null);
}