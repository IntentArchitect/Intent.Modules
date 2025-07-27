#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping;

public abstract partial class CSharpMappingBase : ICSharpMapping
{
    internal Dictionary<string, string> SourceReplacements = new();
    internal Dictionary<string, string?> TargetReplacements = new();

    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public ICSharpMapping? Parent { get; set; }
    public IElementToElementMappedEnd Mapping { get; set; }

    protected readonly ICSharpTemplate Template;

    [Obsolete("Use constructor which accepts ICSharpTemplate instead of ICSharpFileBuilderTemplate. This will be removed in later version.")]
    protected CSharpMappingBase(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : this(model, mapping, children, (ICSharpTemplate)template)
    {
    }

    protected CSharpMappingBase(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpTemplate template)
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
    protected CSharpMappingBase(MappingModel model, ICSharpFileBuilderTemplate template) : this(model, (ICSharpTemplate)template)
    {
    }

    protected CSharpMappingBase(MappingModel model, ICSharpTemplate template)
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

    public virtual IEnumerable<CSharpStatement> GetMappingStatements()
    {
        yield return new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement());
    }

    public virtual CSharpStatement GetSourceStatement(bool? withNullConditionalOperators = null)
    {
        return GetSourcePathText(withNullConditionalOperators ?? Mapping?.TargetElement.TypeReference?.IsNullable == true);
    }

    public virtual CSharpStatement GetTargetStatement() => GetTargetStatement(false);
    public virtual CSharpStatement GetTargetStatement(bool withNullConditionalOperators)
    {
        return GetTargetPathText(withNullConditionalOperators);
    }

