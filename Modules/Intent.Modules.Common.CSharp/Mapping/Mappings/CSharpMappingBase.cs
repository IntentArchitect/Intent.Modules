using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public abstract class CSharpMappingBase : ICSharpMapping
{
    protected Dictionary<string, string> _sourceReplacements = new();
    protected Dictionary<string, string> _targetReplacements = new();

    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappedEnd Mapping { get; set; }
    protected readonly ICSharpFileBuilderTemplate Template;

    protected CSharpMappingBase(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template)
    {
        Template = template;
        Model = model;
        Mapping = mapping;
        Children = children.Select(x => x.GetMapping()).ToList();
    }

    protected CSharpMappingBase(MappingModel model, ICSharpFileBuilderTemplate template)
    {
        Template = template;
        Model = model.Model;
        Mapping = model.Mapping;
        Children = model.Children.Select(x => x.GetMapping()).ToList();
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

    private IEnumerable<ICSharpMapping> GetAllChildren()
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
        foreach (var child in Children)
        {
            child.SetSourceReplacement(type, replacement);
        }
    }

    public void SetTargetReplacement(IMetadataModel type, string replacement)
    {
        if (_targetReplacements.ContainsKey(type.Id))
        {
            _targetReplacements.Remove(type.Id);
        }
        _targetReplacements.Add(type.Id, replacement);
        foreach (var child in Children)
        {
            child.SetTargetReplacement(type, replacement);
        }
    }

    protected string GetSourcePathText()
    {
        var text = Mapping.MappingExpression;
        var paths = ExtractPaths(Mapping.MappingExpression);
        foreach (var path in paths)
        {
            text = text.Replace($"{{{path}}}", GetPathText(Mapping.GetSource(path).Path, _sourceReplacements));
        }
        return text;
    }

    private IEnumerable<string> ExtractPaths(string str){
        var results = new List<string>();
        while (str.IndexOf("{", StringComparison.Ordinal) != -1 && str.IndexOf("}", StringComparison.Ordinal) != -1) {
            results.Add(str[(str.IndexOf("{", StringComparison.Ordinal) + 1)..str.IndexOf("}", StringComparison.Ordinal)]);
            str = str[(str.IndexOf("}", StringComparison.Ordinal) + 1)..];
        }
        return results;
    }

    protected string GetTargetPathText()
    {
        return GetPathText(GetTargetPath(), _targetReplacements);
    }

    protected string GetPathText(IList<IElementMappingPathTarget> mappingPath, IDictionary<string, string> replacements)
    {
        var result = "";
        foreach (var mappingPathTarget in mappingPath)
        {
            if (replacements.ContainsKey(mappingPathTarget.Element.Id))
            {
                result = replacements[mappingPathTarget.Element.Id] ?? ""; // if map to null, ignore
            }
            else
            {
                var referenceName = Template.CSharpFile.GetReferenceForModel(mappingPathTarget.Id)?.Name ?? mappingPathTarget.Element.Name;
                result += $"{(result.Length > 0 ? "." : "")}{referenceName}";
                if (mappingPathTarget.Element.TypeReference?.IsNullable == true && mappingPath.Last() != mappingPathTarget)
                {
                    result += "?";
                }
            }
        }
        return result;
    }
}