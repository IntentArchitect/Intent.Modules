#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.Typescript.Mapping;

public abstract partial class TypescriptMappingBase : ITypescriptMapping
{
    internal Dictionary<string, string> SourceReplacements = new();
    internal Dictionary<string, string?> TargetReplacements = new();

    public ICanBeReferencedType Model { get; }
    public IList<ITypescriptMapping> Children { get; }
    public ITypescriptMapping? Parent { get; set; }
    public IElementToElementMappedEnd Mapping { get; set; }

    protected readonly ITypescriptTemplate Template;

    [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
    protected TypescriptMappingBase(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ITypescriptFileBuilderTemplate template) : this(model, mapping, children, (ITypescriptTemplate)template)
    {
    }

    protected TypescriptMappingBase(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ITypescriptTemplate template)
    {
        Template = template;
        Model = model;
        Mapping = mapping;
        Children = children.Select(x => x.GetMapping()).ToList();
        foreach (var child in Children)
        {
            child.Parent = this;
        }
    }

    [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
    protected TypescriptMappingBase(MappingModel model, ITypescriptFileBuilderTemplate template) : this(model, (ITypescriptTemplate)template)
    {
    }

    protected TypescriptMappingBase(MappingModel model, ITypescriptTemplate template)
    {
        Template = template;
        Model = model.Model;
        Mapping = model.Mapping;
        Children = model.Children.Select(x => x.GetMapping()).ToList();
        foreach (var child in Children)
        {
            child.Parent = this;
        }
    }

    /// <summary>
    /// Returns the assignment statements, assigning the source statements to the target statements.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<TypescriptStatement> GetMappingStatements()
    {
        //TODO
        yield return new TypescriptStatement($"{GetTargetStatement()} = {GetSourceStatement()};");
    }

    public virtual TypescriptStatement GetSourceStatement(bool? withNullConditionalOperators = null)
    {
        return GetSourcePathText(withNullConditionalOperators ?? Mapping?.TargetElement.TypeReference?.IsNullable == true);
    }

    public virtual TypescriptStatement GetTargetStatement() => GetTargetStatement(false);
    public virtual TypescriptStatement GetTargetStatement(bool withNullConditionalOperators)
    {
        return GetTargetPathText(withNullConditionalOperators);
    }

    public virtual IDictionary<string, TypescriptStatement> GetExpressionMap()
    {
        var text = Mapping?.MappingExpression ?? throw new Exception($"Could not resolve source path. Mapping expected on '{Model.DisplayText ?? Model.Name}' [{Model.SpecializationType}]. Check that you have a MappingTypeResolver that addresses this scenario.");
        return GetParsedExpressionMap(text, path => GetSourcePathText(Mapping.GetSource(path).Path, false)).ToDictionary(x => x.Key, x => new TypescriptStatement(x.Value));
    }

    /// <summary>
    /// This method will return the source path to this node, even if it isn't itself mapped.
    /// In the case where it isn't mapped, it will work out it's mapping based on child mappings.
    /// </summary>
    /// <returns></returns>
    protected virtual IList<IElementMappingPathTarget> GetSourcePath()
    {
        // NOTE: This logic is now duplicated in the MappingModel class:
        if (Mapping != null)
        {
            return Mapping.SourcePath;
        }

        var childMappings = GetAllChildren().Where(c => c.Mapping != null).ToList();
        var mapping = childMappings.First().Mapping;
        if (childMappings.Count == 1)
        {
            return mapping.SourcePath.Take(mapping.SourcePath.Count - 1).ToList();
        }

        var toPath = mapping.SourcePath.Take(mapping.SourcePath.IndexOf(
                mapping.SourcePath.Last(x => childMappings.All(c => c.Mapping.SourcePath.Contains(x)))) + 1) // + 1 (inclusive)
            .ToList();
        return toPath;
    }

    protected IList<IElementMappingPathTarget> GetTargetPath()
    {
        // NOTE: This logic is now duplicated in the MappingModel class:
        if (Mapping != null)
        {
            return Mapping.TargetPath;
        }

        var mapping = GetAllChildren().First(x => x.Mapping != null).Mapping;
        var targetPath = mapping.TargetPath.Take(mapping.TargetPath.IndexOf(mapping.TargetPath.Single(x => x.Element.Id == Model.Id)) + 1).ToList();
        return targetPath;
    }

    protected IEnumerable<ITypescriptMapping> GetAllChildren()
    {
        return Children.Concat(Children.SelectMany(x => ((TypescriptMappingBase)x).GetAllChildren()).ToList());
    }

    public virtual void SetSourceReplacement(IMetadataModel type, string replacement)
    {
        SourceReplacements.Remove(type.Id);
        SourceReplacements.Add(type.Id, replacement);
    }

    public virtual bool TryGetSourceReplacement(IMetadataModel type, out string? replacement)
    {
        return SourceReplacements.TryGetValue(type.Id, out replacement) || Parent?.TryGetSourceReplacement(type, out replacement) == true;
    }

    public virtual void SetTargetReplacement(IMetadataModel type, string? replacement)
    {
        TargetReplacements.Remove(type.Id);
        TargetReplacements.Add(type.Id, replacement);
    }

    public virtual bool TryGetTargetReplacement(IMetadataModel type, out string? replacement)
    {
        return TargetReplacements.TryGetValue(type.Id, out replacement) || Parent?.TryGetTargetReplacement(type, out replacement) == true;
    }

    protected string GetSourcePathText() => GetSourcePathText(false);
    protected string GetSourcePathText(bool targetIsNullable)
    {
        var result = Mapping?.MappingExpression ?? throw new Exception($"Could not resolve source path. Mapping expected on '{Model.DisplayText ?? Model.Name}' [{Model.SpecializationType}]. Check that you have a MappingTypeResolver that addresses this scenario.");
        foreach (var map in GetParsedExpressionMap(Mapping.MappingExpression, path => GetSourcePathText(Mapping.GetSource(path).Path, targetIsNullable)))
        {
            result = result.Replace(map.Key, map.Value);
        }
        return result;
    }

    protected string? GetSourcePathText(IList<IElementMappingPathTarget> mappingPaths) => GetSourcePathText(mappingPaths, false);

    protected string? GetSourcePathText(IList<IElementMappingPathTarget> mappingPaths, bool targetIsNullable)
    {
        return GetSourcePathExpression(mappingPaths, targetIsNullable)?.ToString();
    }

    [GeneratedRegex(@"[\(\)?:!=+|&]")]
    private static partial Regex GetParsedExpressionMapRegex();

    protected IDictionary<string, string> GetParsedExpressionMap(string str, Func<string, string?> replacePathFunc)
    {
        var result = new Dictionary<string, string>();
        while (str.Contains('{', StringComparison.Ordinal) && str.Contains('}', StringComparison.Ordinal))
        {
            var fromPos = str.IndexOf('{', StringComparison.Ordinal) + 1;
            var toPos = str.IndexOf('}', StringComparison.Ordinal);
            var expression = str[fromPos..toPos];
            var expressionFromPos = 0;
            var resultExpression = expression;
            var parts = GetParsedExpressionMapRegex().Split(expression).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim());
            foreach (var part in parts)
            {
                // if is literal:
                if (part == "true" ||
                    part == "false" ||
                    part == "null" ||
                    (part.StartsWith('"') && part.EndsWith('"')))
                {
                    continue;
                }

                var replaceResult = replacePathFunc(part) ?? string.Empty;
                expressionFromPos = resultExpression.IndexOf(part, expressionFromPos, StringComparison.Ordinal);
                resultExpression = resultExpression.Remove(expressionFromPos, part.Length);
                resultExpression = resultExpression.Insert(expressionFromPos, replaceResult);
            }

            result.TryAdd($"{{{expression}}}", resultExpression);
            str = str[(str.IndexOf('}', StringComparison.Ordinal) + 1)..];
        }
        return result;
    }

    protected ITypescriptExpression? GetSourcePathExpression(IList<IElementMappingPathTarget> mappingPaths, bool targetIsNullable)
    {
        // TODO: GCB - The check for inheritance like this is a hack.
        // Consider making associations indicating inheritance a first-class citizen via the settings.
        // Alternatively, consider using the metadata to indicate this.
        mappingPaths = mappingPaths.Where(x => x.Element.SpecializationType != "Generalization Target End").ToList();

        TypescriptStatement? result = null;
        foreach (var mappingPathTarget in mappingPaths)
        {
            if (TryGetSourceReplacement(mappingPathTarget.Element, out var replacement))
            {
                if (replacement == string.Empty)
                {
                    continue;
                }
                result = !string.IsNullOrWhiteSpace(replacement) ? new TypescriptStatement(replacement) : null;
            }
            else
            {
                var relativeMappingPath = mappingPaths.Take(mappingPaths.IndexOf(mappingPathTarget) + 1).ToList();
                TypescriptStatement? member;
                if (TryGetReferenceName(mappingPathTarget, out var referenceName))
                {
                    member = new TypescriptStatement(referenceName);
                }
                else if (TryGetReference(relativeMappingPath, out var reference))
                {
                    member = new TypescriptStatement(reference.Name, (ITypescriptReferenceable)reference);
                }
                else
                {
                    // fall back on using the element's name from the metadata:
                    //If this is not expected, you have not set up a `Represents` inside of you template, note you will need to do this for each part of the mapping path
                    member = new TypescriptStatement(mappingPathTarget.Element.Name.ToCamelCase());
                }

                // TODO
                result = result != null ? new TypescriptStatement($"{result}.{member}") : $"{member}";

                // TODO look into if needed for Typescript
                //var previousMappingPath = mappingPaths.TakeWhile(x => x != mappingPathTarget).LastOrDefault();
                //if (targetIsNullable && IsTransitional(previousMappingPath, mappingPathTarget))
                //{
                //    if (previousMappingPath?.Element.TypeReference?.IsNullable == true)
                //        //&& result is CSharpAccessMemberStatement accessMember)
                //    {
                //        //accessMember.IsConditional();
                //    }
                //}
            }
        }

        return result;
    }

    private static bool IsTransitional(IElementMappingPathTarget? previousMappingPath, IElementMappingPathTarget mappingPathTarget)
    {
        if (previousMappingPath == null)
        {
            return false;
        }
        return !((IElement)previousMappingPath.Element).ChildElements.Contains((IElement)mappingPathTarget.Element);
    }

    protected string? GetTargetPathText(bool withNullConditionalOperators = false)
    {
        var targetPathExpression = GetTargetPathExpression(withNullConditionalOperators);
        if (targetPathExpression == null)
        {
            throw new Exception($"The target path text returned null for mapping {GetType().Name}. Check that you are resolving the appropriate mapping type for this model: {Model}");
        }
        return GetTargetPathExpression(withNullConditionalOperators)?.ToString();
    }

    protected TypescriptStatement? GetTargetPathExpression(bool withNullConditionalOperators = false)
    {
        TypescriptStatement? result = null;
        var mappingPaths = GetTargetPath();
        foreach (var mappingPathTarget in mappingPaths)
        {
            if (TryGetTargetReplacement(mappingPathTarget.Element, out var replacement))
            {
                if (string.IsNullOrWhiteSpace(replacement))
                {
                    continue;
                }
                result = !string.IsNullOrWhiteSpace(replacement) ? new TypescriptStatement(replacement) : null;
            }
            else
            {
                TypescriptStatement member = new TypescriptStatement("");
                var relativeMappingPath = mappingPaths.Take(mappingPaths.IndexOf(mappingPathTarget) + 1).ToList();
                if (TryGetReferenceName(mappingPathTarget, out var referenceName))
                {
                    member = new TypescriptStatement(referenceName);
                }
                else if (TryGetReference(relativeMappingPath, out var reference))
                {
                    member = new TypescriptStatement(reference.Name);//, (ITypescriptReferenceable)reference);
                }
                else
                {
                    // fall back on using the element's name from the metadata:
                    //If this is not expected, you have not set up a `Represents` inside of you template, note you will need to do this for each part of the mapping path
                    member = new TypescriptStatement(mappingPathTarget.Element.Name.ToCamelCase());
                }

                result = member;
                // TODO - is this needed?
                //var previousMappingPath = mappingPaths.TakeWhile(x => x != mappingPathTarget).LastOrDefault();
                //if (withNullConditionalOperators && IsTransitional(previousMappingPath, mappingPathTarget))
                //{
                //    if (previousMappingPath?.Element.TypeReference?.IsNullable == true
                //        && result is CSharpAccessMemberStatement accessMember)
                //    {
                //        accessMember.IsConditional();
                //    }
                //}
            }
        }
        return result;
    }

    private static bool CheckChildrenRecursive(IList<ITypescriptMapping> children, Func<ITypescriptMapping, bool> predicate)
    {
        return children.All(c => predicate(c) || CheckChildrenRecursive(c.Children, predicate));
    }

    private bool TryGetReference(IList<IElementMappingPathTarget> mappingPath, [NotNullWhen(true)] out IHasTypescriptName? reference)
    {
        reference = null;
        var mappingPathTarget = mappingPath.Last();
        var previousPathTarget = (IElement)mappingPath.TakeLast(2).First().Element;

        // First try the previous mapping element, but only if it has children (e.g. a command / query):
        if (previousPathTarget.ChildElements.Any() && TryFindTemplates(Template, previousPathTarget, out var foundTypeTemplates))
        {
            foreach (var context in foundTypeTemplates ?? [])
            {
                if (context.RootCodeContext.TryGetReferenceForModel(mappingPathTarget.Id, out reference))
                {
                    return true;
                }
            }
        }

        // Second try the previous mapping element's type reference (e.g. an attribute with a complex type):
        if (TryFindTemplates(Template, previousPathTarget.TypeReference?.Element as IElement, out foundTypeTemplates))
        {
            foreach (var context in foundTypeTemplates ?? [])
            {
                if (context.RootCodeContext.TryGetReferenceForModel(mappingPathTarget.Id, out reference))
                {
                    return true;
                }
            }
        }

        // Finally try the previous mapping element's parent (e.g. when you're mapping to an injected service's operation):
        if (TryFindTemplates(Template, previousPathTarget.ParentElement, out foundTypeTemplates))
        {
            foreach (var context in foundTypeTemplates ?? [])
            {
                if (context.RootCodeContext.TryGetReferenceForModel(mappingPathTarget.Id, out reference))
                {
                    return true;
                }
            }
        }

        // Finally, Finally, try the type reference's type reference. This is for when mapping from a property on an operation result DTO
        // e.g. The Response from a operation call is a DTO, and one of the DTO's fields is mapped
        if (TryFindTemplates(Template, previousPathTarget.TypeReference?.Element?.TypeReference?.Element as IElement, out foundTypeTemplates))
        {
            foreach (var context in foundTypeTemplates ?? [])
            {
                if (context.RootCodeContext.TryGetReferenceForModel(mappingPathTarget.Id, out reference))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryFindTemplates(ITypescriptTemplate template, IElement? elementToLookup, out IList<ITypescriptFileBuilderTemplate>? foundTemplates)
    {
        if (elementToLookup is null)
        {
            foundTemplates = null;
            return false;
        }

        foundTemplates = [];

        var foundTypeInfos = template.GetAllTypeInfo(elementToLookup.AsTypeReference());
        foreach (var foundTemplate in foundTypeInfos.Select(x => x.Template).OfType<ITypescriptFileBuilderTemplate>())
        {
            foundTemplates.Add(foundTemplate);
        }

        return foundTemplates.Any();
    }

    /// <summary>
    /// Resolve the reference name after all replacement have been applied for this specified <paramref name="mappingPath"/>
    /// By default this method will return false and supply a null <paramref name="reference"/>. Override this method for your
    /// particular use case if required.
    /// </summary>
    /// <param name="mappingPath"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    protected virtual bool TryGetReferenceName(IElementMappingPathTarget mappingPath, out string? reference)
    {
        reference = null;
        return false;
    }
}