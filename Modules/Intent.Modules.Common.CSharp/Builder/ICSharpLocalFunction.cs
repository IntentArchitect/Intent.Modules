#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpLocalFunction : ICSharpMethod<ICSharpLocalFunction>
{
    new IHasCSharpStatements Parent { get; }
    IEnumerable<ICSharpAttribute> Attributes { get; }
    ICSharpLocalFunction AddAttribute(string name, Action<ICSharpAttribute>? configure = null);
    ICSharpLocalFunction AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute>? configure = null);
}