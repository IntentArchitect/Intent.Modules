using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface IBuildsCSharpMembers : ICSharpCodeContext
{
    IList<ICodeBlock> Declarations { get; }
    IBuildsCSharpMembers InsertField(int index, string type, string name, Action<ICSharpField> configure = null);
    IBuildsCSharpMembers AddField(string type, string name, Action<ICSharpField> configure = null);
    IBuildsCSharpMembers InsertProperty(int index, string type, string name, Action<ICSharpProperty> configure = null);
    IBuildsCSharpMembers AddProperty(string type, string name, Action<ICSharpProperty> configure = null);
    IBuildsCSharpMembers InsertMethod(int index, string returnType, string name, Action<ICSharpClassMethod> configure = null);
    IBuildsCSharpMembers AddMethod(string returnType, string name, Action<ICSharpClassMethod> configure = null);
    IBuildsCSharpMembers AddClass(string name, Action<ICSharpClass> configure = null);
    int IndexOf(ICodeBlock codeBlock);
}