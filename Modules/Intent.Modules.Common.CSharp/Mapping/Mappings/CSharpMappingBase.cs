using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
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
    protected readonly ICSharpFileBuilderTemplate Template;
    protected ICSharpCodeContext Context { get; }

    protected CSharpMappingBase(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template)
    {
        Template = template;
        Model = model;
        Mapping = mapping;
        Children = children.Select(x => x.GetMapping()).ToList();
        Context = template.CSharpFile;
        foreach (var child in Children)
        {
            child.Parent = this;
        }
    }

    protected CSharpMappingBase(MappingModel model, ICSharpFileBuilderTemplate template)
    {
        Template = template;
        Model = model.Model;
        Mapping = model.Mapping;
        Children = model.Children.Select(x => x.GetMapping()).ToList();
        Context = model.CodeContext;
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
        var text = Mapping?.MappingExpression ?? throw new Exception($"Could not resolve source path. Mapping expected on {Model.Name} [{Model.SpecializationType}]");
        var paths = ExtractPaths(Mapping.MappingExpression);
        foreach (var path in paths)
        {
            text = text.Replace($"{{{path}}}", GetSourcePathText(Mapping.GetSource(path).Path));
        }
        return text;
    }

    private IEnumerable<string> ExtractPaths(string str)
    {
        var results = new List<string>();
        while (str.IndexOf("{", StringComparison.Ordinal) != -1 && str.IndexOf("}", StringComparison.Ordinal) != -1)
        {
            results.Add(str[(str.IndexOf("{", StringComparison.Ordinal) + 1)..str.IndexOf("}", StringComparison.Ordinal)]);
            str = str[(str.IndexOf("}", StringComparison.Ordinal) + 1)..];
        }
        return results;
    }

    protected string GetSourcePathText(IList<IElementMappingPathTarget> mappingPaths)
    {
        var result = "";
        foreach (var mappingPathTarget in mappingPaths)
        {
            //if (replacements.ContainsKey(mappingPathTarget.Element.Id))
            //{
            //    result = replacements[mappingPathTarget.Element.Id] ?? ""; // if map to null, ignore
            //}
            if (TryGetSourceReplacement(mappingPathTarget.Element, out var replacement))
            {
                result = replacement ?? ""; // if map to null, ignore
            }
            else
            {
                var referenceName = GetReferenceName(mappingPaths.Take(mappingPaths.IndexOf(mappingPathTarget) + 1).ToList());

                result += $"{(result.Length > 0 ? "." : "")}{referenceName}";
                if (mappingPathTarget.Element.TypeReference?.IsNullable == true && mappingPaths.Last() != mappingPathTarget)
                {
                    result += "?";
                }
            }
        }
        return result;
    }

    protected string GetTargetPathText()
    {
        var result = "";
        var mappingPaths = GetTargetPath();
        foreach (var mappingPathTarget in mappingPaths)
        {
            if (TryGetTargetReplacement(mappingPathTarget.Element, out var replacement))
            {
                result = replacement ?? ""; // if map to null, ignore
            }
            else
            {
                var referenceName = GetReferenceName(mappingPaths.Take(mappingPaths.IndexOf(mappingPathTarget) + 1).ToList());

                result += $"{(result.Length > 0 ? "." : "")}{referenceName}";
                if (mappingPathTarget.Element.TypeReference?.IsNullable == true && mappingPaths.Last() != mappingPathTarget)
                {
                    result += "?";
                }
            }
        }
        return result;
    }

    private string GetReferenceName(IList<IElementMappingPathTarget> mappingPath)
    {
        var mappingPathTarget = mappingPath.Last();
        if (TryGetReferenceName(mappingPathTarget, out var referenceName))
        {
            return referenceName;
        }

        if (Context?.TryGetReferenceForModel(mappingPathTarget.Id, out var reference) == true)
        {
            return reference.Name;
        }

        // try parent type's template:
        if (Template.GetTypeInfo(mappingPath.First().Element.AsTypeReference())?.Template is ICSharpFileBuilderTemplate foundTypeTemplate)
        {
            var csharpElement = foundTypeTemplate.CSharpFile as CSharpMetadataBase;
            foreach (var pathTarget in mappingPath)
            {
                if (csharpElement?.TryGetReferenceForModel(pathTarget.Id, out reference) == true)
                {
                    csharpElement = reference as CSharpMetadataBase;
                    continue;
                }
                if (mappingPath.IndexOf(pathTarget) > 0)
                {
                    var foundSubTypeTemplate = Template.GetTypeInfo(mappingPath[mappingPath.IndexOf(pathTarget) - 1].Element.TypeReference.Element.AsTypeReference())?.Template as ICSharpFileBuilderTemplate;
                    if (foundSubTypeTemplate?.CSharpFile.Classes.First().TryGetReferenceForModel(pathTarget.Id, out reference) == true)
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
                return hasName.Name;
            }
        }

        // try this template:
        if (Template.CSharpFile.TryGetReferenceForModel(mappingPathTarget.Id, out reference))
        {
            return reference.Name;
        }

        // fall back on using the element's name from the metadata:
        return mappingPathTarget.Element.Name;
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