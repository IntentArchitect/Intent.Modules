#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
// ReSharper disable CheckNamespace
using System.Diagnostics.CodeAnalysis;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Common;

public interface ITemplateDependency
{
    string? TemplateId { get; }
    bool IsMatch(ITemplate template);
    IOutputTarget? Source => null;

    /// <summary>
    /// If the <see cref="Source"/> is <see langword="null"/> and the provided <paramref name="source"/>
    /// is not <see langword="null"/> and the implementation supports it, a new instance of
    /// <see cref="ITemplateDependency"/> will be created with it.
    /// </summary>
    bool TryGetWithSource(IOutputTarget? source, [NotNullWhen(true)] out ITemplateDependency? templateDependency)
    {
        templateDependency = null;
        return false;
    }
}