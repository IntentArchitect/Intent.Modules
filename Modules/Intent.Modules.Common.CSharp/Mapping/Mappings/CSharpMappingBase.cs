using System;
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

    public virtual CSharpStatement GetSourceStatement()
    {
        return GetSourcePathText();
    }

    public virtual CSharpStatement GetTargetStatement()
    {
        return GetTargetPathText();
    }

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
        var targetPath = mapping.TargetPath.Take(mapping.TargetPath.IndexOf(mapping.TargetPath.Single(x => x.Element == Model)) + 1).ToList();
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

    protected string GetSourcePathText()
    {
        var text = Mapping?.MappingExpression ??
                   throw new Exception(
                       $"Could not resolve source path. Mapping expected on '{Model.DisplayText ?? Model.Name}' [{Model.SpecializationType}]. Check that you have a MappingTypeResolver that addresses this scenario.");
        text = ParseAndReplaceExpression(text, path => GetSourcePathText(Mapping.GetSource(path).Path));

        return text;
    }

    private string ParseAndReplaceExpression(string str, Func<string, string> replacePathFunc)
    {
        var result = str;
        while (str.IndexOf("{", StringComparison.Ordinal) != -1 && str.IndexOf("}", StringComparison.Ordinal) != -1)
        {
            var fromPos = str.IndexOf("{", StringComparison.Ordinal) + 1;
            var toPos = str.IndexOf("}", StringComparison.Ordinal);
            var expression = str[fromPos..toPos];
            var expressionFromPos = 0;
            var resultExpression = expression;
            foreach (var part in Regex.Split(expression, @"[\(\)?:!=]").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
            {
                if (part == "true" ||
                    part == "false" ||
                    part == "null" ||
                    (part.StartsWith('"') && part.EndsWith('"')))
                {
                    continue;
                }

                expressionFromPos = expression.IndexOf(part, expressionFromPos, StringComparison.Ordinal);
                resultExpression = resultExpression.Remove(expressionFromPos, part.Length).Insert(expressionFromPos, replacePathFunc(part));
            }

            result = result.Replace($"{{{expression}}}", resultExpression);
            str = str[(str.IndexOf("}", StringComparison.Ordinal) + 1)..];
        }
        return result;
    }

    protected string GetSourcePathText(IList<IElementMappingPathTarget> mappingPaths)
    {
        return GetSourcePathExpression(mappingPaths).ToString();
    }

    protected ICSharpExpression GetSourcePathExpression(IList<IElementMappingPathTarget> mappingPaths)
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
                    member = new CSharpStatement(mappingPathTarget.Element.Name);
                }

                result = result != null ? new CSharpAccessMemberStatement(result, member) : member;
                var previousMappingPath = mappingPaths.TakeWhile(x => x != mappingPathTarget).LastOrDefault();
                if (IsTransitional(previousMappingPath, mappingPathTarget))
                {
                    if (previousMappingPath?.Element.TypeReference?.IsNullable == true && result is CSharpAccessMemberStatement accessMember)
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
                    member = new CSharpStatement(mappingPathTarget.Element.Name);
                }

                result = result != null ? new CSharpAccessMemberStatement(result, member) : member;
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

        if (TryFindTemplate(mappingElement, out ICSharpFileBuilderTemplate foundTypeTemplate))
        {
            csharpElement = foundTypeTemplate.CSharpFile;
        }
        else if (TryFindTemplate(mappingElement.ParentElement, out ICSharpFileBuilderTemplate foundParentTypeTemplate) &&
                 TryGetReferenceForModelInCSharpFile(foundParentTypeTemplate.CSharpFile, mappingElement, out reference))
        {
            csharpElement = reference as CSharpMetadataBase;
        }

        if (csharpElement != null)
        {
            foreach (var pathTarget in mappingPath)
            {
                if (csharpElement?.TryGetReferenceForModel(pathTarget.Id, out reference) == true)
                {
                    csharpElement = reference as CSharpMetadataBase;
                    continue;
                }

                if (mappingPath.IndexOf(pathTarget) > 0)
                {
                    var foundSubTypeTemplate =
                        Template.GetTypeInfo(mappingPath[mappingPath.IndexOf(pathTarget) - 1].Element.TypeReference.Element.AsTypeReference())?.Template as
                            ICSharpFileBuilderTemplate;
                    if (foundSubTypeTemplate?.CSharpFile.TypeDeclarations.First().TryGetReferenceForModel(pathTarget.Id, out reference) == true)
                    {
                        csharpElement = reference as CSharpMetadataBase;
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
        }

        return false;
    }

    private static bool TryGetReferenceForModelInCSharpFile(CSharpFile csharpFile, IElement mappedElement, out IHasCSharpName reference)
    {
        reference = null;

        var result = csharpFile.TypeDeclarations.FirstOrDefault()?.TryGetReferenceForModel(mappedElement, out reference) == true
                     || csharpFile.Interfaces.FirstOrDefault()?.TryGetReferenceForModel(mappedElement, out reference) == true;

        return result;
    }

    private bool TryFindTemplate(IElement elementToLookup, out ICSharpFileBuilderTemplate template)
    {
        if (elementToLookup is null)
        {
            template = null;
            return false;
        }

        var result = Template.GetTypeInfo(elementToLookup.AsTypeReference());
        if (result.Template is ICSharpFileBuilderTemplate foundTemplate)
        {
            template = foundTemplate;
            return true;
        }

        template = null;
        return false;
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