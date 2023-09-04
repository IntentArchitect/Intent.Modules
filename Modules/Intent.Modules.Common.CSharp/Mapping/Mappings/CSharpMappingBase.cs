using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public abstract class CSharpMappingBase : ICSharpMapping
{
    protected Dictionary<string, string> _fromReplacements = new();
    protected Dictionary<string, string> _toReplacements = new();

    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappingConnection Mapping { get; set; }
    protected readonly ICSharpFileBuilderTemplate Template;

    protected CSharpMappingBase(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template)
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
        yield return new CSharpAssignmentStatement(GetToStatement(), GetFromStatement());
    }

    public virtual CSharpStatement GetFromStatement()
    {
        return GetFromPathText();
    }

    public virtual CSharpStatement GetToStatement()
    {
        return GetToPathText();
    }

    protected IList<IElementMappingPathTarget> GetFromPath()
    {
        if (Mapping != null)
        {
            return Mapping.FromPath;
        }

        var mapping = GetAllChildren().First(x => x.Mapping != null).Mapping;
        if (GetAllChildren().Count(x => x.Mapping != null) == 1)
        {
            return mapping.FromPath.Take(mapping.FromPath.Count - 1).ToList();
        }
        var toPath = mapping.FromPath.Take(mapping.FromPath.IndexOf(mapping.FromPath
            .Last(x => GetAllChildren().Where(c => c.Mapping != null).All(c => c.Mapping.FromPath.Contains(x)))) + 1)
            .ToList();
        return toPath;
    }

    protected IList<IElementMappingPathTarget> GetToPath()
    {
        if (Mapping != null)
        {
            return Mapping.ToPath;
        }
        var mapping = GetAllChildren().First(x => x.Mapping != null).Mapping;
        var toPath = mapping.ToPath.Take(mapping.ToPath.IndexOf(mapping.ToPath.Single(x => x.Element == Model)) + 1).ToList();
        return toPath;
    }

    private IEnumerable<ICSharpMapping> GetAllChildren()
    {
        return Children.Concat(Children.SelectMany(x => ((CSharpMappingBase)x).GetAllChildren()).ToList());
    }

    public void SetFromReplacement(IMetadataModel type, string replacement)
    {
        if (_fromReplacements.ContainsKey(type.Id))
        {
            _fromReplacements.Remove(type.Id);
        }
        _fromReplacements.Add(type.Id, replacement);
        foreach (var child in Children)
        {
            child.SetFromReplacement(type, replacement);
        }
    }

    public void SetToReplacement(IMetadataModel type, string replacement)
    {
        if (_toReplacements.ContainsKey(type.Id))
        {
            _toReplacements.Remove(type.Id);
        }
        _toReplacements.Add(type.Id, replacement);
        foreach (var child in Children)
        {
            child.SetToReplacement(type, replacement);
        }
    }

    protected string GetFromPathText()
    {
        return GetPathText(GetFromPath(), _fromReplacements);
    }

    protected string GetToPathText()
    {
        return GetPathText(GetToPath(), _toReplacements);
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