#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpDeclaration<out TSpecialization> : ICSharpMetadataBase
    where TSpecialization : ICSharpDeclaration<TSpecialization>
{
    TSpecialization AddAttribute(string name, Action<ICSharpAttribute> configure = null);
    TSpecialization AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure = null);
    TSpecialization WithComments(string xmlComments);
    TSpecialization WithComments(IEnumerable<string> xmlComments);
}