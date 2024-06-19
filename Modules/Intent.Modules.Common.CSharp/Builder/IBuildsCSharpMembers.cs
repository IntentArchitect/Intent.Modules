using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// <see cref="IBuildsCSharpMembers"/> can't be updated for backwards compatibility reasons, so
/// this was created for interfaces which needed to be "clean" of concretes.
/// </summary>
public interface IBuildsCSharpMembersActual : ICSharpCodeContext
{
    IList<ICodeBlock> Declarations { get; }
    IBuildsCSharpMembersActual AddField(string type, string name, Action<ICSharpField> configure = null);
    IBuildsCSharpMembersActual AddProperty(string type, string name, Action<ICSharpProperty> configure = null);
    IBuildsCSharpMembersActual AddMethod(string returnType, string name, Action<ICSharpClassMethod> configure = null);
    IBuildsCSharpMembersActual AddClass(string name, Action<ICSharpClass> configure = null);
}

public interface IBuildsCSharpMembers : IBuildsCSharpMembersActual
{
    IBuildsCSharpMembers AddField(string type, string name, Action<CSharpField> configure = null);
    IBuildsCSharpMembers AddProperty(string type, string name, Action<CSharpProperty> configure = null);
    IBuildsCSharpMembers AddMethod(string returnType, string name, Action<CSharpClassMethod> configure = null);
    IBuildsCSharpMembers AddClass(string name, Action<CSharpClass> configure = null);
}