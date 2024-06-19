#nullable enable
using System;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IHtmlAttribute : IEquatable<IHtmlAttribute>
{
    string Name { get; set; }
    string? Value { get; set; }
}