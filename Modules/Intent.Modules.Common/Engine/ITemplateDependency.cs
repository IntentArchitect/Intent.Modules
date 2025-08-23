#pragma warning disable IDE0130
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using Intent.Templates;

namespace Intent.Modules.Common;

public interface ITemplateDependency
{
    string? TemplateId { get; }
    bool IsMatch(ITemplate template);
}