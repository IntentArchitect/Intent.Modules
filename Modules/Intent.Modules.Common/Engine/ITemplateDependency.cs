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

    /// <summary>
    /// The resolved dependency must be accessible to <see cref="AccessibleTo"/>.
    /// </summary>
    IOutputTarget? AccessibleTo => null;

    /// <summary>
    /// If the <see cref="AccessibleTo"/> is <see langword="null"/> and the provided <paramref name="accessibleTo"/>
    /// is not <see langword="null"/> and the implementation supports it, a new instance of
    /// <see cref="ITemplateDependency"/> will be created with it.
    /// </summary>
    bool TryGetWithAccessibleTo(IOutputTarget? accessibleTo, [NotNullWhen(true)] out ITemplateDependency? templateDependency)
    {
        templateDependency = null;
        return false;
    }
}