    public virtual IDictionary<string, CSharpStatement> GetExpressionMap()
    {
        var text = Mapping?.MappingExpression ?? throw new Exception($"Could not resolve source path. Mapping expected on '{Model.DisplayText ?? Model.Name}' [{Model.SpecializationType}]. Check that you have a MappingTypeResolver that addresses this scenario.");
        return GetParsedExpressionMap(text, path => GetSourcePathText(Mapping.GetSource(path).Path, false)).ToDictionary(x => x.Key, x => new CSharpStatement(x.Value));
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

    protected IEnumerable<ICSharpMapping> GetAllChildren()
    {
        return Children.Concat(Children.SelectMany(x => ((CSharpMappingBase)x).GetAllChildren()).ToList());
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

    protected ICSharpExpression? GetSourcePathExpression(IList<IElementMappingPathTarget> mappingPaths, bool targetIsNullable)
    {
        // TODO: GCB - The check for inheritance like this is a hack.
        // Consider making associations indicating inheritance a first-class citizen via the settings.
        // Alternatively, consider using the metadata to indicate this.
        mappingPaths = mappingPaths.Where(x => x.Element.SpecializationType != "Generalization Target End").ToList();

        CSharpStatement? result = null;
        foreach (var mappingPathTarget in mappingPaths)
        {
            if (TryGetSourceReplacement(mappingPathTarget.Element, out var replacement))
            {
                if (replacement == string.Empty)
                {
                    continue;
                }
                result = !string.IsNullOrWhiteSpace(replacement) ? new CSharpStatement(replacement) : null;
            }
            else
            {
                var relativeMappingPath = mappingPaths.Take(mappingPaths.IndexOf(mappingPathTarget) + 1).ToList();
                CSharpStatement? member;
                if (TryGetReferenceName(mappingPathTarget, out var referenceName))
                {
                    member = new CSharpStatement(referenceName);
                }
                else if (TryGetReference(relativeMappingPath, out var reference))
                {
                    member = new CSharpStatement(reference.Name, (ICSharpReferenceable)reference);
                }
                else
                {
                    // fall back on using the element's name from the metadata:
                    //If this is not expected, you have not set up a `Represents` inside of you template, note you will need to do this for each part of the mapping path
                    member = new CSharpStatement(mappingPathTarget.Element.Name);
                }

                result = result != null ? new CSharpAccessMemberStatement(result, member) : member;
                var previousMappingPath = mappingPaths.TakeWhile(x => x != mappingPathTarget).LastOrDefault();
                if (targetIsNullable && IsTransitional(previousMappingPath, mappingPathTarget))
                {
                    if (previousMappingPath?.Element.TypeReference?.IsNullable == true
                        && result is CSharpAccessMemberStatement accessMember)
                    {
                        accessMember.IsConditional();
                    }
                }
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

    protected ICSharpExpression? GetTargetPathExpression(bool withNullConditionalOperators = false)
    {
        CSharpStatement? result = null;
        var mappingPaths = GetTargetPath();
        foreach (var mappingPathTarget in mappingPaths)
        {
            if (TryGetTargetReplacement(mappingPathTarget.Element, out var replacement))
            {
                if (replacement == string.Empty)
                {
                    continue;
                }
                result = !string.IsNullOrWhiteSpace(replacement) ? new CSharpStatement(replacement) : null;
            }
            else
            {
                CSharpStatement member;
                var relativeMappingPath = mappingPaths.Take(mappingPaths.IndexOf(mappingPathTarget) + 1).ToList();
                if (TryGetReferenceName(mappingPathTarget, out var referenceName))
                {
                    member = new CSharpStatement(referenceName);
                }
                else if (TryGetReference(relativeMappingPath, out var reference))
                {
                    member = new CSharpStatement(reference.Name, (ICSharpReferenceable)reference);
                }
                else
                {
                    // fall back on using the element's name from the metadata:
                    //If this is not expected, you have not set up a `Represents` inside of you template, note you will need to do this for each part of the mapping path
                    member = new CSharpStatement(mappingPathTarget.Element.Name);
                }

                result = result != null ? new CSharpAccessMemberStatement(result, member, member.Reference) : member;
                var previousMappingPath = mappingPaths.TakeWhile(x => x != mappingPathTarget).LastOrDefault();
                if (withNullConditionalOperators && IsTransitional(previousMappingPath, mappingPathTarget))
                {
                    if (previousMappingPath?.Element.TypeReference?.IsNullable == true
                        && result is CSharpAccessMemberStatement accessMember)
                    {
                        accessMember.IsConditional();
                    }
                }
            }
        }

        return result;
    }

    protected CSharpStatement GetNullableAwareInstantiation(ICanBeReferencedType model, IList<ICSharpMapping> children, CSharpStatement instantiationStatement)
    {
        // Only go for Target Elements that are Nullable and that have children whose source mappings have a Map path length that is beyond the root Element.
        // e.g. We won't target "request.FieldName" (flat mappings pose problems) but rather "request.NavProp.FieldName" for source elements.
        if (model is IElement { TypeReference: { IsNullable: true, IsCollection: false } } &&
            CheckChildrenRecursive(children, c => c.Mapping != null && c.Mapping.SourcePath.SkipLast(1).Count() > 1) &&
            GetSourcePath().Last().Element.TypeReference.IsNullable)
        {
            // GCB - this code (now commented out) was seriously hacky and broke in a simple use case of assigning a DTO to a Model (UI)
            //var child = children.First();
            //var accessPath = child.Mapping.SourcePath.SkipLast(1).Select(s => child.TryGetSourceReplacement(s.Element, out var a) ? a : s.Name).ToArray();
            return new CSharpConditionalExpressionStatement($"{GetSourcePathText(GetSourcePath(), true)} is not null", instantiationStatement, "null");
        }

        return instantiationStatement;
    }

    private static bool CheckChildrenRecursive(IList<ICSharpMapping> children, Func<ICSharpMapping, bool> predicate)
    {
        return children.All(c => predicate(c) || CheckChildrenRecursive(c.Children, predicate));
    }

    private bool TryGetReference(IList<IElementMappingPathTarget> mappingPath, [NotNullWhen(true)] out IHasCSharpName? reference)
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

    private static bool TryFindTemplates(ICSharpTemplate template, IElement? elementToLookup, out IList<ICSharpFileBuilderTemplate>? foundTemplates)
    {
        if (elementToLookup is null)
        {
            foundTemplates = null;
            return false;
        }

        foundTemplates = [];

        var foundTypeInfos = template.GetAllTypeInfo(elementToLookup.AsTypeReference());
        foreach (var foundTemplate in foundTypeInfos.Select(x => x.Template).OfType<ICSharpFileBuilderTemplate>())
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