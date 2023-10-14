using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class MappingModel
{
    private readonly MappingManagerBase _manager;

    public MappingModel(IElementToElementMapping mapping, MappingManagerBase manager) : this(mapping.MappingType, mapping.MappingTypeId, mapping.TargetElement, mapping.MappedEnds, manager)
    {
    }

    public MappingModel(string mappingType,
        string mappingTypeId,
        IElementToElementMappedEnd mapping,
        MappingManagerBase manager) : this(
            mappingType: mappingType,
            mappingTypeId: mappingTypeId,
            model: mapping.TargetElement,
            mappings: new List<IElementToElementMappedEnd>() { mapping },
            manager: manager,
            level: mapping.TargetPath.Count)
    {
    }

    private MappingModel(
        string mappingType,
        string mappingTypeId,
        ICanBeReferencedType model,
        IList<IElementToElementMappedEnd> mappings,
        MappingManagerBase manager,
        int level = 1)
    {
        MappingType = mappingType;
        MappingTypeId = mappingTypeId;
        _manager = manager;
        Model = model;
        Mapping = mappings.SingleOrDefault(x => x.TargetElement == model);
        Children = mappings.Where(x => x.TargetPath.Count > level)
            .GroupBy(x => x.TargetPath.Skip(level).First(), x => x)
            .Select(x => new MappingModel(mappingType, mappingTypeId, x.Key.Element, x.ToList(), manager, level + 1))
            .OrderBy(x => ((IElement)x.Model).Order)
            .ToList();
    }

    public string MappingType { get; }
    public string MappingTypeId { get; }
    public ICanBeReferencedType Model { get; set; }
    public IElementToElementMappedEnd Mapping { get; set; }
    public IList<MappingModel> Children { get; set; }

    public ICSharpMapping GetMapping()
    {
        return _manager.ResolveMappings(this);
    }
}

public abstract class MappingManagerBase
{
    private readonly ICSharpFileBuilderTemplate _template;
    protected Dictionary<IMetadataModel, string> _fromReplacements = new();
    protected Dictionary<IMetadataModel, string> _toReplacements = new();
    private readonly List<IMappingTypeResolver> _mappingResolvers = new();

    protected MappingManagerBase(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public CSharpStatement GenerateCreationStatement(IElementToElementMapping model)
    {
        //var mapping = CreateMapping(new MappingModel(model), GetCreateMappingType);
        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel);
        ApplyReplacements(mapping);

        return mapping.GetSourceStatement();
    }

    public IList<CSharpStatement> GenerateUpdateStatements(IElementToElementMapping model)
    {
        //var mapping = CreateMapping(new MappingModel(model, this), GetUpdateMappingType);
        //ApplyReplacements(mapping);

        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel);//, new ObjectUpdateMapping(mappingModel, _template));
        ApplyReplacements(mapping);

        return mapping.GetMappingStatements().ToList();
    }

    public CSharpStatement GenerateSourceStatementForMapping(IElementToElementMapping model, IElementToElementMappedEnd mappingEnd)
    {
        //var mapping = CreateMapping(new MappingModel(model, this), GetUpdateMappingType);
        //ApplyReplacements(mapping);

        var mappingModel = new MappingModel(model.MappingType, model.MappingTypeId, mappingEnd, this);
        var mapping = ResolveMappings(mappingModel);//, new ObjectUpdateMapping(mappingModel, _template));
        ApplyReplacements(mapping);

        return mapping.GetSourceStatement();
    }

    public void SetFromReplacement(IMetadataModel type, string replacement)
    {
        if (_fromReplacements.ContainsKey(type))
        {
            _fromReplacements.Remove(type);
        }
        _fromReplacements.Add(type, replacement);
    }

    public void SetToReplacement(IMetadataModel type, string replacement)
    {
        if (_toReplacements.ContainsKey(type))
        {
            _toReplacements.Remove(type);
        }
        _toReplacements.Add(type, replacement);
    }

    public ICSharpMapping ResolveMappings(MappingModel model, ICSharpMapping defaultMapping = null)
    {
        foreach (var resolver in _mappingResolvers)
        {
            var found = resolver.ResolveMappings(model);
            if (found != null)
            {
                return found;
            }
        }

        return defaultMapping ?? (model.Mapping != null ? new DefaultCSharpMapping(model, _template) : new MapChildrenMapping(model, _template));
    }
    

    public void AddMappingResolver(IMappingTypeResolver resolver)
    {
        _mappingResolvers.Add(resolver);
    }

    private ICSharpMapping CreateMapping(
        ICanBeReferencedType model,
        IList<IElementToElementMappedEnd> mappings,
        Func<ICanBeReferencedType, IElementToElementMappedEnd, List<ICSharpMapping>, ICSharpMapping> mappingResolver,
        int level = 1)
    {
        var mapping = mappings.SingleOrDefault(x => x.TargetPath.Last().Element == model);
        var children = mappings.Where(x => x.TargetPath.Count > level)
            .GroupBy(x => x.TargetPath.Skip(level).First(), x => x)
            .Select(x => CreateMapping(x.Key.Element, x.ToList(), mappingResolver, level + 1))
            .OrderBy(x => ((IElement)x.Model).Order)
            .ToList();
        return mappingResolver(model, mapping, children.ToList());
    }

    private void ApplyReplacements(ICSharpMapping mapping)
    {
        foreach (var fromReplacement in _fromReplacements)
        {
            mapping.SetSourceReplacement(fromReplacement.Key, fromReplacement.Value);
        }

        foreach (var toReplacement in _toReplacements)
        {
            mapping.SetTargetReplacement(toReplacement.Key, toReplacement.Value);
        }
    }


}