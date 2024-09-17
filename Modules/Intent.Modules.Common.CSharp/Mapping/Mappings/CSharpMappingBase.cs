﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.Mapping;

public abstract class CSharpMappingBase : ICSharpMapping
{
    protected Dictionary<string, string> _sourceReplacements = new();
    protected Dictionary<string, string> _targetReplacements = new();


    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public ICSharpMapping Parent { get; set; }
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

    //protected CSharpMappingBase(MappingModel mappingModel, ICSharpFileBuilderTemplate template)
    //{
    //    Template = template;
    //    Model = mappingModel.Model;
    //    Mapping = mappingModel.Mapping;
    //    Children = mappingModel.GetChildMappings(template);
    //}

    public virtual IEnumerable<CSharpStatement> GetMappingStatements()
    {
        yield return new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement());
    }

    //public CSharpStatement GetSourceStatement()
    //{
    //    return GetSourcePathText(true);
    //}

    public virtual CSharpStatement GetSourceStatement(bool? targetIsNullable = default)
    {
        return GetSourcePathText(targetIsNullable ?? Mapping?.TargetElement.TypeReference?.IsNullable == true);
    }

    public virtual CSharpStatement GetTargetStatement()
    {
        return GetTargetPathText();
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
    protected IList<IElementMappingPathTarget> GetSourcePath()
    {
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

    public void SetSourceReplacement(IMetadataModel type, string replacement)
    {
        if (_sourceReplacements.ContainsKey(type.Id))
        {
            _sourceReplacements.Remove(type.Id);
        }

        _sourceReplacements.Add(type.Id, replacement);
        //foreach (var child in Children)
        //{
        //    child.SetSourceReplacement(type, replacement);
        //}
    }

    public virtual bool TryGetSourceReplacement(IMetadataModel type, out string replacement)
    {
        return _sourceReplacements.TryGetValue(type.Id, out replacement) || Parent?.TryGetSourceReplacement(type, out replacement) == true;
    }

    public void SetTargetReplacement(IMetadataModel type, string replacement)
    {
        if (_targetReplacements.ContainsKey(type.Id))
        {
            _targetReplacements.Remove(type.Id);
        }

        _targetReplacements.Add(type.Id, replacement);
        //foreach (var child in Children)
        //{
        //    child.SetTargetReplacement(type, replacement);
        //}
    }

    public virtual bool TryGetTargetReplacement(IMetadataModel type, out string replacement)
    {
        return _targetReplacements.TryGetValue(type.Id, out replacement) || Parent?.TryGetTargetReplacement(type, out replacement) == true;
    }

    protected string GetSourcePathText() => GetSourcePathText(false);
    protected string GetSourcePathText(bool targetIsNullable)
    {
        var result = Mapping?.MappingExpression ?? throw new Exception($"Could not resolve source path. Mapping expected on '{Model.DisplayText ?? Model.Name}' [{Model.SpecializationType}]. Check that you have a MappingTypeResolver that addresses this scenario.");
        foreach (var map in GetParsedExpressionMap(Mapping?.MappingExpression, path => GetSourcePathText(Mapping.GetSource(path).Path, targetIsNullable)))
        {
            result = result.Replace(map.Key, map.Value);
        }
        return result;
    }

    protected string GetSourcePathText(IList<IElementMappingPathTarget> mappingPaths) => GetSourcePathText(mappingPaths, false);

    protected string GetSourcePathText(IList<IElementMappingPathTarget> mappingPaths, bool targetIsNullable)
    {
        return GetSourcePathExpression(mappingPaths, targetIsNullable).ToString();
    }

    protected IDictionary<string, string> GetParsedExpressionMap(string str, Func<string, string> replacePathFunc)
    {
        var result = new Dictionary<string, string>();
        while (str.IndexOf("{", StringComparison.Ordinal) != -1 && str.IndexOf("}", StringComparison.Ordinal) != -1)
        {
            var fromPos = str.IndexOf("{", StringComparison.Ordinal) + 1;
            var toPos = str.IndexOf("}", StringComparison.Ordinal);
            var expression = str[fromPos..toPos];
            var expressionFromPos = 0;
            var resultExpression = expression;
            foreach (var part in Regex.Split(expression, @"[\(\)?:!=+|&]").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
            {
                // if is literal:
                if (part == "true" ||
                    part == "false" ||
                    part == "null" ||
                    (part.StartsWith('"') && part.EndsWith('"')))
                {
                    continue;
                }

                expressionFromPos = resultExpression.IndexOf(part, expressionFromPos, StringComparison.Ordinal);
                resultExpression = resultExpression.Remove(expressionFromPos, part.Length).Insert(expressionFromPos, replacePathFunc(part));
            }

            result.TryAdd($"{{{expression}}}", resultExpression);
            str = str[(str.IndexOf("}", StringComparison.Ordinal) + 1)..];
        }
        return result;
    }

    protected virtual ICSharpExpression GetSourcePathExpression(IList<IElementMappingPathTarget> mappingPaths, bool targetIsNullable)
    {
        CSharpStatement result = default;
        foreach (var mappingPathTarget in mappingPaths)
        {
            if (TryGetSourceReplacement(mappingPathTarget.Element, out var replacement))
            {
                result = !string.IsNullOrWhiteSpace(replacement) ? new CSharpStatement(replacement) : null;
            }
            else
            {
                CSharpStatement member = null;
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

                result = result != null ? new CSharpAccessMemberStatement(result, member) : member;
                var previousMappingPath = mappingPaths.TakeWhile(x => x != mappingPathTarget).LastOrDefault();
                if (IsTransitional(previousMappingPath, mappingPathTarget))
                {
                    if (targetIsNullable
                        && previousMappingPath?.Element.TypeReference?.IsNullable == true
                        && result is CSharpAccessMemberStatement accessMember)
                    {
                        accessMember.IsConditional();
                    }
                }
            }
        }

        return result;
    }

    private bool IsTransitional(IElementMappingPathTarget previousMappingPath, IElementMappingPathTarget mappingPathTarget)
    {
        if (previousMappingPath == null) return false;
        return !((IElement)previousMappingPath?.Element).ChildElements.Contains((IElement)mappingPathTarget?.Element);
    }

    protected string GetTargetPathText()
    {
        var targetPathExpression = GetTargetPathExpression();
        if (targetPathExpression == null)
        {
            throw new Exception($"The target path text returned null for mapping {GetType().Name}. Check that you are resolving the appropriate mapping type for this model: {Model}");
        }
        return GetTargetPathExpression().ToString();
    }

    protected ICSharpExpression GetTargetPathExpression()
    {
        CSharpStatement result = default;
        var mappingPaths = GetTargetPath();
        foreach (var mappingPathTarget in mappingPaths)
        {
            if (TryGetTargetReplacement(mappingPathTarget.Element, out var replacement))
            {
                result = !string.IsNullOrWhiteSpace(replacement) ? new CSharpStatement(replacement) : null;
            }
            else
            {
                CSharpStatement member = null;
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
            }
        }

        return result;
    }

    private bool TryGetReference(IList<IElementMappingPathTarget> mappingPath, out IHasCSharpName reference)
    {
        reference = null;
        var mappingPathTarget = mappingPath.Last();

        if (Template.RootCodeContext?.TryGetReferenceForModel(mappingPathTarget.Id, out reference) == true)
        {
            return true;
        }

        CSharpMetadataBase csharpElement = default;

        var mappingElement = (IElement)mappingPath.First().Element;

        if (TryFindTemplates(mappingElement, out var foundTypeTemplates))
        {
            foreach (var foundTemplate in foundTypeTemplates)
            {
                if (GetReference(mappingPath, foundTemplate.RootCodeContext, out reference))
                {
                    return true;
                }
            }
        }
        else if (TryFindTemplates(mappingElement.ParentElement, out var foundParentTypeTemplate))
        {
            foreach (var foundTemplate in foundParentTypeTemplate)
            {
                if (TryGetReferenceForModelInCSharpFile(foundTemplate.CSharpFile, mappingElement, out reference))
                {
                    if (GetReference(mappingPath, reference as ICSharpCodeContext, out reference))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool GetReference(IList<IElementMappingPathTarget> mappingPath, ICSharpCodeContext csharpElement, out IHasCSharpName reference)
    {
        foreach (var pathTarget in mappingPath)
        {
            if (csharpElement.TryGetReferenceForModel(pathTarget.Id, out reference) == true)
            {
                csharpElement = (ICSharpCodeContext)reference;
                continue;
            }

            if (mappingPath.IndexOf(pathTarget) > 0)
            {
                ITypeReference typeReference = null;
                if (mappingPath[mappingPath.IndexOf(pathTarget) - 1].Element.TypeReference != null)
                {
                    typeReference = mappingPath[mappingPath.IndexOf(pathTarget) - 1].Element.TypeReference.Element.AsTypeReference();
                }
                else
                {
                    continue;
                    //typeReference = mappingPath[mappingPath.IndexOf(pathTarget) - 1].Element.AsTypeReference();
                }

                var foundSubTypeTemplates = Template.GetAllTypeInfo(typeReference).Select(x => x.Template).OfType<ICSharpFileBuilderTemplate>();
                foreach (var foundSubTypeTemplate in foundSubTypeTemplates)
                {
                    if (foundSubTypeTemplate?.CSharpFile.TypeDeclarations.First().TryGetReferenceForModel(pathTarget.Id, out reference) == true)
                    {
                        csharpElement = (ICSharpCodeContext)reference;
                    }
                }

                if (csharpElement is not null)
                {
                    continue;
                }
            }

            csharpElement = null;
            break;
        }

        if (csharpElement is IHasCSharpName hasName)
        {
            reference = hasName;
            return true;
        }

        reference = default;
        return false;
    }

    private static bool TryGetReferenceForModelInCSharpFile(CSharpFile csharpFile, IElement mappedElement, out IHasCSharpName reference)
    {
        reference = null;

        var result = csharpFile.TypeDeclarations.FirstOrDefault()?.TryGetReferenceForModel(mappedElement, out reference) == true
                     || csharpFile.Interfaces.FirstOrDefault()?.TryGetReferenceForModel(mappedElement, out reference) == true;

        return result;
    }

    private bool TryFindTemplates(IElement elementToLookup, out IList<ICSharpFileBuilderTemplate> template)
    {
        if (elementToLookup is null)
        {
            template = null;
            return false;
        }

        template = [];

        var foundTypeInfos = Template.GetAllTypeInfo(elementToLookup.AsTypeReference());
        foreach (var foundTemplate in foundTypeInfos.Select(x => x.Template).OfType<ICSharpFileBuilderTemplate>())
        {
            template.Add(foundTemplate);
        }

        return template.Any();
    }

    /// <summary>
    /// Resolve the reference name after all replacement have been applied for this specified <paramref name="mappingPath"/>
    /// By default this method will return false and supply a null <paramref name="reference"/>. Override this method for your
    /// particular use case if required.
    /// </summary>
    /// <param name="mappingPath"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    protected virtual bool TryGetReferenceName(IElementMappingPathTarget mappingPath, out string reference)
    {
        reference = null;
        return false;
    }
}